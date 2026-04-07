using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCardScriptableData : ScriptableObject
{
    #region 基础配置
    [Header("卡牌基础配置")]
    [Tooltip("卡牌权值(根据权值大小进行List排序)")]
    public int weight;
    [Tooltip("卡牌ID")]
    public string cardID;
    [Tooltip("卡牌元素属性")]
    public E_Element elementType;
    [Tooltip("卡牌类型")]
    public E_CardType cardType;
    [Tooltip("卡牌打出类型")]
    public E_CardPlayType cardPlayType;
    [Tooltip("卡牌伤害")]
    public int baseAtk;
    //[Tooltip("灼烧造成的伤害")]
    //public int burnAtk = 3;
    [Tooltip("卡牌稀有度")]
    public bool isRareCard;
    [Tooltip("卡牌资源路径")]
    public abstract string MyResName { get; }

    #endregion

    #region 范围配置
    [Header("卡牌范围配置")]
    [Tooltip("卡牌影响范围类型")]
    public E_CardRangeType CardRangeType;
    [Tooltip("矩形范围类型卡牌影响范围-wide")]
    public int baseRecRangeWide;
    [Tooltip("矩形范围类型卡牌影响范围-high")]
    public int baseRecRangeHigh;
    [Tooltip("十字范围类型卡牌影响范围(中心向上扩展)")]
    public int baseCrossRangeUp;
    [Tooltip("十字范围类型卡牌影响范围(中心向下扩展)")]
    public int baseCrossRangeDown;
    [Tooltip("十字范围类型卡牌影响范围(中心向左扩展)")]
    public int baseCrossRangeLeft;
    [Tooltip("十字范围类型卡牌影响范围(中心向右扩展)")]
    public int baseCrossRangeright;
    #endregion

    #region 效果数值
    [Header("卡牌核心效果配置")]
    [Tooltip("卡牌技能效果")]
    public List<CardSkillPair> skills;
    #endregion

   
    #region 美术
    [Header("美术效果配置")]
    [Tooltip("卡牌使用/合成时的特效预制体")]
    public GameObject effectPrefab;
    [Tooltip("特效层级（如火焰最上层、水流在怪物下层）")]
    public int effectSortingOrder = 0;
    [Tooltip("卡牌打出时释放的特效")]
    public E_AttackEffectType attackEffectType;
    #endregion

    #region 图鉴
    [Tooltip("图鉴分类ID（卡牌/典籍/奇物分别对应不同ID，方便面板分类）")]
    public string albumCateId = "card";
    #endregion

}
