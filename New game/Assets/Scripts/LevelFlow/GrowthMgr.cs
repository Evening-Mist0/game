using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 成长系统管理器
/// 负责执照升级、典籍、奇物逻辑与数据管理
/// </summary>
public class GrowthMgr : BaseMgr<GrowthMgr>
{
    /// <summary>
    /// 配置数据
    /// </summary>
    private LevelUpOptionConfigSO levelUpConfig;
    private BookConfigSO bookConfig;
    private RelicConfigSO relicConfig;


    /// <summary> 
    /// 运行时数据
    /// </summary>
    public PlayerGrowthData growthData { get; private set; }


    // 私有构造函数 符合单例规范
    private GrowthMgr()
    {
        growthData = new PlayerGrowthData();
        // 加载配置数据
        levelUpConfig = Resources.Load<LevelUpOptionConfigSO>("Config/LevelUpOptionConfig");
        bookConfig = Resources.Load<BookConfigSO>("Config/BookConfig");
        relicConfig = Resources.Load<RelicConfigSO>("Config/RelicConfig");
    }

    /// <summary>
    /// 生命周期
    /// </summary>
    //新局初始化玩家数据
    public void InitNewGameData(int initMaxHp = 30, int initHp = 30)
    {
        growthData.ResetData(initMaxHp, initHp);
    }

    //重置成长数据
    public void ResetGrowthData()
    {
        growthData.ResetData(30, 30);
    }


    #region 玩家血量与属性管理
    /// <summary> 
    /// 玩家受到伤害 
    /// </summary>
    public void PlayerTakeDamage(int damage, bool isRealDamage = false)
    {
        if (isRealDamage)
        {
            // 真实伤害 直接扣血
            growthData.playerCurrentHp = Mathf.Max(0, growthData.playerCurrentHp - damage);
        }
        else
        {
            // 先扣护甲
            if (growthData.playerCurrentArmor > 0)
            {
                int remainDamage = damage - growthData.playerCurrentArmor;
                growthData.playerCurrentArmor = Mathf.Max(0, growthData.playerCurrentArmor - damage);
                if (remainDamage > 0)
                    growthData.playerCurrentHp = Mathf.Max(0, growthData.playerCurrentHp - remainDamage);
            }
            else
            {
                growthData.playerCurrentHp = Mathf.Max(0, growthData.playerCurrentHp - damage);
            }
        }

        // 通知血量变更
        EventCenter.Instance.EventTrigger(E_EventType.Growth_PlayerHpChanged,
            (growthData.playerCurrentHp, growthData.playerMaxHp));

        // 血量为0 触发爬塔失败
        if (growthData.playerCurrentHp <= 0)
        {
            LevelFlowMgr.Instance.OnTowerFailed();
        }
    }

    /// <summary> 
    /// 玩家恢复血量 
    /// </summary>
    public void PlayerRecoverHp(int recoverValue)
    {
        growthData.playerCurrentHp = Mathf.Min(growthData.playerMaxHp, growthData.playerCurrentHp + recoverValue);
        EventCenter.Instance.EventTrigger(E_EventType.Growth_PlayerHpChanged,
            (growthData.playerCurrentHp, growthData.playerMaxHp));
    }

    /// <summary> 
    /// 增加玩家最大血量 
    /// </summary>
    public void AddPlayerMaxHp(int addValue)
    {
        growthData.playerMaxHp += addValue;
        growthData.playerCurrentHp += addValue; // 增加上限同时加当前血量
        EventCenter.Instance.EventTrigger(E_EventType.Growth_PlayerHpChanged,
            (growthData.playerCurrentHp, growthData.playerMaxHp));
    }

    /// <summary> 
    /// 添加护甲 
    /// </summary>
    public void AddArmor(int armorValue)
    {
        growthData.playerCurrentArmor += armorValue;
    }


