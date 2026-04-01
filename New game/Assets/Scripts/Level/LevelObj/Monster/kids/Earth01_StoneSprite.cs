using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth01_StoneSprite : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    [Tooltip("ø…ºı√‚…À∫¶")]
    public int def;

    protected override void OnEnterSpecial(MonsterOnEnter evt)
    {
        base.OnEnterSpecial(evt);
        effectControl.AddBuffIcon(E_BuffIconType.ArbitraryDamegeRedution);
    }
    protected override void OnHurtSpecial(MonsterOnHurt evt)
    {
        base.OnHurtSpecial(evt);
        evt.resultAtk -= def;
        if (evt.resultAtk < 0) 
        evt.resultAtk = 0;
    }
}
