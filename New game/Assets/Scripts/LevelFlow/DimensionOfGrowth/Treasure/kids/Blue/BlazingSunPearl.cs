using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlazingSunPearl : BaseTreasure
{
    //»ðÔªËØ¿¨ÅÆµÄ±©»÷ÂÊ
    private int doubleProb = 25;

    public override E_TreasureType type => E_TreasureType.BlazingSunPearl;

    public override int round => 0;

    public override void OnPlay(BaseCard card)
    {
        bool isDouble = Random.Range(0, 100) < doubleProb;
        Debug.Log($"[ÑÞÑô±¦Öé]»ðÔªËØ¿¨ÅÆ±©»÷ÂÊÅÐ¶¨£¬±¾´ÎÎª{isDouble}");
        if(isDouble && card.elementType == E_Element.Fire)
        {
            card.currentAtk *= 2;
        }
    }


 

}
