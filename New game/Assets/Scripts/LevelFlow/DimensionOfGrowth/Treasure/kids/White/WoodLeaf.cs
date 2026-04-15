using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodLeaf : BaseTreasure  
{
    //木基础牌附加的伤害值
    private int extraDamage = 1;
   

    public override void OnDrawCard(BaseCard card)
    {
        if (card.elementType == E_Element.Wood  && card.cardType == E_CardType.Base)
        {
            Debug.Log("[木叶]更新基础土牌伤害：" + extraDamage);
            int atk = card.currentAtk + extraDamage;
            card.cardEffectControl.UpdateDesAtk(atk);
        }
    }

    public override void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Wood && card.cardType == E_CardType.Base)
        {
            Debug.Log("[木叶]基础木牌伤害增加：" + extraDamage);
            card.currentAtk += extraDamage;
        }
    }

  
}
