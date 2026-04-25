using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBook_Yao : BaseEntityTowerBook
{
    public string cardID_combine_earth_yao = "combine_earth_yao";
    public override E_BookType BookType => E_BookType.Earth_Yao;

    public override EntityTowerScriptableData entityTowerData => Resources.Load<EntityTowerScriptableData>("BaseEntityTowerSO/Level1/Level1_DefTower_Earth_Yao");

    public override BasePlaceCardScriptable placeCardData => Resources.Load<BasePlaceCardScriptable>("BaseCardScriptableObject/combine_earth_yao");



    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard(card);
        if (card.cardID == cardID_combine_earth_yao)
        {
            Debug.Log("[土典籍]判定到生成的卡牌为垚，进行激活");
            card.isActive = true;
        }
    }
}
