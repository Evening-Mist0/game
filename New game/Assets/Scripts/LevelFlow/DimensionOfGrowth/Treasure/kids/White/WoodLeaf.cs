using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodLeaf : BaseTreasure  
{
    //Ä¾»ù´¡ÅÆ¸½¼ÓµÄÉËº¦Öµ
    private int extraDamage = 1;

    public override E_TreasureType type => E_TreasureType.WoodLeaf;

    public override void OnDrawCard(BaseCard card)
    {
        if (card.elementType == E_Element.Wood)
        {
            Debug.Log("[Ä¾Ò¶]¸üÐÂÄ¾ÅÆÉËº¦£º" + extraDamage);
            int atk = card.currentAtk + extraDamage;
            card.cardEffectControl.UpdateDesAtk(atk);
        }
    }

    public override void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Wood)
        {
            Debug.Log("[Ä¾Ò¶]»ù´¡Ä¾ÅÆÉËº¦Ôö¼Ó£º" + extraDamage);
            card.currentAtk += extraDamage;
        }
    }

  
}
