using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBook : BaseBook
{
    public string cardID_combine_earth_yao = "combine_earth_yao";
    public override E_BookType BookType => E_BookType.Earth_HouTu;

    public override void OnComposite(BaseCard card)
    {
        if (card.cardID == cardID_combine_earth_yao)
        {
            Debug.Log("[土典籍]判定到生成的卡牌为垚，进行激活");
            card.isActive = true;
        }
    }
}
