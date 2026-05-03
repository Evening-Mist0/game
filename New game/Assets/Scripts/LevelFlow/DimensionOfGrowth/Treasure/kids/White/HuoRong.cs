using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuoRong : BaseTreasure
{
    //火基础牌附加的伤害值
    private int extraDamage = 1;
    public override int round => -1;


    public override E_TreasureType type => E_TreasureType.HuoRong;

    public override void OnCreatNewCard(BaseCard card)
    {
        base.OnCreatNewCard(card);
        if (card.elementType == E_Element.Fire)
        {
            int atk = card.currentAtk + extraDamage;
            Debug.Log($"[火绒]更新{card.cardID}伤害为：" + atk);
            MonoMgr.Instance.StartCoroutine(UpdateDesAtk(card,atk));
        }
    }

    private IEnumerator UpdateDesAtk(BaseCard card,int atk)
    {
        yield return null;
        card.cardEffectControl.UpdateDesAtk(atk);
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
