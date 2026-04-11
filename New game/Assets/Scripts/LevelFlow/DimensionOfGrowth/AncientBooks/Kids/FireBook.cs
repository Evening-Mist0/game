using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBook : BaseBook
{
    private string cardID_combine_fire_yi = "combine_fire_yi";

    public override E_BookType BookType => E_BookType.Fire_LiaoYuan;

    public override void OnComposite(BaseCard card)
    {
        if(card.cardID == cardID_combine_fire_yi)
        {
            Debug.Log("[火典籍]判定到生成的卡牌为燚，进行激活");
                card.isActive = true;
        }
    }
}
