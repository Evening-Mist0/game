using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Shell : BaseTreasure, I_Treasure
{
    public void OnCreateDefTower(BaseCard card)
    {
      
    }

    public void OnDrawCard(BaseCard card)
    {
      
    }

    public void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Water && card.cardType == E_CardType.Base)
            card.isUseDestroy = false;
    }

    public void OnSynthesis(BaseCard card)
    {
        
    }

    public void ResetMyself()
    {
        
    }
}
