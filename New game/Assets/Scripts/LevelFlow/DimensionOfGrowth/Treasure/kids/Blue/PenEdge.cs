using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenEdge : I_Treasure
{
    //每有两张牌额外伤害
    private int extraAtk = 1;
    //最高达到的伤害
    private int maxExtraAtk = 3;

    public int weight = 1;


    public void OnCreateDefTower(BasePlaceCard card)
    {

    }

    public void OnDrawCard(BaseCard card)
    {

    }

    public void OnPlay(BaseCard card)
    {
        int atk = extraAtk * (Dealer.Instance.nowCards.Count / 2);
        if(atk > maxExtraAtk)
            atk = maxExtraAtk;


        Debug.Log($"[笔峰]当前持有的卡牌数量{Dealer.Instance.nowCards.Count}强化前的卡牌伤害{card.currentAtk}强化后的卡牌伤害{card.currentAtk + atk}");
        card.currentAtk += atk;
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
