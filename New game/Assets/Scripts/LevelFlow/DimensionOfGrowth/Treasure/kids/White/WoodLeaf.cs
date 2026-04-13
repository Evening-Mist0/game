using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodLeaf : I_Treasure  
{
    //木基础牌附加的伤害值
    private int extraDamage = 1;
    public void OnCreateDefTower(BasePlaceCard card)
    {

    }

    public void OnDrawCard(BaseCard card)
    {
        if (card.elementType == E_Element.Wood  && card.cardType == E_CardType.Base)
        {
            Debug.Log("[木叶]更新基础土牌伤害：" + extraDamage);
            card.cardEffectControl.UpdateDesAtk(card.currentAtk += extraDamage);
        }
    }

    public void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Wood && card.cardType == E_CardType.Base)
        {
            Debug.Log("[木叶]基础木牌伤害增加：" + extraDamage);
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
