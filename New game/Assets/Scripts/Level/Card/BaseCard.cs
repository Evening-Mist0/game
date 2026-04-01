
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 卡牌的影响范围类型
/// </summary>
public enum E_CardRangeType
{
    /// <summary>
    /// 矩形范围（1*5，2*2等）
    /// </summary>
    Rectangle,
    /// <summary>
    /// 玩家获得效果
    /// </summary>
    MySelf,
    /// <summary>
    /// 十字范围
    /// </summary>
    Cross,
}

/// <summary>
/// 元素属性伤害属性
/// </summary>
public enum E_Element
{
    /// <summary>
    /// 无属性
    /// </summary>
    None = 0,
    /// <summary>
    /// 火
    /// </summary>
    Fire,
    /// <summary>
    /// 水
    /// </summary>
    Water,
    /// <summary>
    /// 土
    /// </summary>
    Earth,
    /// <summary>
    /// 木
    /// </summary>
    Wood,
}

/// <summary>
/// 卡牌技能效果
/// </summary>
public enum E_CardSkill
{
    /// <summary>
    /// 无效果
    /// </summary>
    None = 0,
    /// <summary>
    /// 灼烧
    /// </summary>
    Burn,
    /// <summary>
    /// 击退
    /// </summary>
    Repel,
    /// <summary>
    /// 禁锢
    /// </summary>
    Imprison,
    /// <summary>
    /// 恢复
    /// </summary>
    Heal,
    /// <summary>
    /// 真伤
    /// </summary>
    TrueDamage,
    /// <summary>
    /// 给予玩家护甲
    /// </summary>
    GetDef,

    /// <summary>
    /// 给予防御塔护甲
    /// </summary>
    AddDefToTower,
}

/// <summary>
/// 卡牌的选择状态
/// </summary>
public enum E_SelectedType
{
    /// <summary>
    /// 用于攻击
    /// </summary>
    Fight,
    /// <summary>
    /// 用于合成
    /// </summary>
    Composite,
    /// <summary>
    /// 未被选中
    /// </summary>
    Idle,
}

/// <summary>
/// 卡牌的类型
/// </summary>
public enum E_CardType
{
    /// <summary>
    /// 基础牌
    /// </summary>
    Base,
    /// <summary>
    /// 合成牌
    /// </summary>
    Combine,
    /// <summary>
    /// 部首牌
    /// </summary>
    Radical,
}

/// <summary>
/// 部首牌的卡牌类型
/// </summary>
public enum E_RadicalCardType
{
    /// <summary>
    /// 夕
    /// </summary>
    Xi,
    /// <summary>
    /// 也
    /// </summary>
    Ye,
    /// <summary>
    /// 可
    /// </summary>
    Ke,
    /// <summary>
    /// 皮
    /// </summary>
    Pi,
}

/// <summary>
/// 卡牌打出的效果类型
/// </summary>
public enum E_CardPlayType
{
    /// <summary>
    /// 直接释放效果
    /// </summary>
    Effect,
    /// <summary>
    /// 放置到地图
    /// </summary>
    Place,
}

/// <summary>
/// 卡牌打出效果字典配置
/// </summary>
[Serializable]
public struct CardSkillPair
{
    public E_CardSkill cardSkill;    //技能枚举
    public int effectValue;  //具体的效果数值
    public int roundValue;  //持续的时间数值
}


//[RequireComponent(typeof(Image)), RequireComponent(typeof(CardEffectControl)), RequireComponent(typeof(Animator)), RequireComponent(typeof(EventTrigger))]
public abstract class BaseCard : MonoBehaviour
{
    [SerializeField]
    protected BaseCardScriptableData cardData;

    #region 卡牌基础配置
    [Header("卡牌基础配置")]
    [HideInInspector]
    public int weight;
    [HideInInspector]
    public string cardID;
    [HideInInspector]
    public E_Element elementType;
    [HideInInspector]
    public E_CardType cardType;
    [HideInInspector]
    public E_CardPlayType cardPlayType;
    [HideInInspector]
    public static int burnAtk = 3;
    [HideInInspector]
    public int currentAtk;//卡牌伤害用于肉鸽
    [HideInInspector]
    public string myResName;
    [HideInInspector]
    public bool isRareCard = false;
    //开始是该否被激活(用于肉鸽),如果没激活就得不到对应合成卡牌
    [HideInInspector]
    public bool isActive = false;
    #endregion

    #region 卡牌范围配置
    [Header("卡牌范围配置")]
    [HideInInspector]
    public E_CardRangeType cardRangeType;
    [HideInInspector]
    public int currentRecRangeWide;//用于肉鸽
    [HideInInspector]
    public int currentRecRangeHigh;//用于肉鸽
    [HideInInspector]
    public int currentCrossRangeUp;//用于肉鸽
    [HideInInspector]
    public int currentCrossRangeDown;//用于肉鸽
    [HideInInspector]
    public int currentCrossRangeLeft;//用于肉鸽
    [HideInInspector]
    public int currentCrossRangeRight;//用于肉鸽
    #endregion

