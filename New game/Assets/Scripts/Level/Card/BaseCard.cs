
using System.Collections;
using System.Collections.Generic;
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
/// 卡牌的元素属性
/// </summary>
public enum E_CardElement
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
    /// 反伤
    /// </summary>
    Reflect,
    /// <summary>
    /// 恢复
    /// </summary>
    Heal,
    /// <summary>
    /// 真伤
    /// </summary>
    TrueDamage
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
/// 卡牌持有技能的具体效果
/// </summary>
public class CardEffect
{

}

[RequireComponent(typeof(Image)), RequireComponent(typeof(CardEffectControl)), RequireComponent(typeof(CardEventTrigger)), RequireComponent(typeof(Animator))]
public abstract class BaseCard : MonoBehaviour
{
    #region 卡牌基础配置
    [Header("卡牌基础配置")]
    [Tooltip("卡牌权值(根据权值大小进行List排序)")]
    public int weight;
    [Tooltip("卡牌ID")]
    public string cardID;
    [Tooltip("卡牌元素属性")]
    public E_CardElement elementType;
    [Tooltip("卡牌类型")]
    public E_CardType cardType;
    [Tooltip("卡牌伤害")]
    public int baseAtk;
    [HideInInspector]
    public int currentAtk;//用于肉鸽
    [Tooltip("是否为放置类卡牌")]
    public bool isPlaceCard = false;
    [Tooltip("卡牌稀有度")]
    public bool isRareCard = false;
    //开始是该否被激活(用于肉鸽),如果没激活就得不到对应合成卡牌
    [HideInInspector]
    public bool isActive = false;
    #endregion

    #region 卡牌范围配置
    [Header("卡牌范围配置")]

    [Tooltip("卡牌影响范围类型")]
    public E_CardRangeType CardRangeType;

    [Tooltip("矩形范围类型卡牌影响范围-wide")]
    public int baseRecRangeWide;
    [HideInInspector]
    public int currentRecRangeWide;//用于肉鸽
    [Tooltip("矩形范围类型卡牌影响范围-high")]
    public int baseRecRangeHigh;
    [HideInInspector]
    public int currentRecRangeHigh;//用于肉鸽
    [Tooltip("十字范围类型卡牌影响范围(中心向上扩展)")]
    public int baseCrossRangeUp;
    [HideInInspector]
    public int currentCrossRangeUp;//用于肉鸽
    [Tooltip("十字范围类型卡牌影响范围(中心向下扩展)")]
    public int baseCrossRangeDown;
    [HideInInspector]
    public int currentCrossRangeDown;//用于肉鸽
    [Tooltip("十字范围类型卡牌影响范围(中心向左扩展)")]
    public int baseCrossRangeLeft;
    [HideInInspector]
    public int currentCrossRangeLeft;//用于肉鸽
    [Tooltip("十字范围类型卡牌影响范围(中心向右扩展)")]
    public int baseCrossRangeright;
    [HideInInspector]
    public int currentCrossRangeRight;//用于肉鸽
    #endregion

    #region  核心效果数值
    [Header("卡牌核心效果配置")]
    [Tooltip("卡牌技能效果")]
    public E_CardSkill skill;
    [Tooltip("效果持续回合数")]
    public int baseEffectLastRound = 0;
    [Tooltip("附加效果配套数值-基础值（如击退2格、灼烧每回合3伤）")]
    public int baseExtraEffectValue;
    [Tooltip("放置类卡牌的实体血量-基础值（如木障1点、坷牌土块6点）")]
    public int basePlaceCardHp = 0;
    //[Header("核心效果数值-当前值（运行时由基础值+强化值计算）")]
    //[HideInInspector] public int currentCoreValue;
    //[HideInInspector] public int currentEffectLastRound;
    //[HideInInspector] public int currentExtraEffectValue;
    //[HideInInspector] public int currentPlaceCardHp;
    #endregion

    #region 卡牌关联控件
    //自身UI控件
    [HideInInspector]
    public CardEffectControl cardEffectControl;
    //卡牌动画
    private Animator animator;
    #endregion

    #region 留存/消耗规则
    [Header("留存/消耗规则配置")]
    [Tooltip("是否跨回合保留（部首牌强制false，基础/合成牌默认true）")]
    public bool isCrossRoundKeep = true;
    [Tooltip("是否为使用后销毁（所有卡牌使用/合成后均销毁，默认true）")]
    public bool isUseDestroy = true;
    [Tooltip("当前是否触发「不消耗」效果（适配燧石/贝壳/元素充盈执照，运行时赋值）")]
    [HideInInspector] public bool isNoConsume = false;
    [Tooltip("「不消耗」效果的触发概率（0-1，如50%则为0.5）")]
    [HideInInspector] public float noConsumeProb = 0f;
    [Tooltip("稀有合成牌是否保留部首牌（适配一气呵成执照，运行时赋值）")]
    [HideInInspector] public bool isKeepRadicalWhenCombine = false;
    #endregion

