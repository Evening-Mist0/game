
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 卡牌的影响范围
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
    cross,
}

/// <summary>
/// 卡牌的元素
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

public abstract class BaseCard : MonoBehaviour
{
    [Header("卡牌基础配置")]
    [Tooltip("卡牌ID")]
    public string cardID;
    [Tooltip("卡牌属性")]
    public E_CardElement elementType;
    [Tooltip("卡牌类型")]
    public E_CardType cardType;
    [Tooltip("卡牌伤害")]
    public int baseAtk;
    [HideInInspector]
    public int currentAtk;
    [Tooltip("是否为放置类卡牌")]
    public bool isPlaceCard = false;
    //开始是该否被激活(用于肉鸽),如果没激活就得不到对应合成卡牌
    [HideInInspector]
    public bool isActive = false;

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

    [Header("卡牌效果配置")]
    [Tooltip("卡牌效果")]
    public E_CardSkill skill;

    [Header("卡牌UI配置")]
    [HideInInspector]
    public RectTransform myUIPos;
    //UI控件
    [HideInInspector]
    public CardEffectControl cardEffectControl;


    //事件发生控件
    public CardEventBinder cardEventBinder;

    /// <summary>
    /// 卡牌是否被玩家选中，用于出牌阶段
    /// </summary>
    //[HideInInspector]
    public bool isSelected = false;
    public bool isLeftMouseButtonCliking;
    public bool isRightMouseButtonCliking;
    //[HideInInspector]
    public E_SelectedType selectedType = E_SelectedType.Idle;

    /// <summary>
    /// 使用该卡牌效果的方法,打出卡牌后通过这个委托赋予怪物效果
    /// </summary>
    public UnityAction<BaseMonster> AddEffectAt;


    private void Awake()
    {
        #region 基础配置
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

        //获取控件
        cardEventBinder = this.GetComponent<CardEventBinder>();
        if (cardEventBinder == null)
            Debug.LogError("请挂载组件cardEventBinder");

        cardEffectControl = this.GetComponent<CardEffectControl>();
        if (cardEffectControl == null)
            Debug.LogError("请挂载组件CardEffectControl");

        // 调试：检查AddEffectAt是否为空
        if (AddEffectAt == null)
        {
            Debug.LogWarning($"卡牌{cardID}的AddEffectAt委托为空，skill类型：{skill}");
        }
        #endregion

        #region 技能效果
        //初始化子卡牌的技能效果
        InitCardSkill(skill);
        //添加赋予卡牌效果时机的事件监听
        if (AddEffectAt != null)
        {
            EventCenter.Instance.AddEventListener<BaseMonster>(E_EventType.MonsterHurt, AddEffectAt);
        }
        #endregion
    }

    #region 具体技能效果相关
    public void Effect_Burn(BaseMonster mosnter)
    {
        Debug.Log("赋予灼烧效果");
    }
    public void Effect_Repel(BaseMonster mosnter)
    {
        Debug.Log("赋予击退效果");
    }
    public void Effect_Imprison(BaseMonster mosnter)
    {
        Debug.Log("赋予禁锢效果");
    }
    public void Effect_Reflect(BaseMonster mosnter)
    {
        Debug.Log("赋予反伤效果");
    }
    public void Effect_Heal(BaseMonster mosnter)
    {
        Debug.Log("赋予治愈效果");
    }

    public void Effect_TrueDamage(BaseMonster mosnter)
    {
        Debug.Log("赋予真伤效果");
    }
    #endregion


    private void Update()
    {

    }

    private void OnDestroy()
    {
        //清空委托
        EventCenter.Instance.RemoveEventListener<BaseMonster>(E_EventType.MonsterHurt, AddEffectAt);
        AddEffectAt = null;
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

    public void DestroyMe()
    {
        //清空表
        if(LevelStepMgr.Instance.machine.nowState as CardOperateState is CardOperateState)
        {
            CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
            state.RemoveCardInCompositeList(this);
            Destroy(this.gameObject);
            return;
        }
        Debug.Log("该状态不处于出牌阶段，删除无效");
    }
}
