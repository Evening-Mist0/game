using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water03_AbyssEel : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;


    protected override void OnMoveSpecial(MonsterOnMove evt)
    {
        base.OnMoveSpecial(evt);
        evt.isCoundDestoryDef = couldDestoryDefAndAhead;
    }
}
