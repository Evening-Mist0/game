using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBook : BaseBook
{
    public string cardID_combine_water_miao = "combine_water_miao";
    public override E_BookType BookType => E_BookType.Water_BaiChuan;

    public override void OnComposite(BaseCard card)
    {
        if (card.cardID == cardID_combine_water_miao)
        {
            Debug.Log("[水典籍]判定到生成的卡牌为淼，进行激活");
            card.isActive = true;
        }
    }
}
