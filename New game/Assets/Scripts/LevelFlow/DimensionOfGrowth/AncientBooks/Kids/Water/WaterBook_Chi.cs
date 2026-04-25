using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBook_Chi : BaseGhostTowerBook
{
    public override E_BookType BookType => E_BookType.Water_Chi;

    public override GhostTowerScriptableData ghostTowerData => Resources.Load<GhostTowerScriptableData>("BaseGhostTowerSO/Level1/Level1_DefTower_Water_Chi");

    public override BasePlaceCardScriptable placeCardData => Resources.Load<BasePlaceCardScriptable>("BaseCardScriptableObject/combine_water_chi");


    public string cardID_combine_water_chi = "combine_water_chi";

    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard(card);
        if (card.cardID == cardID_combine_water_chi)
        {
            Debug.Log("[水典籍]判定到生成的卡牌为池，进行激活");
            card.isActive = true;
        }
    }
}
