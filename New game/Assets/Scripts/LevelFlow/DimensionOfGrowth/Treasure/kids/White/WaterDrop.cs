using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : I_Treasure
{
    //水基础牌附加的伤害值
    private int extraDamage = 1;
    public void OnCreateDefTower(BasePlaceCard card)
    {

    }

    public void OnDrawCard(BaseCard card)
    {
        if (card.elementType == E_Element.Water && card.cardType == E_CardType.Base)
        {
            Debug.Log("[水滴]更新基础水牌伤害：" + extraDamage);
            card.cardEffectControl.UpdateDesAtk(card.currentAtk += extraDamage);
        }
    }

    public void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Water && card.cardType == E_CardType.Base)
        {
            Debug.Log("[水滴]基础水牌伤害增加：" + extraDamage);
            card.currentAtk += extraDamage;
        }
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
