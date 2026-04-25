using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBook_Yi : BaseEntityTowerBook
{
    public string cardID_combine_wood_yi = "combine_wood_yi";
    public override E_BookType BookType => E_BookType.Wood_Yi;

    public override EntityTowerScriptableData entityTowerData => Resources.Load<EntityTowerScriptableData>("BaseEntityTowerSO/Level1/Level1_DefTower_Wood_Yi");

    public override BasePlaceCardScriptable placeCardData => Resources.Load<BasePlaceCardScriptable>("BaseCardScriptableObject/combine_wood_yi");


    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard(card);
        if (card.cardID == cardID_combine_wood_yi)
        {
            Debug.Log("[木典籍]判定到生成的卡牌为杝，进行激活");
            card.isActive = true;
        }
    }   
}