    /// <summary> 
    /// 回合结束结算护甲(可根据需求调整) 
    /// </summary>
    public void OnRoundEndClearArmor()
    {
        growthData.playerCurrentArmor = 0;
    }
    #endregion

    #region 执照升级系统

    /// <summary> 
    /// 增加执照经验 
    /// </summary>
    public void AddLicenseExp(int addExp)
    {
        if (growthData.licenseLevel >= levelUpConfig.maxLevel) return;

        growthData.licenseExp += addExp;
        EventCenter.Instance.EventTrigger(E_EventType.Growth_LicenseExpChanged, growthData.licenseExp);

        // 检查是否升级
        CheckLevelUp();
    }


    /// <summary> 
    /// 检查升级 
    /// </summary>
    private void CheckLevelUp()
    {
        int needExp = levelUpConfig.expPerLevel;
        while (growthData.licenseExp >= needExp && growthData.licenseLevel < levelUpConfig.maxLevel)
        {
            // 扣除经验
            growthData.licenseExp -= needExp;
            // 等级+1
            growthData.licenseLevel++;
            // 触发升级事件
            EventCenter.Instance.EventTrigger(E_EventType.Growth_LicenseLevelUp, growthData.licenseLevel);
            // 弹出升级选择面板
            //UIMgr.Instance.ShowPanel<LevelUpPanel>(E_UILayerType.top);
        }
    }


    /// <summary> 
    /// 随机生成3个不重复的升级选项 
    /// </summary>
    public List<LevelUpOptionConfig> GetRandomLevelUpOptions()
    {
        // 过滤已选择的选项
        var availableOptions = levelUpConfig.optionPool
            .Where(o => !growthData.selectedLevelUpOptions.Contains(o.optionType))
            .ToList();

        // 随机3个
        List<LevelUpOptionConfig> result = new List<LevelUpOptionConfig>();
        int count = Mathf.Min(3, availableOptions.Count);
        for (int i = 0; i < count; i++)
        {
            int randomIndex = Random.Range(0, availableOptions.Count);
            result.Add(availableOptions[randomIndex]);
            availableOptions.RemoveAt(randomIndex);
        }
        return result;
    }

    /// <summary> 
    /// 选择升级选项 
    /// </summary>
    public void SelectLevelUpOption(E_LevelUpOptionType optionType)
    {
        if (growthData.selectedLevelUpOptions.Contains(optionType)) return;

        growthData.selectedLevelUpOptions.Add(optionType);
        // 执行选项对应的效果(即时生效的效果)
        switch (optionType)
        {
            case E_LevelUpOptionType.HpMaxAdd:
                AddPlayerMaxHp(5);
                break;
            case E_LevelUpOptionType.HandCardMaxAdd:
                // 通知卡牌模块修改手牌上限
                break;
                // 其他选项均为被动效果，由卡牌/战斗模块主动查询
        }
    }


    /// <summary> 
    /// 检查是否已选择某升级选项 
    /// </summary>
    public bool HasLevelUpOption(E_LevelUpOptionType optionType)
    {
        return growthData.selectedLevelUpOptions.Contains(optionType);
    }
    #endregion

    #region 典籍系统

    /// <summary> 
    /// 获得典籍 
    /// </summary>
    public bool AddBook(E_BookType bookType)
    {
        // 检查上限
        if (growthData.ownedBooks.Count >= growthData.maxBookCount)
        {
            Debug.LogWarning("典籍数量已达上限，无法获得");
            return false;
        }
        if (growthData.ownedBooks.Contains(bookType))
        {
            Debug.LogWarning("已拥有该典籍");
            return false;
        }

        growthData.ownedBooks.Add(bookType);
        EventCenter.Instance.EventTrigger(E_EventType.Growth_AddBook, bookType);
        return true;
    }

    /// <summary> 
    /// 检查是否拥有某典籍 
    /// </summary>
    public bool HasBook(E_BookType bookType)
    {
        return growthData.ownedBooks.Contains(bookType);
    }


