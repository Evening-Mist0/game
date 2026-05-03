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

  

    public override void OnCreatNewCard(BaseCard card)
    {
        base.OnCreatNewCard(card);
        MonoMgr.Instance.StartCoroutine(UpdateDesAtk());

    }

    private IEnumerator UpdateDesAtk()
    {
        yield return null;
        EventCenter.Instance.EventTrigger(E_EventType.Treasure_PenEdgeUpdateAtk);
    }


    public override void OnPlay(BaseCard card)
    {  
        int atk = extraAtk * (Dealer.Instance.nowCards.Count / 3);
        if(atk > maxExtraAtk)
            atk = maxExtraAtk;

        Debug.Log($"[笔锋]当前持有的卡牌数量{Dealer.Instance.nowCards.Count}强化前的卡牌伤害{card.currentAtk}强化后的卡牌伤害{card.currentAtk + atk}");
        card.currentAtk += atk;
    }

    public override void OnPlayFinish(BaseCard card)
    {
        base.OnPlayFinish(card);
        MonoMgr.Instance.StartCoroutine(UpdateDesAtk());
    }

    public override void OnPrevSlected(BaseCardScriptableData data)
    {
        base.OnPrevSlected(data);
        int atk = extraAtk * ((Dealer.Instance.nowCards.Count -1) / 3);
        if (atk > maxExtraAtk)
            atk = maxExtraAtk;

        data.baseAtk += atk;
    }

}
