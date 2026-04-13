using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBook_Xie : BaseBook
{
    public override E_BookType BookType => E_BookType.Fire_Xie;

    private string cardID_combine_fire_xie = "combine_fire_xie";
    
    public override void OnComposite(BaseCard card)
    {
        if (card.cardID == cardID_combine_fire_xie)
        {
            Debug.Log("[火典籍]判定到生成的卡牌为灺，进行激活");
            card.isActive = true;
        }
    }

}
