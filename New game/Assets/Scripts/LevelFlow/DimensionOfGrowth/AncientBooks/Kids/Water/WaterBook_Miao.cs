using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterBook_Miao : BaseEffectBook
{
    public string cardID_combine_water_miao = "combine_water_miao";
    public override E_BookType BookType => E_BookType.Water_Miao;

    public override EffectCardScriptable effectCardData => Resources.Load<EffectCardScriptable>("BaseCardScriptableObject/combine_water_miao");

    public override CardSkillPair extraSkillAddition => new CardSkillPair(E_CardSkill.Repel, 0, 0);

    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard (card);
        if (card.cardID == cardID_combine_water_miao)
        {
            Debug.Log("[水典籍]判定到生成的卡牌为淼，进行激活");
            card.isActive = true;
        }
    }
}
