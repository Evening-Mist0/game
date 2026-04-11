using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBook : BaseBook
{
    private int extraAtk = 2;

    public override E_BookType BookType => E_BookType.Wood_KuRong;

    public override void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Wood)
        {
            Debug.Log($"[木典籍]卡牌{card.cardID}打出攻击力增加" + extraAtk);
            card.currentAtk += extraAtk;
        }         
    }
}
