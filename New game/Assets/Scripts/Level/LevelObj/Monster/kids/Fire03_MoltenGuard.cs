using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire03_MoltenGuard : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    protected override void OnEnterSpecial(MonsterOnEnter evt)
    {
        base.OnEnterSpecial(evt);
        //effectControl.AddBuffIcon(E_BuffIconType.ImmunityBurn);
        //effectControl.AddBuffIcon(E_BuffIconType.FireDamegeRedution);
    }
    protected override void OnHurtSpecial(MonsterOnHurt evt)
    {
        base.OnHurtSpecial(evt);

        // ª—Ê–ŒÃ¨£∫ ‹µΩªœµø®≈∆π•ª˜ ±£¨ºı√‚…À∫¶
        if (evt.cardElement == E_Element.Fire)
        {
            switch (evt.atkType)
            {
                case E_AtkType.CardAtk:
                    evt.resultAtk /= 2;
                    break;
                case E_AtkType.BurnSkill:
                case E_AtkType.DefAtk:
                    evt.resultAtk = 0;
                    break;
            }
        }
    }
}
