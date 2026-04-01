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
