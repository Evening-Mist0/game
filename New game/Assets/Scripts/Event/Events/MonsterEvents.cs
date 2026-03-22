using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEventBase : GameEventBase
{
    
}

/// <summary>
/// 移动时触发
/// </summary>
public class MonsterOnMove : MonsterEventBase
{
    public bool isCoundDestoryDef = false;
    public bool isHorizontalMove;
    public bool isCancelAtk;
}

/// <summary>
/// 受到伤害触发
/// </summary>
public class MonsterOnHurt : MonsterEventBase
{
    /// <summary>
    /// 经过技能特性受到的伤害值
    /// </summary>
    public int resultAtk;
    /// <summary>
    /// 元素伤害类型
    /// </summary>
    public E_Element cardElement;
    /// <summary>
    /// 卡牌的技能效果（主要是用于判断真伤）
    /// </summary>
    public E_CardSkill cardSkill;
}

/// <summary>
/// 回合更新触发
/// </summary>
public class MonsterOnRound : MonsterEventBase
{
    public GridPos currentPos;
}

/// <summary>
/// 进场时触发
/// </summary>
public class MonsterOnEnter : MonsterEventBase
{

}

/// <summary>
/// 死亡时触发
/// </summary>
public class MonsterOnDead : MonsterEventBase
{
    public GridPos currentPos;
}

/// <summary>
/// 低于到血量阈值触发
/// </summary>
public class MonsterOnHpLow : MonsterEventBase
{
   
}

/// <summary>
/// 获得负面效果的时候触发
/// </summary>
public class MonsterOnGetDeBuff : MonsterEventBase
{
    public E_CardSkill skill;
    //是否免疫负面效果
    public bool isImmunity;
}

/// <summary>
/// 清理怪物需要在移动后消除的负面状态
/// </summary>
public class CleanupExpiredBuffs : MonsterEventBase
{

}

public class MonsterOnAtk : MonsterEventBase
{
    /// <summary>
    /// 怪物攻击时处于哪个位置
    /// </summary>
    public GridPos nowPos;

    /// <summary>
    /// 是否是元素法王攻击（攻击完全是另一套我服了）
    /// </summary>
    public bool isElementGodAtk;

    /// <summary>
    /// 攻击的对象是否是怪物
    /// </summary>
    public bool isMonster;

    /// <summary>
    /// 是否为边界攻击
    /// </summary>
    public bool isBoundaryAttack;  
}


