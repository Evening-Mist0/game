using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石块（奇物）
/// </summary>
public class Stone : BaseTreasure
{
    //土基础牌附加的伤害值
    private int extraDamage = 1;
    public override int round => -1;


    public override E_TreasureType type => E_TreasureType.Stone;
    public override void OnDrawCard(BaseCard card)
    {
        if (card.elementType == E_Element.Earth)
        {
            Debug.Log("[石块]更新土牌伤害：" + extraDamage);
            int atk = card.currentAtk + extraDamage;

            card.cardEffectControl.UpdateDesAtk(atk);
        }
    }

    public override void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Earth)
        {
            Debug.Log("[石块]基础伤害增加：" + extraDamage);
            card.currentAtk += extraDamage;
        }
    }

 
}