    #region  核心效果数值
    [Header("卡牌核心效果配置")]
    [Tooltip("卡牌技能效果")]
    [HideInInspector]
    public List<CardSkillPair> skills;

    //[Tooltip("效果持续回合数")]
    //public int baseEffectLastRound = 0;
    //[Tooltip("效果具体数值，比如（击退2格）")]
    //public int baseEffectExtraValue = 0;
    #endregion

    #region 卡牌关联控件
  

    //卡牌动画
    private Animator animator;
    #endregion

    #region 留存/消耗规则
    [Header("留存/消耗规则配置")]
    [Tooltip("是否跨回合保留（部首牌强制false，基础/合成牌默认true）")]
    public bool isCrossRoundKeep = true;
    [Tooltip("是否为使用后销毁（后续牌有合成不被消耗的情况)")]
    public bool isUseDestroy = true;
    [Tooltip("「不消耗」效果的触发概率（0-1，如50%则为0.5）")]
    [HideInInspector] public float noConsumeProb = 0f;
    [Tooltip("稀有合成牌是否保留部首牌（适配一气呵成执照，运行时赋值）")]
    [HideInInspector] public bool isKeepRadicalWhenCombine = false;
    #endregion

    #region 美术效果配置
    [Header("美术效果配置")]
    [HideInInspector]
    public GameObject effectPrefab;
    [HideInInspector]
    public int effectSortingOrder = 0;
    [HideInInspector]
    public E_AttackEffectType attackEffectType;
    #endregion

    #region 图鉴解锁配置
    [Header("图鉴解锁配置（预留）")]
    [Tooltip("是否为卡牌图鉴（默认true，典籍/奇物为false,有些典籍卡牌是false）")]
    public bool isCardAlbum = true;
    [Tooltip("是否已解锁图鉴（玩家首次获得时设为true）")]
    public bool isUnlockAlbum = false;
    [HideInInspector]
    public string albumCateId = "card";
    #endregion

    //自身UI控件
    [HideInInspector]
    public CardEffectControl cardEffectControl;
    //public CardEffectUIControl cardEffectControl;

    /// <summary>
    /// 卡牌是否被玩家选中，用于出牌阶段
    /// </summary>
    [HideInInspector]
    public bool isSelected = false;
    [HideInInspector]
    public bool isLeftMouseButtonCliking;
    [HideInInspector]
    public bool isRightMouseButtonCliking;
    [HideInInspector]
    public E_SelectedType selectedType = E_SelectedType.Idle;

    /// <summary>
    /// 使用该卡牌效果的方法,打出卡牌后通过这个委托赋予怪物效果
    /// </summary>
    public UnityAction<BaseMonsterCore,Cell> AddEffectAt;

    private void Awake()
    {
        InitCardValue();
        InitCardContactCotrol();       
        InitCardSkill(skills);
    }

    protected virtual void OnCardAwake() { }

    /// <summary>
    /// 初始化卡牌当前数值为基础值
    /// </summary>
    protected virtual void InitCardValue()
    {
        //初始化基础值     
        //基础配置
        weight = cardData.weight;
        cardID = cardData.cardID;
        elementType = cardData.elementType;
        cardType = cardData.cardType;
        cardPlayType = cardData.cardPlayType;
        //burnAtk = cardData.burnAtk;
        currentAtk = cardData.baseAtk;
        isRareCard = cardData.isRareCard;
        myResName = cardData.MyResName;

        //效果配置
        skills = cardData.skills;
      
        //美术
        effectPrefab = cardData.effectPrefab;
        effectSortingOrder = cardData.effectSortingOrder;
        attackEffectType = cardData.attackEffectType;
        //图鉴
        albumCateId = cardData.albumCateId;

        //范围
        cardRangeType = cardData.CardRangeType;
        // 矩形范围
        currentRecRangeWide = cardData.baseRecRangeWide;
        currentRecRangeHigh = cardData.baseRecRangeHigh;
        // 十字范围
        currentCrossRangeUp = cardData.baseCrossRangeUp;
        currentCrossRangeDown = cardData.baseCrossRangeDown;
        currentCrossRangeLeft = cardData.baseCrossRangeLeft;
        currentCrossRangeRight = cardData.baseCrossRangeright;
    }

    private void InitCardContactCotrol()
    {
        cardEffectControl = this.GetComponent<CardEffectControl>();
        if (cardEffectControl == null)
            Debug.LogError("请挂载组件CardEffectUIControl");

        animator = this.GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("请挂载组件Animator");
    }

