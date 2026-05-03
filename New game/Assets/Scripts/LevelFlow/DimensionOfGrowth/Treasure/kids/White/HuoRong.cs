using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuoRong : BaseTreasure
{
    //»ğ»ù´¡ÅÆ¸½¼ÓµÄÉËº¦Öµ
    private int extraDamage = 1;
    public override int round => -1;


    public override E_TreasureType type => E_TreasureType.HuoRong;

    public override void OnCreatNewCard(BaseCard card)
    {
        base.OnCreatNewCard(card);
        if (card.elementType == E_Element.Fire)
        {
            card.desViewAtk = card.currentAtk + extraDamage;
            Debug.Log($"[»ğÈŞ]¸üĞÂ{card.cardID}ÃèÊöÉËº¦Îª£º" + card.desViewAtk);
            MonoMgr.Instance.StartCoroutine(UpdateDesAtk(card, card.desViewAtk));
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
            Debug.Log("[»ğÈŞ]»ğÅÆÉËº¦Ôö¼Ó£º" + extraDamage);
            card.currentAtk += extraDamage;
        }
    }

    public override void OnPrevSlected(BaseCardScriptableData data)
    {
        base.OnPrevSlected(data);
        if (data.elementType == E_Element.Fire)
        {
            data.baseAtk += extraDamage;
        }
    }

 
}
