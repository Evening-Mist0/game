using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBook_Bi : BaseBook
{
    public string cardID_combine_wood_bi = "combine_wood_bi";

    public override E_BookType BookType => E_BookType.Wood_Bi;
    public override void OnComposite(BaseCard card)
    {
        if (card.cardID == cardID_combine_wood_bi)
        {
            Debug.Log("[木典籍]判定到生成的卡牌为柀，进行激活");
            card.isActive = true;
        }
    }
}
