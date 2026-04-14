using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBrush : I_Treasure
{
    //增加到的高度
    private int newHight = 2;

    public int weight = 0;

    public void OnCreateDefTower(BasePlaceCard card)
    {

    }

    public void OnDrawCard(BaseCard card)
    {
        //更新卡牌范围
        card.cardEffectControl.UpdateDesRange(card.currentRecRangeWide, newHight);

        card.currentRecRangeHigh = newHight;
        Debug.Log($"[神来之笔]卡牌{card.cardID}高度更新为{card.currentRecRangeHigh}");
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
