using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBook_Bi : BaseEffectBook
{
    public string cardID_combine_wood_bi = "combine_wood_bi";

    public override E_BookType BookType => E_BookType.Wood_Bi;

    public override EffectCardScriptable effectCardData => Resources.Load<EffectCardScriptable>("BaseCardScriptableObject/combine_wood_bi");

    public override CardSkillPair extraSkillAddition => new CardSkillPair(E_CardSkill.AddMaxHPToDefTower, 0, 0);
    public CardSkillPair extraSkillAddition_AddHealthToDefTower = new CardSkillPair(E_CardSkill.AddHealthToDefTower, 0, 0);

    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard(card);
        if (card.cardID == cardID_combine_wood_bi)
        {
            Debug.Log("[木典籍]判定到生成的卡牌为柀，进行激活");
            card.isActive = true;
        }
    }
}
