using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodLeaf : BaseTreasure  
{
    //Ä¾»ù´¡ÅÆ¸½¼ÓµÄÉËº¦Öµ
    private int extraDamage = 1;
    public override int round => -1;


    public override E_TreasureType type => E_TreasureType.WoodLeaf;


    public override void OnCreatNewCard(BaseCard card)
    {
        base.OnCreatNewCard(card);
        if (card.elementType == E_Element.Wood)
        {
            card.desViewAtk = card.currentAtk + extraDamage;
            Debug.Log("[Ä¾Ò¶]¸üÐÂ{card.cardID}ÉËº¦Îª£º" + card.desViewAtk);
            MonoMgr.Instance.StartCoroutine(UpdateDesAtk(card, card.desViewAtk));
        }
    }

   
    private IEnumerator UpdateDesAtk(BaseCard card, int atk)
    {
        yield return null;
        card.cardEffectControl.UpdateDesAtk(atk);
    }



    public override void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Wood)
        {
            Debug.Log("[Ä¾Ò¶]»ù´¡Ä¾ÅÆÉËº¦Ôö¼Ó£º" + extraDamage);
            card.currentAtk += extraDamage;
        }
    }

    public override void OnPrevSlected(BaseCardScriptableData data)
    {
        base.OnPrevSlected(data);
        if (data.elementType == E_Element.Wood)
        {
            data.baseAtk += extraDamage;
        }
    }


}