    /// <summary> 
    /// 获取随机未拥有的典籍 
    /// </summary>
    public List<BookConfig> GetRandomUnownedBooks(int count)
    {
        var unownedBooks = bookConfig.bookConfigs
            .Where(b => !growthData.ownedBooks.Contains(b.bookId))
            .ToList();

        List<BookConfig> result = new List<BookConfig>();
        int getCount = Mathf.Min(count, unownedBooks.Count);
        for (int i = 0; i < getCount; i++)
        {
            int randomIndex = Random.Range(0, unownedBooks.Count);
            result.Add(unownedBooks[randomIndex]);
            unownedBooks.RemoveAt(randomIndex);
        }
        return result;
    }


    /// <summary> 
    /// 获取典籍配置 
    /// </summary>
    public BookConfig GetBookConfig(E_BookType bookType)
    {
        return bookConfig.bookConfigs.Find(b => b.bookId == bookType);
    }
    #endregion

    #region 奇物系统

    /// <summary> 
    /// 获得奇物 
    /// </summary>
    public void AddRelic(string relicId)
    {
        if (growthData.ownedRelicIds.Contains(relicId)) return;

        growthData.ownedRelicIds.Add(relicId);
        EventCenter.Instance.EventTrigger(E_EventType.Growth_AddRelic, relicId);
    }


    /// <summary> 
    /// 检查是否拥有某奇物 
    /// </summary>
    public bool HasRelic(string relicId)
    {
        return growthData.ownedRelicIds.Contains(relicId);
    }

 
    /// <summary> 
    /// 按品级随机获取奇物配置 
    /// </summary>
    public List<RelicConfig> GetRandomRelicsByQuality(E_RelicQuality quality, int count)
    {
        var relics = relicConfig.relicConfigs.Where(r => r.quality == quality).ToList();
        List<RelicConfig> result = new List<RelicConfig>();
        int getCount = Mathf.Min(count, relics.Count);
        for (int i = 0; i < getCount; i++)
        {
            int randomIndex = Random.Range(0, relics.Count);
            result.Add(relics[randomIndex]);
            relics.RemoveAt(randomIndex);
        }
        return result;
    }

    /// <summary> 
    /// 按掉落概率随机奇物(普通战斗掉落) 
    /// </summary>
    public RelicConfig GetRandomRelicByDropRate()
    {
        // 白色70% 绿色30%
        int random = Random.Range(0, 100);
        E_RelicQuality quality = random < 70 ? E_RelicQuality.White : E_RelicQuality.Green;
        var relics = relicConfig.relicConfigs.Where(r => r.quality == quality).ToList();
        if (relics.Count == 0) return null;
        return relics[Random.Range(0, relics.Count)];
    }

 
    /// <summary> 
    /// 精英战斗奇物3选1 
    /// </summary>
    public List<RelicConfig> GetEliteBattleRelicOptions()
    {
        List<RelicConfig> result = new List<RelicConfig>();
        // 白色20% 绿色50% 蓝色30%
        for (int i = 0; i < 3; i++)
        {
            int random = Random.Range(0, 100);
            E_RelicQuality quality;
            if (random < 20) quality = E_RelicQuality.White;
            else if (random < 70) quality = E_RelicQuality.Green;
            else quality = E_RelicQuality.Blue;

            var relics = relicConfig.relicConfigs.Where(r => r.quality == quality && !result.Contains(r)).ToList();
            if (relics.Count > 0)
            {
                var relic = relics[Random.Range(0, relics.Count)];
                result.Add(relic);
            }
        }
        return result;
    }

    /// <summary> 
    /// 获取奇物配置 
    /// </summary>
    public RelicConfig GetRelicConfig(string relicId)
    {
        return relicConfig.relicConfigs.Find(r => r.relicId == relicId);
    }
    #endregion
}