    /// <summary>
    /// 赋予卡牌相关技能效果（给予“卡牌效果”变量赋值）
    /// </summary>
    /// <param name="skilltype"></param>
    protected void InitCardSkill(List<CardSkillPair> skillsTypeList)
    {
        AddEffectAt = null;

        for (int i = 0; i < skillsTypeList.Count; i++)
        {

            switch (skillsTypeList[i].cardSkill)
            {
                case E_CardSkill.None:
                    break;
                case E_CardSkill.Burn:
                    AddEffectAt += Effect_Burn;               
                    break;
                case E_CardSkill.Repel:
                    AddEffectAt += Effect_Repel;
                    break;
                case E_CardSkill.Imprison:
                    AddEffectAt += Effect_Imprison;                  
                    break;
                case E_CardSkill.Heal:
                    AddEffectAt += Effect_Heal;                  
                    break;
                case E_CardSkill.GetDef:
                    AddEffectAt += Effect_GetDef;
                    break;
                case E_CardSkill.AddDefToTower:
                    AddEffectAt += Effect_AddDefToTower;
                    break;
                case E_CardSkill.TrueDamage:
                    AddEffectAt += Effect_AddTrueDamage;
                    break;
            }
        }

    }

    #region 具体技能效果相关
    public void Effect_Burn(BaseMonsterCore monster,Cell coreCell)
    {
        if (monster == null)
            return;
        Debug.Log($"[效果]赋予 {monster.name} 灼烧效果");
        int roundValue = GetCardSkilllRoundValue(E_CardSkill.Burn);
        if(roundValue != -1)
        monster.GetBurn(roundValue);
    }
    
    public void Effect_Repel(BaseMonsterCore monster,Cell coreCell)
    {
        if (monster == null)
            return;
        Debug.Log($"[效果]赋予 {monster.name} 击退效果");
        int effectValue = GetCardSkilllEffectValue(E_CardSkill.Repel);
        if(effectValue != -1)
        monster.GetRepel(this,coreCell, effectValue);
    }
    public void Effect_Imprison(BaseMonsterCore monster, Cell coreCell)
    {
        if (monster == null)
            return;
        Debug.Log($"[效果]赋予 {monster.name} 禁锢效果");
        int roundValue = GetCardSkilllRoundValue(E_CardSkill.Imprison);
        if (roundValue != -1)
            monster.GetImprison(roundValue);
    }

    public virtual void Effect_Heal(BaseMonsterCore monster, Cell coreCell)
    {
        Debug.Log($"[效果]赋予 玩家 治愈效果");
        //赋予效果
        int roundValue = GetCardSkilllRoundValue(E_CardSkill.Heal);
        int effectValue = GetCardSkilllEffectValue(E_CardSkill.Heal);
        GamePlayer.Instance.GetHeal(effectValue, roundValue);
        //添加图标
        GamePlayer.Instance.effectControl.AddBuffIcon(E_BuffIconType.Heal);
        GamePlayer.Instance.effectControl.UpdateIconCount(E_BuffIconType.Heal, roundValue);
    }

    public virtual void Effect_GetDef(BaseMonsterCore monster, Cell coreCell)
    {
        Debug.Log($"[效果]赋予 玩家 护甲效果");

    }

    public void Effect_AddDefToTower(BaseMonsterCore monster,Cell coreCell)
    {
        Debug.Log($"[效果]赋予 防御塔 护甲效果");
        if (coreCell.nowStateType == CellStateType.EntityOccupied)
        {
            BaseDefTower tower = coreCell.nowObj as BaseDefTower;
            if (tower != null)
            {
                int roundValue = GetCardSkilllEffectValue(E_CardSkill.AddDefToTower);
                tower.GetDef(roundValue);
            }
            else
                Debug.LogError("该格子上的物体不是防御塔");
        }

    }

    public virtual void Effect_AddTrueDamage(BaseMonsterCore monster, Cell coreCell)
    {
        Debug.Log($"[效果]赋予 真伤 效果");
        monster.TakeDamage(currentAtk, elementType,E_AtkType.CardAtk,true);

    }
    #endregion


    /// <summary>
    /// 得到自己的技能效果相关的持续回合值
    /// </summary>
    /// <param name="cardSkill">自身的技能枚举</param>
    /// <returns></returns>
    private int GetCardSkilllRoundValue(E_CardSkill cardSkill)
    {
        for(int i = 0; i < skills.Count;i++)
        {
            if (skills[i].cardSkill == cardSkill)
                return skills[i].roundValue;
        }
        Debug.LogError($"没有找到该卡牌对应的{cardSkill},返回-1");
        return -1;
    }

    /// <summary>
    /// 得到自己的技能效果相关的效果值
    /// </summary>
    /// <param name="cardSkill">自身的技能枚举</param>
    /// <returns></returns>
    private int GetCardSkilllEffectValue(E_CardSkill cardSkill)
    {
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i].cardSkill == cardSkill)
                return skills[i].effectValue;
        }
        Debug.LogError($"没有找到该卡牌对应的{cardSkill},返回-1");
        return -1;
    }

    private void OnDestroy()
    {
        //清空委托
        AddEffectAt = null;
    }

    
   
    /// <summary>
    /// 销毁该卡牌
    /// </summary>
    public virtual void DestroyMe()
    {
        //在表当中清除该卡牌
        cardEffectControl.ForceUnlockAndReturn();

        // 从合成列表中移除（如果存在）
        if (LevelStepMgr.Instance.machine.nowState is CardOperateState state)
        {
            state.RemoveCardInCompositeList(this);
        }
        PoolMgr.Instance.PushObj(this.gameObject);
    }
}
