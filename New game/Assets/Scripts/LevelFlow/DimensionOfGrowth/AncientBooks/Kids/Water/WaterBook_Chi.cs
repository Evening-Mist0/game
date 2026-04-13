using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBook_Chi : BaseBook
{
    public override E_BookType BookType => E_BookType.Water_Chi;

    public string cardID_combine_water_chi = "combine_water_chi";

    public override void OnComposite(BaseCard card)
    {
        if (card.cardID == cardID_combine_water_chi)
        {
            Debug.Log("[水典籍]判定到生成的卡牌为池，进行激活");
            card.isActive = true;
        }
    }
}
