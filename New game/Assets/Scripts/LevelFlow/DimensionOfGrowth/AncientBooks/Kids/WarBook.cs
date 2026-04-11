using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarBook : BaseBook
{
    private int extraAtk = 1;
    public override E_BookType BookType => E_BookType.Battle_PoWang;

    public override void OnPlay(BaseCard card)
    {
        Debug.Log("[战典籍]增加卡牌{}攻击力" + extraAtk);
        card.currentAtk += extraAtk;
    }
}
