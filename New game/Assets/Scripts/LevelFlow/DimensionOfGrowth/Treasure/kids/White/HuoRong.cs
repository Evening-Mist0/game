using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuoRong : I_Treasure
{
    //»ð»ù´¡ÅÆ¸½¼ÓµÄÉËº¦Öµ
    private int extraDamage = 1;
    public void OnCreateDefTower(BasePlaceCard card)
    {
        
    }

    public void OnDrawCard(BaseCard card)
    {
        if (card.elementType == E_Element.Fire && card.cardType == E_CardType.Base)
        {
            Debug.Log("[»ðÈÞ]¸üÐÂ»ù´¡»ðÅÆÉËº¦£º" + extraDamage);
            card.cardEffectControl.UpdateDesAtk(card.currentAtk += extraDamage);
        }
    }

    public void OnPlay(BaseCard card)
    {
        if(card.elementType == E_Element.Fire && card.cardType == E_CardType.Base)
        {
            Debug.Log("[»ðÈÞ]»ù´¡»ðÅÆÉËº¦Ôö¼Ó£º" + extraDamage);
            card.currentAtk += extraDamage;
        }
    }

    public void OnSynthesis(BaseCard card)
    {
        
    }

    public void ResetOnClickOverTurn()
    {
        
    }

    public void ResetOnLevelOver()
    {
      
    }
}
