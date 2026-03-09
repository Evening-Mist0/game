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
/// 卡牌持有技能的具体效果
/// </summary>
public class CardEffect
{

}
public abstract class MyBaseCard : MonoBehaviour
{
    [Header("卡牌基础配置")]
    [Tooltip("卡牌ID")]
    public string cardID;
    [Tooltip("卡牌属性")]
    public E_CardElement elementType;
    [Tooltip("卡牌伤害")]
    public int baseAtk;
    [HideInInspector]
    public int currentAtk;
    [Tooltip("是否为放置类卡牌")]
    public bool isPlaceCard = false;
    [Tooltip("卡牌图片")]
    public Image imgCard;


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

    /// <summary>
    /// 使用该卡牌效果的方法,打出卡牌后通过这个委托赋予怪物效果
    /// </summary>
    public UnityAction<BaseMonster> AddEffectAt;


    private void Awake()
    {
        #region 技能效果
        //初始化子卡牌的技能效果
        InitCardSkill(skill);
        //添加赋予卡牌效果时机的事件监听
        EventCenter.Instance.AddEventListener<BaseMonster>(E_EventType.MonsterHurt, AddEffectAt);
        #endregion

        #region UI
        //添加卡牌UI效果
        if (imgCard == null)
            imgCard = this.GetComponent<Image>();
        if (imgCard == null)
            Debug.LogError("请为该对象挂载Image控件");
        //鼠标进入卡牌
        UIMgr.Instance.AddCustomEventListener<Image>(imgCard, EventTriggerType.PointerEnter, HandlePointerEnter);
        //鼠标移出卡牌
        UIMgr.Instance.AddCustomEventListener<Image>(imgCard, EventTriggerType.PointerExit, HandlePointerExit);
        //鼠标点击卡牌
        UIMgr.Instance.AddCustomEventListener<Image>(imgCard, EventTriggerType.PointerClick, HandlePointerClick);
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


    #region UI相关
    private void HandlePointerEnter(BaseEventData data)
    {
        print("鼠标进入,详细显示");
    }

    private void HandlePointerExit(BaseEventData data)
    {
        print("鼠标离开，放回卡牌");
    }

    private void HandlePointerClick(BaseEventData data)
    {
        print("鼠标点击，IK线出现");
    }

    #endregion

    private void OnDestroy()
    {
        //清空委托
        AddEffectAt = null;
        EventCenter.Instance.RemoveEventListener<BaseMonster>(E_EventType.MonsterHurt, AddEffectAt);
    }

    /// <summary>
    /// 赋予卡牌相关技能效果（给予“卡牌效果”变量赋值）
    /// </summary>
    /// <param name="skilltype"></param>
    protected void InitCardSkill(E_CardSkill skilltype)
    {
        switch(skilltype)
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
}
