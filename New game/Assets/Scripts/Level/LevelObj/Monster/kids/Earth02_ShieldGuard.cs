using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth02_ShieldGuard : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    [Tooltip("∑¥µØ…À∫¶")]
    public int reflect;

    protected override void OnEnterSpecial(MonsterOnEnter evt)
    {
        base.OnEnterSpecial(evt);
        effectControl.AddBuffIcon(E_BuffIconType.Reflect);
    }

    protected override void OnHurtSpecial(MonsterOnHurt evt)
    {
        base.OnHurtSpecial(evt);
        GamePlayer.Instance.Hurt(reflect);
    }

   
}