    #region 美术效果配置
    [Header("美术效果配置")]
    [Tooltip("卡牌使用/合成时的特效预制体")]
    public GameObject effectPrefab;
    [Tooltip("特效层级（如火焰最上层、水流在怪物下层）")]
    public int effectSortingOrder = 0;
    #endregion

    #region 图鉴解锁配置
    [Header("图鉴解锁配置（预留）")]
    [Tooltip("是否为卡牌图鉴（默认true，典籍/奇物为false,有些典籍卡牌是false）")]
    public bool isCardAlbum = true;
    [Tooltip("是否已解锁图鉴（玩家首次获得时设为true）")]
    public bool isUnlockAlbum = false;
    [Tooltip("图鉴分类ID（卡牌/典籍/奇物分别对应不同ID，方便面板分类）")]
    public string albumCateId = "card";
    #endregion

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
    public UnityAction<BaseMonster,Cell> AddEffectAt;

    public abstract string MyResName  { get; }

private void Awake()
    {
        InitCardValue();

        InitCardContactCotrol();
         
        InitCardSkill(skill);
    }

    protected virtual void OnCardAwake() { }

    /// <summary>
    /// 初始化卡牌当前数值为基础值
    /// </summary>
    private void InitCardValue()
    {
        //初始化基础值
        // 基础攻击力
        currentAtk = baseAtk;
        // 矩形范围
        currentRecRangeWide = baseRecRangeWide;
        currentRecRangeHigh = baseRecRangeHigh;
        // 十字范围
        currentCrossRangeUp = baseCrossRangeUp;
        currentCrossRangeDown = baseCrossRangeDown;
        currentCrossRangeLeft = baseCrossRangeLeft;
        currentCrossRangeRight = baseCrossRangeright;
    }

    private void InitCardContactCotrol()
    {
        cardEffectControl = this.GetComponent<CardEffectControl>();
        if (cardEffectControl == null)
            Debug.LogError("请挂载组件CardEffectControl");

        animator = this.GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("请挂载组件Animator");
    }

    /// <summary>
    /// 赋予卡牌相关技能效果（给予“卡牌效果”变量赋值）
    /// </summary>
    /// <param name="skilltype"></param>
    protected void InitCardSkill(E_CardSkill skilltype)
    {
        switch (skilltype)
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
            case E_CardSkill.Reflect:
                AddEffectAt += Effect_Reflect;
                break;
            case E_CardSkill.TrueDamage:
                AddEffectAt += Effect_TrueDamage;
                break;
            case E_CardSkill.Heal:
                AddEffectAt += Effect_Heal;
                break;
        }

    }

    #region 具体技能效果相关
    public void Effect_Burn(BaseMonster monster,Cell coreCell)
    {
        Debug.Log($"[效果]赋予 {monster.name} 灼烧效果");
        monster.GetBurn(baseEffectLastRound);
    }
    
    public void Effect_Repel(BaseMonster monster,Cell coreCell)
    {
        GridPos dir = monster.currentPos - coreCell.logicalPos;
        Debug.Log($"[效果]赋予 {monster.name} 击退效果,击退的方向是{dir.x}{dir.y}");
        if(dir.x == 1||dir.x == -1)
            StartCoroutine(monster.MoveHorizontal(1,dir.x));
        else if(dir.y == 1||dir.y == -1)
            StartCoroutine(monster.MoveVertical(1, dir.y, true));
        else if(dir.x == 0 && dir.y == 0)
            StartCoroutine(monster.MoveHorizontal(1,1));
        else
        Debug.Log("[效果]赋予击退效果失败，计算出的位置不合法！");
    }
    public void Effect_Imprison(BaseMonster monster, Cell coreCell)
    {
        Debug.Log($"[效果]赋予 {monster.name} 禁锢效果");
        monster.GetImprison(baseEffectLastRound);

    }
    public void Effect_Reflect(BaseMonster monster, Cell coreCell)
    {
        Debug.Log($"[效果]赋予 {monster.name} 反伤效果");
        monster.GetReflect(currentAtk);
    }
    public void Effect_Heal(BaseMonster monster, Cell coreCell)
    {
        Debug.Log($"[效果]赋予 {monster.name} 治愈效果");
        PlayerTest.Instance.GetHeal(currentAtk);
    }
    public void Effect_TrueDamage(BaseMonster monster, Cell coreCell)
    {
        Debug.Log($"[效果]赋予 {monster.name} 真伤效果");
        monster.GetReflect(currentAtk);
    }
    #endregion

    private void OnDestroy()
    {
        //清空委托
        AddEffectAt = null;
    }

   
    /// <summary>
    /// 销毁该卡牌
    /// </summary>
    public void DestroyMe()
    {
        //在表当中清除该卡牌
        if(LevelStepMgr.Instance.machine.nowState as CardOperateState is CardOperateState)
        {
            CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
            state.RemoveCardInCompositeList(this);
            Destroy(this.gameObject);
            return;
        }       
        Debug.Log("该状态不处于出牌阶段，删除无效");
        Debug.LogWarning("若怪物有删除玩家卡牌的行为，请补充逻辑");
    }
}
