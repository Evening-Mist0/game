using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : BaseTreasure
{
    //水基础牌附加的伤害值
    private int extraDamage = 1;
    public override int round => -1;


    public override E_TreasureType type => E_TreasureType.WaterDrop;
    public override void OnDrawCard(BaseCard card)
    {
        if (card.elementType == E_Element.Water)
        {
            Debug.Log("[水滴]更新水牌伤害：" + extraDamage);
            int atk = card.currentAtk + extraDamage;

            card.cardEffectControl.UpdateDesAtk(atk);
        }
    }

    public override void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Water)
        {
            Debug.Log("[水滴]水牌伤害增加：" + extraDamage);
            card.currentAtk += extraDamage;
        }
    }

   
}
