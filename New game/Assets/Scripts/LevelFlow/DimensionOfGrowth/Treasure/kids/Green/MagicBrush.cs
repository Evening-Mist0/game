using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBrush : BaseTreasure
{
    //增加到的高度
    private int newHight = 2;

    public int weight = 0;


    public override void OnDrawCard(BaseCard card)
    {
        //更新卡牌范围
        card.cardEffectControl.UpdateDesRange(card.currentRecRangeWide, newHight);

        card.currentRecRangeHigh = newHight;
        Debug.Log($"[神来之笔]卡牌{card.cardID}高度更新为{card.currentRecRangeHigh}");
    }

    public override void OnPlayFinish(BaseCard card)
    {
 
        //if(card.isUseDestroy && card.cardType == E_CardType.Base)
        //{
        //    card.cardEffectControl.UpdateDesRange(card.currentRecRangeWide, newHight);

        //    card.currentRecRangeHigh = newHight;
        //    Debug.Log($"[神来之笔]卡牌{card.cardID}高度更新为{card.currentRecRangeHigh}");
        //}
    }

   
}
