using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石块（奇物）
/// </summary>
public class Stone : I_Treasure
{
    //土基础牌附加的伤害值
    private int extraDamage = 1;
    public void OnCreateDefTower(BasePlaceCard card)
    {

    }

    public void OnDrawCard(BaseCard card)
    {
        if (card.elementType == E_Element.Earth && card.cardType == E_CardType.Base)
        {
            Debug.Log("[石块]更新基础土牌伤害：" + extraDamage);
            card.cardEffectControl.UpdateDesAtk(card.currentAtk += extraDamage);
        }
    }

    public void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Earth && card.cardType == E_CardType.Base)
        {
            Debug.Log("[石块]基础土牌伤害增加：" + extraDamage);
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

