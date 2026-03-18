//// MonsterData.cs - 纯数据，不包含逻辑
//using System;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 怪物元素属性
///// </summary>
//public enum MonsterElement
//{
//    /// <summary>
//    /// 无属性
//    /// </summary>
//    None,
//    /// <summary>
//    /// 火系怪
//    /// </summary>
//    Fire,
//    /// <summary>
//    /// 水系怪
//    /// </summary>
//    Water,
//    /// <summary>
//    /// 土系怪
//    /// </summary>
//    Earth
//}
///// <summary>
///// 怪物身份类型
///// </summary>
//public enum MonsterIdentity
//{
//    /// <summary>
//    /// 基础怪
//    /// </summary>
//    Basic,
//    /// <summary>
//    /// 精英怪
//    /// </summary>
//    Elite,
//    /// <summary>
//    /// Boss怪
//    /// </summary>
//    Boss
//}

///// <summary>
///// 怪物特性触发类型
///// </summary>
//public enum E_MonsterTriggerType
//{
//    /// <summary>
//    /// 死亡触发
//    /// </summary>
//    Death,
//    /// <summary>
//    /// 受击触发
//    /// </summary>
//    Hurt,
//    /// <summary>
//    /// 移动触发
//    /// </summary>
//    Move,
//    /// <summary>
//    /// 进入战场触发
//    /// </summary>
//    Enter,
//    /// <summary>
//    /// 每回合触发
//    /// </summary>
//    Round,
//    /// <summary>
//    /// 血量低于阈值触发
//    /// </summary>
//    HpLow
//}


//[Serializable]
//public class MonsterData
//{
//    [Header("基础信息")]
//    [Tooltip("怪物唯一ID")]
//    public string monsterID;
//    [Tooltip("怪物元素属性")]
//    public MonsterElement element;
//    [Tooltip("怪物身份类型")]
//    public MonsterIdentity identity;

//    [Header("战斗属性")]
//    [Tooltip("怪物最大血量")]
//    public int maxHp;
//    [Tooltip("怪物攻击力")]
//    public int attack;

//    [Header("移动属性")]
//    [Tooltip("基础左移格数/回合")]
//    public int baseMoveStepHorizontal = 1;
//    [Tooltip("基础上/下 移格数/回合（没有上下移动的功能就填-1!）")]
//    public int baseMoveStepVertical = 1; 
//    [Tooltip("移动间隔回合（1=每回合移，2=每2回合移）")]
//    public int moveInterval = 1; 

//    private MonsterData(MonsterData data)
//    {

//    }
//}

