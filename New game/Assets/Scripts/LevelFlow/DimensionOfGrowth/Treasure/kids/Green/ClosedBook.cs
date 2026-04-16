using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedBook : BaseTreasure
{

    //部首卡牌获得计数
    private int currentRewardCardCount = 0;
    //可以获得多少张部首牌
    private int cardRewardCount = 1;

    public int weight = 2;



   
    public override void OnSynthesis(BaseCard card)
    {

        if (currentRewardCardCount < cardRewardCount)
        {
            Debug.Log($"[无字天书]第一次成功合成，奖励当前元素类型的基础牌，合成出的卡牌为"+card.cardID);

            switch (card.elementType)
            {
                case E_Element.None:
                case E_Element.Fire:
                    Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_fire_huo, card.transform.GetSiblingIndex());
                    Debug.Log($"[无字天书]第一次成功合成，奖励火基础牌");

                    break;
                case E_Element.Water:
                    Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_water_shui, card.transform.GetSiblingIndex());
                    Debug.Log($"[无字天书]第一次成功合成，奖励水基础牌");

                    break;
                case E_Element.Earth:
                    Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_earth_tu, card.transform.GetSiblingIndex());
                    Debug.Log($"[无字天书]第一次成功合成，奖励土基础牌");

                    break;
                case E_Element.Wood:
                    Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_wood_mu, card.transform.GetSiblingIndex());
                    Debug.Log($"[无字天书]第一次成功合成，奖励木基础牌");

                    break;
            }
            currentRewardCardCount++;
        }

    }

    public override void ResetOnClickOverTurn()
    {
        Debug.Log("[无字天书]重置回合");
        currentRewardCardCount = 0;
    }
}
