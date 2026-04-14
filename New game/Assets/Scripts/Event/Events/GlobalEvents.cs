using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEvents : GameEventBase
{
   
}

/// <summary>
/// 点击打牌面板的Over按钮，从CardOperateState到MonsterEnterSettle时候trigger该事件
/// 用于更新怪物的负面状态、通知有攻击力的防御塔攻击怪物等
/// </summary>
public class OnEnterMonsterSettelEvent : GlobalEvents
{

}

public class OnExitCardOperateStateEvent : GlobalEvents
{

}

public class OnExitMonsterMoveStateEvent : GlobalEvents
{

}

public class OnPlaceDefTower_Ke : GlobalEvents
{
    /// <summary>
    /// 建筑物放在哪一列
    /// </summary>
    public int currentColumn;

    /// <summary>
    /// 当前列存在的柯数量
    /// </summary>
    public int currentColumnCounts;
}

public class OnDestoryDefTower_Ke : GlobalEvents
{
    /// <summary>
    /// 建筑物放在哪一列
    /// </summary>
    public int currentColumn;
    /// <summary>
    /// 被哪个怪物摧毁
    /// </summary>
    public BaseMonsterCore monster;
}

public class OnAtkDefTower_Ke : GlobalEvents
{
    /// <summary>
    /// 建筑物放在哪一列
    /// </summary>
    public int currentColumn;

    /// <summary>
    /// 被哪个怪物伤害
    /// </summary>
    public BaseMonsterCore monster;
}
