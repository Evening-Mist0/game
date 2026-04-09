using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cobblestone : BaseTreasure, I_Treasure
{
    public void OnCreateDefTower(BaseCard card)
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
        if (card.elementType == E_Element.Earth && card.cardType == E_CardType.Combine)
        {
            Debug.Log("[鹅卵石]土卡牌与部首牌合成成功,奖励一张土基础牌");
            Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_earth_tu, card.transform.GetSiblingIndex());
        }
    }

    public void ResetMyself()
    {

    }
}
