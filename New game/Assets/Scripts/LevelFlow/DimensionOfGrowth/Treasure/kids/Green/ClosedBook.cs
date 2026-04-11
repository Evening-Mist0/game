using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClosedBook : I_Treasure
{

    //部首卡牌获得计数
    private int currentradicalCardCount;
    //可以获得多少张部首牌
    private int radicalCardCount = 1;

    public int weight = 2;



    public void OnCreateDefTower(BasePlaceCard card)
    {

    }

    public void OnDrawCard(BaseCard card)
    {

    }

    public void OnPlay(BaseCard card)
    {

    }

    public void OnSynthesis(BaseCard card)
    {

        if (currentradicalCardCount < radicalCardCount)
        {
            Debug.Log($"[无字天书]第一次成功合成，奖励当前元素类型的基础牌");

            switch (card.elementType)
            {
                case E_Element.None:
                case E_Element.Fire:
                    Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_fire_huo, card.transform.GetSiblingIndex());
                    break;
                case E_Element.Water:
                    Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_water_shui, card.transform.GetSiblingIndex());
                    break;
                case E_Element.Earth:
                    Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_earth_tu, card.transform.GetSiblingIndex());
                    break;
                case E_Element.Wood:
                    Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_wood_mu, card.transform.GetSiblingIndex());
                    break;
            }
            currentradicalCardCount++;
        }

    }

    public void ResetOnClickOverTurn()
    {
        Debug.Log("[无字天书]重置回合");
        currentradicalCardCount = 0;
    }

    public void ResetOnLevelOver()
    {

    }
}
