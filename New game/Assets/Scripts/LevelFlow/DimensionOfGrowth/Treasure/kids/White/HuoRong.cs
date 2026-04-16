using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuoRong : BaseTreasure
{
    //火基础牌附加的伤害值
    private int extraDamage = 1;
   

    public override void OnDrawCard(BaseCard card)
    {
        if (card.elementType == E_Element.Fire)
        {
            Debug.Log("[火绒]更新基础火牌伤害：" + extraDamage);
            int atk = card.currentAtk + extraDamage;

            card.cardEffectControl.UpdateDesAtk(atk);
        }
    }

    public override void OnPlay(BaseCard card)
    {
        if(card.elementType == E_Element.Fire)
        {
            Debug.Log("[火绒]火牌伤害增加：" + extraDamage);
            card.currentAtk += extraDamage;
        }
    }

 
}
