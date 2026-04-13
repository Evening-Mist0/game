using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBook_Ke : BaseBook
{
    public string cardID_combine_wood_ke = "combine_wood_ke";
    public override E_BookType BookType => E_BookType.Wood_Ke;
     public override void OnComposite(BaseCard card)
    {
        if (card.cardID == cardID_combine_wood_ke)
        {
            Debug.Log("[木典籍]判定到生成的卡牌为柯，进行激活");
            card.isActive = true;
        }
    }
}
