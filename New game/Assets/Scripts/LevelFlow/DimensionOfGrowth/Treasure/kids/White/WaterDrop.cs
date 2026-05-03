using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDrop : BaseTreasure
{
    //水基础牌附加的伤害值
    private int extraDamage = 1;
    public override int round => -1;


    public override E_TreasureType type => E_TreasureType.WaterDrop;


    public override void OnCreatNewCard(BaseCard card)
    {
        base.OnCreatNewCard(card);
        if (card.elementType == E_Element.Water)
        {
            int atk = card.currentAtk + extraDamage;
            Debug.Log("[水滴]更新{card.cardID}伤害为：" + atk);
            MonoMgr.Instance.StartCoroutine(UpdateDesAtk(card, atk));
        }
    }

    private IEnumerator UpdateDesAtk(BaseCard card, int atk)
    {
        yield return null;
        card.cardEffectControl.UpdateDesAtk(atk);
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
