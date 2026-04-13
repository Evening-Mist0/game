using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBook_Lin : BaseBook
{
    public override E_BookType BookType => E_BookType.Water_Lin;

    public string cardID_combine_water_lin = "combine_water_lin";
    public override void OnComposite(BaseCard card)
    {
        if (card.cardID == cardID_combine_water_lin )
        {
            Debug.Log("[水典籍]判定到生成的卡牌为淋，进行激活");
            card.isActive = true;
        }
    }



}
