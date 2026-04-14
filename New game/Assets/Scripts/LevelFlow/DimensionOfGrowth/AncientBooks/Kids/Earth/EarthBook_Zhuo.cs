using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBook_Zhuo : BaseBook
{
    public string cardID_combine_earth_zhuo = "combine_earth_zhuo";
    public override E_BookType BookType => E_BookType.Earth_Zhuo;
    public override void OnComposite(BaseCard card)
    {
        if (card.cardID == cardID_combine_earth_zhuo)
        {
            Debug.Log("[土典籍]判定到生成的卡牌为圴，进行激活");
            card.isActive = true;
        }
    }
}
