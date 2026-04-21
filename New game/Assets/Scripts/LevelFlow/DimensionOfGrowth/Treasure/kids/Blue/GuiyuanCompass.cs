using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuiyuanCompass : BaseTreasure
{
    public int weight = 5;
    public override int round => -1;


    public override E_TreasureType type => E_TreasureType.GuiyuanCompass;

    public override void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Earth && card.cardType == E_CardType.Base)
        {
            bool isUseDestory = Random.Range(0, 2) == 0 ? true : false;
            Debug.Log("[归元罗盘]50%概率打出土牌不消耗,本次为" + isUseDestory);
            card.isUseDestroy = isUseDestory;
        }
    }

  
}
