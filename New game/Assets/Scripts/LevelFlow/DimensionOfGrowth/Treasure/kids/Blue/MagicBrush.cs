using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBrush : I_Treasure
{
    //增加到的高度
    private int extraHight = 2;

    public int weight = 0;

    public void OnCreateDefTower(BasePlaceCard card)
    {

    }

    public void OnDrawCard(BaseCard card)
    {
        card.currentRecRangeHigh = extraHight;
        Debug.Log($"[神来之笔]卡牌{card.cardID}高+1");
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
