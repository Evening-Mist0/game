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
    
}

/// <summary>
/// 受到伤害触发
/// </summary>
public class MonsterOnHurt : MonsterEventBase
{
    public int atk;
    public bool isTrueDamage;
}

/// <summary>
/// 回合更新触发
/// </summary>
public class MonsterOnRound : MonsterEventBase
{

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
