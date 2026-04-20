using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuoRong : BaseTreasure
{
    //»ğ»ù´¡ÅÆ¸½¼ÓµÄÉËº¦Öµ
    private int extraDamage = 1;

    public override E_TreasureType type => E_TreasureType.HuoRong;

    public override void OnDrawCard(BaseCard card)
    {
        if (card.elementType == E_Element.Fire)
        {
            Debug.Log("[»ğÈŞ]¸üĞÂ»ù´¡»ğÅÆÉËº¦£º" + extraDamage);
            int atk = card.currentAtk + extraDamage;

            card.cardEffectControl.UpdateDesAtk(atk);
        }
    }

    public override void OnPlay(BaseCard card)
    {
        if(card.elementType == E_Element.Fire)
        {
            Debug.Log("[»ğÈŞ]»ğÅÆÉËº¦Ôö¼Ó£º" + extraDamage);
            card.currentAtk += extraDamage;
        }
    }

 
}
