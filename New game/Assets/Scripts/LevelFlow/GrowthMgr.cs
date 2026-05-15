using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;


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
    public RelicConfigSO relicConfig { get; private set; }


    /// <summary> 
    /// 运行时数据
    /// </summary>
    public PlayerGrowthData growthData { get; private set; }


    // 构造函数
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
        Debug.Log($"恢复血量: {recoverValue}, 变为 {growthData.playerCurrentHp}");
        EventCenter.Instance.EventTrigger(E_EventType.Growth_PlayerHpChanged,
            (growthData.playerCurrentHp, growthData.playerMaxHp));
    }

    /// <summary> 
    /// 增加玩家最大血量 
    /// </summary>
    public void AddPlayerMaxHp(int addValue)
    {
        growthData.playerMaxHp += addValue;
        if(addValue >= 0)
        {
            growthData.playerCurrentHp += addValue; // 增加上限同时加当前血量
        }
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
    /// 回合结束结算护甲
    /// </summary>
    public void OnRoundEndClearArmor()
    {
        growthData.playerCurrentArmor = 0;
        //得到下一次的初始护甲
        growthData.playerCurrentArmor += growthData.playerExtraDef;
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
           growthData.licenseExp -= needExp;
           growthData.licenseLevel++;
    
           EventCenter.Instance.EventTrigger(E_EventType.Growth_LicenseLevelUp, growthData.licenseLevel);
    
           // 获取所有可用选项
           var options = GetAllAvailableLevelUpOptions();
           if (options.Count > 0)
           {
               UIMgr.Instance.ShowPanel<LevelUpPanel>(E_UILayerType.bottom);
               var panel = UIMgr.Instance.GetPanel<LevelUpPanel>();
               panel.ShowWithOptions(options);
           }
           else
           {
               Debug.Log("没有可用的升级选项，不再弹出升级面板");
          }
       }
    }


    /// <summary> 
    /// 生成升级选项 
    /// </summary>
    public List<LevelUpOptionConfig> GetAllAvailableLevelUpOptions()
{
    // 过滤已选择的选项
    var availableOptions = levelUpConfig.optionPool
        .Where(o => !growthData.selectedLevelUpOptions.Contains(o.optionType))
        .ToList();
    return availableOptions;
}

    /// <summary> 
    /// 选择升级选项 
    /// </summary>
    public void SelectLevelUpOption(E_LevelUpOptionType optionType)
    {
        if (growthData.selectedLevelUpOptions.Contains(optionType)) return;

        Debug.Log("玩家获得技能" + optionType);
        growthData.selectedLevelUpOptions.Add(optionType);
        // 执行选项对应的效果
        switch (optionType)
        {
            case E_LevelUpOptionType.HpMaxAdd:
                GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.HpMaxAdd);
                break;
            case E_LevelUpOptionType.HandCardMaxAdd:
                GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.HandCardMaxAdd);               
                break;
            case E_LevelUpOptionType.InitArmor:
                GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.InitArmor);
                break;
            case E_LevelUpOptionType.DrawCardSpeedUp:
                GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.DrawCardSpeedUp);

                break;
            case E_LevelUpOptionType.InkGrowthAddSkill:
                GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.InkGrowthAddSkill);
                break;
           
        }
        //更新局外玩家面板血量信息
        EventCenter.Instance.EventTrigger<(int, int)>(E_EventType.UI_PlayerInfoUpdate,(growthData.playerCurrentHp,growthData.playerMaxHp));
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

        Debug.Log("选中获得典籍" + bookType);
        GamePlayer.Instance.playerBag.AddBook(bookType);
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

    /// <summary>
    /// 获取玩家拥有的典籍配置列表
    /// </summary>
    public List<BookConfig> GetOwnedBookConfigs()
    {
        List<BookConfig> list = new List<BookConfig>();
        foreach (var bookType in growthData.ownedBooks)
        {
            var cfg = GetBookConfig(bookType);
            if (cfg != null) list.Add(cfg);
        }
        return list;
    }

    /// <summary>
    /// 获取未获得的典籍总数（配置表总数量 - 已拥有数量）
    /// </summary>
    public int GetTotalUnownedBooksCount()
    {
        int totalConfigCount = bookConfig.bookConfigs.Count;
        int ownedCount = growthData.ownedBooks.Count;
        return Mathf.Max(0, totalConfigCount - ownedCount);
    }

    public List<BookDisplayData> GetOwnedBookDisplayData()
    {
        List<BookDisplayData> list = new List<BookDisplayData>();
        foreach (var bookType in growthData.ownedBooks)
            {
            var cfg = GetBookConfig(bookType);
            if (cfg == null) continue;
            int level = BookUpgradeMgr.Instance.GetUpgradeLevel(bookType);
            list.Add(new BookDisplayData
            {
                bookType = bookType,
                bookName = cfg.bookName,
                bookDesc = cfg.GetDescription(level),   // 动态描述
                bookIcon = cfg.bookIcon,
                upgradeLevel = level
            });
        }
        return list;
}

    /// <summary>
    /// 移除典籍（用于变卖等事件）
    /// </summary>
    public void RemoveBook(E_BookType bookType)
    {
        if (!growthData.ownedBooks.Contains(bookType))
        {
            Debug.LogWarning($"未拥有典籍 {bookType}，无法移除");
            return;
        }

        growthData.ownedBooks.Remove(bookType);
        GamePlayer.Instance.playerBag.RemoveBook(bookType);
        //EventCenter.Instance.EventTrigger(E_EventType.Growth_RemoveBook, bookType);
    }
    #endregion

    #region 奇物系统

    /// <summary> 
    /// 获得奇物 
    /// </summary>
    public void AddRelic(string relicId)
    {
        if (growthData.ownedRelicIds.Contains(relicId))
        {
            Debug.Log($"奇物 {relicId} 已拥有，不重复添加");
            return;
        }
        growthData.ownedRelicIds.Add(relicId);
        GamePlayer.Instance.playerBag.AddTreasure(relicId);
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
            // 先按品质概率确定品质
        // int random = Random.Range(0, 100);
        // E_RelicQuality quality = random < 70 ? E_RelicQuality.White : E_RelicQuality.Green;
    
        // 过滤
        var candidates = relicConfig.relicConfigs
            .Where(r => r.quality == E_RelicQuality.White && !growthData.ownedRelicIds.Contains(r.relicId))
            .ToList();
        
        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }


    /// <summary> 
    /// 获取奇物配置 
    /// </summary>
    public RelicConfig GetRelicConfig(string relicId)
    {
        return relicConfig.relicConfigs.Find(r => r.relicId == relicId);
    }

    /// <summary>
    /// 获取玩家拥有的奇物配置列表
    /// </summary>
    public List<RelicConfig> GetOwnedRelicConfigs()
    {
        List<RelicConfig> list = new List<RelicConfig>();
        foreach (var RelicType in growthData.ownedRelicIds)
        {
            var cfg = GetRelicConfig(RelicType);
            if (cfg != null) list.Add(cfg);
        }
        return list;
    }

    /// <summary>
    /// 移除奇物（用于变卖、消耗等事件）
    /// </summary>
    public void RemoveRelic(string relicId)
    {
        if (!growthData.ownedRelicIds.Contains(relicId))
        {
            Debug.LogWarning($"未拥有奇物 {relicId}，无法移除");
            return;
        }

        growthData.ownedRelicIds.Remove(relicId);
        GamePlayer.Instance.playerBag.RemoveTreasure(relicId);
        //EventCenter.Instance.EventTrigger(E_EventType.Growth_RemoveRelic, relicId);
    }
    

    /// <summary>
    /// 精英战斗奇物随机掉落
    /// </summary>
    public RelicConfig GetRandomRelicForElite()
    {
        int random = Random.Range(0, 100);
        E_RelicQuality quality;
        if (random < 10) quality = E_RelicQuality.White;
        else if (random < 60) quality = E_RelicQuality.Green;
        else quality = E_RelicQuality.Blue;
        
            var candidates = relicConfig.relicConfigs
            .Where(r => r.quality == quality && !growthData.ownedRelicIds.Contains(r.relicId))
            .ToList();
        
        if (candidates.Count == 0)
        {
            // 降级
            candidates = relicConfig.relicConfigs
                .Where(r => !growthData.ownedRelicIds.Contains(r.relicId))
                .ToList();
        }
        if (candidates.Count == 0) return null;
        return candidates[Random.Range(0, candidates.Count)];
    }
    #endregion

    #region 铜钱系统
    
    public int GetCopperCoins() => growthData.copperCoins;
    public void AddCopperCoins(int amount)
    {
        growthData.copperCoins += amount;
        EventCenter.Instance.EventTrigger(E_EventType.Growth_CopperChanged, growthData.copperCoins);
    }

    public bool SpendCopperCoins(int amount)
    {
        if (growthData.copperCoins >= amount)
        {
            growthData.copperCoins -= amount;
            EventCenter.Instance.EventTrigger(E_EventType.Growth_CopperChanged, growthData.copperCoins);
            EventCenter.Instance.EventTrigger(E_EventType.UI_PlayerMoneyUpdate,growthData.copperCoins);
            return true;
        }
        Debug.LogWarning("铜钱不足");
        return false;
    }
    
     #endregion    
}
