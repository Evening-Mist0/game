using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenEdge : BaseTreasure
{
    //每有两张牌额外伤害
    private int extraAtk = 1;
    //最高达到的伤害
    private int maxExtraAtk = 3;

    public int weight = 1;

    public override E_TreasureType type => E_TreasureType.PenEdge;

    public override int round => -1;

    public override void OnSynthesis(BaseCard card)
    {

        int currentCardCounts = Dealer.Instance.nowCards.Count;
        EventCenter.Instance.EventTrigger<int>(E_EventType.Treasure_PenEdgeUpdateAtk, currentCardCounts);

    }

    public override void OnDrawCard(BaseCard card)
    {
        base.OnDrawCard(card);

        int currentCardCounts = Dealer.Instance.nowCards.Count;
        Debug.Log($"[笔峰]抽牌时根据牌的数量更新攻击值，当前牌的数量为"+ currentCardCounts);
        EventCenter.Instance.EventTrigger<int>(E_EventType.Treasure_PenEdgeUpdateAtk, currentCardCounts);

    }


    public override void OnPlay(BaseCard card)
    {  
        int atk = extraAtk * (Dealer.Instance.nowCards.Count / 2);
        if(atk > maxExtraAtk)
            atk = maxExtraAtk;

        Debug.Log($"[笔峰]当前持有的卡牌数量{Dealer.Instance.nowCards.Count}强化前的卡牌伤害{card.currentAtk}强化后的卡牌伤害{card.currentAtk + atk}");
        card.currentAtk += atk;

    
    }

    public override void OnPlayFinish(BaseCard card)
    {
        int currentCardCounts = Dealer.Instance.nowCards.Count - 1;
        EventCenter.Instance.EventTrigger<int>(E_EventType.Treasure_PenEdgeUpdateAtk, currentCardCounts);
        Debug.Log($"[笔峰]出牌结束，读取道当前手牌还有{currentCardCounts}张");

    }

    public override void OnPrevSlected(BaseCardScriptableData data)
    {
        base.OnPrevSlected(data);
        int atk = extraAtk * ((Dealer.Instance.nowCards.Count -1) / 2);
        if (atk > maxExtraAtk)
            atk = maxExtraAtk;

        data.baseAtk += atk;
    }

}
