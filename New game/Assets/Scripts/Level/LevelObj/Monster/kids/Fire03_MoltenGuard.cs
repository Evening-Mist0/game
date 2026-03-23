using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire03_MoltenGuard : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    protected override void OnHurtSpecial(MonsterOnHurt evt)
    {
        base.OnHurtSpecial(evt);
        if(evt.cardElement == E_Element.Fire)
        {
            evt.resultAtk /= 2;
        }
    }
}
