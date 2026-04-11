using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class Cobblestone : I_Treasure
{
    public int weight = 5;


    public void OnCreateDefTower(BasePlaceCard card)
    {
     
    }

    public void OnDrawCard(BaseCard card)
    {
       
    }

    public void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Earth && card.cardType == E_CardType.Base)
        {
            bool isUseDestory = Random.Range(0, 2) == 0 ? true : false;
            Debug.Log("[鹅卵石]50%概率打出土牌不消耗,本次为" + isUseDestory);
            card.isUseDestroy = isUseDestory;
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
