using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EchoConch : BaseTreasure
{
    public int weight = 7;

    public override E_TreasureType type => E_TreasureType.EchoConch;

    public override void OnSynthesis(BaseCard card)
    {
        if (card.elementType == E_Element.Water && card.cardType == E_CardType.Combine)
        {
            bool isReward = Random.Range(0, 2) == 0 ? true : false;
            Debug.Log("[回音海螺]水卡牌与部首牌合成成功,[50%概率]奖励一张水基础牌，本次为" + isReward);
            if (isReward)
                Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_water_shui, card.transform.GetSiblingIndex());
        }
    }

}
