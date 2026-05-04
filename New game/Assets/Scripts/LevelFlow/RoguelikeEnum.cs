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
    BossBattle,     // BOSS战
    Shop            //商店节点

}



/// <summary> 随机事件类型 </summary>
public enum E_RandomEventType
{
    Healer,          // 偶遇医师
    Scholar,         // 偶遇学者
    Gambler,         // 千门高手赌局
    TreasureHouse,   // 藏宝库
    ScaleTrade,      // 天平奇物
    //AltarUpgrade     // 祭坛升级
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
    /// <summary>
    /// 燚
    /// </summary>
    Fire_Yi,
    /// <summary>
    /// 灺
    /// </summary>
    Fire_Xie,
    /// <summary>
    /// 焚
    /// </summary>
    Fire_Fen,
    /// <summary>
    /// 淼
    /// </summary>
    Water_Miao,
    /// <summary>
    /// 池
    /// </summary>
    Water_Chi,
    /// <summary>
    /// 淋
    /// </summary>
    Water_Lin,
    /// <summary>
    /// 垚
    /// </summary>
    Earth_Yao,
    /// <summary>
    /// 汋
    /// </summary>
    Earth_Zhuo,
    /// <summary>
    /// 杝
    /// </summary>
    Wood_Yi,
    /// <summary>
    /// 柀
    /// </summary>
    Wood_Bi,
    /// <summary>
    /// 不属于典籍解锁的牌
    /// </summary>
    None,
}

/// <summary> 执照升级选项类型 </summary>
public enum E_LevelUpOptionType
{
    // 通用强化
    HpMaxAdd,       // 生命增幅
    InitArmor,      // 初始武装
    HandCardMaxAdd, // 手牌扩容
    DrawCardSpeedUp,// 迅捷抽卡
    InkGrowthAddSkill,//笔墨充盈
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

