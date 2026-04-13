using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlazingSunPearl : I_Treasure
{
    //»ðÔªËØ¿¨ÅÆµÄ±©»÷ÂÊ
    private int doubleProb = 25;


    public void OnCreateDefTower(BasePlaceCard card)
    {

    }

    public void OnDrawCard(BaseCard card)
    {

    }



    public void OnPlay(BaseCard card)
    {
        bool isDouble = Random.Range(0, 100) < doubleProb;
        Debug.Log($"[ÑÞÑô±¦Öé]»ðÔªËØ¿¨ÅÆ±©»÷ÂÊÅÐ¶¨£¬±¾´ÎÎª{isDouble}");
        if(isDouble && card.elementType == E_Element.Fire)
        {
            card.currentAtk *= 2;
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
