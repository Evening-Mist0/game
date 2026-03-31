using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary> 节点状态 </summary>
public enum E_NodeState
{
    Locked,        // 锁定（灰）
    Unlocked,      // 解锁（亮）
    Current,       // 当前选中
    Completed,     // 已完成（变暗+对勾）
    BossUnlocked   // BOSS解锁（高亮闪烁）
}

/// <summary> 节点类型 </summary>
public enum E_TowerNodeType
{
    None,
    NormalBattle,  // 普通战斗
    EliteBattle,   // 精英战斗
    Camp,          // 休整营地
    RandomEvent,   // 随机事件
    BossBattle     // BOSS战
}





/// <summary> 随机事件类型 </summary>
public enum E_RandomEventType
{
    GetExp,         // 获得执照经验
    SellRelic,      // 变卖奇物
    SellBook,       // 变卖典籍
    TakeDamage,     // 受到伤害
    RecoverHpByRelic// 消耗奇物回血
}



/// <summary> 元素类型 </summary>
public enum E_ElementType
{
    Fire,
    Water,
    Earth,
    Wood
}

/// <summary> 奇物品级 </summary>
public enum E_RelicQuality
{
    White,  // 白色
    Green,  // 绿色
    Blue    // 蓝色
}

/// <summary> 典籍类型 </summary>
public enum E_BookType
{
    Fire_LiaoYuan,      // 火经
    Water_BaiChuan,     // 水经
    Earth_HouTu,        // 土经
    Wood_KuRong,        // 木经
    Composite_TongBian, // 合成经
    Battle_PoWang       // 战法经
}

/// <summary> 执照升级选项类型 </summary>
public enum E_LevelUpOptionType
{
    // 元素充盈系列
    Element_Fire,
    Element_Water,
    Element_Earth,
    Element_Wood,
    // 通用强化
    HpMaxAdd,       // 生命增幅
    InitArmor,      // 初始武装
    CompositeReward,// 妙手偶得
    RareCompositeSave, // 一气呵成
    HandCardMaxAdd, // 手牌扩容
    DrawCardSpeedUp,// 迅捷抽卡
    RadicalSave     // 部首留存
}

public enum E_BookSelectMode
{
    Acquire,   // 获取新典籍（从未拥有的列表中选）
    Sell       // 出售已有典籍（从已拥有的列表中选）
}

public enum E_RelicSelectMode
{
    Acquire,   // 获取新奇物（如精英战三选一，已不使用，但保留）
    Sell,      // 出售奇物换取经验
    Recover    // 消耗奇物恢复血量
}

