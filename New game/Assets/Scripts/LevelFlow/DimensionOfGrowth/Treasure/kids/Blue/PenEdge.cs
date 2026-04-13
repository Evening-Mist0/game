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


 

    public override void OnDrawCard(BaseCard card)
    {

        int atk = extraAtk * (Dealer.Instance.nowCards.Count / 2);
        if (atk > maxExtraAtk)
            atk = maxExtraAtk;


        Debug.Log($"[笔峰]将卡牌{card.cardID}攻击值更新为{card.currentAtk + atk}增加额外伤害为{atk}");
        card.cardEffectControl.UpdateDesAtk(card.currentAtk + atk);
        //EventCenter.Instance.EventTrigger<int>(E_EventType.Treasure_PenEdgeUpdateAtk, card.currentAtk + atk);
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
        Debug.Log("[笔峰]出牌结束，发送事件更新其他卡牌攻击描述");
        EventCenter.Instance.EventTrigger<int>(E_EventType.Treasure_PenEdgeUpdateAtk,card.currentAtk);
    }

}
