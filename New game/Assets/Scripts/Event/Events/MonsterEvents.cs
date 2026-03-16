using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEventBase : GameEventBase
{
    /// <summary>
    /// 触发事件的单元格实例
    /// </summary>
    public BaseMonster SourceMonster { get; protected set; }

    public MonsterEventBase(BaseMonster sourceMonster)
    {
        SourceMonster = sourceMonster;
    }
}

public class MonsterTurn_StartMoveEvent : MonsterEventBase
{
    public MonsterTurn_StartMoveEvent(BaseMonster sourceMonster) : base(sourceMonster)
    {

    }
}
