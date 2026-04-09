using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flint : BaseTreasure, I_Treasure
{
    //火系卡牌计数
    private int currentFireCardCount;
    //可以抽到多少张火基础牌
    private int fireCardCount = 2;
    public void OnCreateDefTower(BaseCard card)
    {
      
    }

    public void OnDrawCard(BaseCard card)
    {
        if (card.elementType != E_Element.Fire)
        {
            if (currentFireCardCount > fireCardCount)
                return;

            Dealer.Instance.RemoveCard(card);
            Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_fire_huo, 0);
            currentFireCardCount++;
        }
    }

    public void OnPlay(BaseCard card)
    {
        
    }

    public void OnSynthesis(BaseCard card)
    {

    }
}
