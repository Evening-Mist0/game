using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paperweight : I_Treasure
{
    //建筑物额外血量
    private int extraHp = 2;

    public int weight = 4;


    public void OnCreateDefTower(BasePlaceCard card)
    {
        Debug.Log("[镇纸]放置的建筑物血量额外增加" + extraHp);
        card.currentExtraDefTowerHp += extraHp;
    }

    public void OnDrawCard(BaseCard card)
    {
     
    }

    public void OnPlay(BaseCard card)
    {

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
