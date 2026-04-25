using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBook_Fen : BaseEffectBook
{
    public override E_BookType BookType => E_BookType.Fire_Fen;


    public override EffectCardScriptable effectCardData => Resources.Load<EffectCardScriptable>("BaseCardScriptableObject/combine_fire_fen");



    public override CardSkillPair extraSkillAddition => new CardSkillPair(E_CardSkill.Burn, 0, 0);

    private string cardID_combine_fire_fen = "combine_fire_fen";

    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard(card);
        if (card.cardID == cardID_combine_fire_fen)
        {
            Debug.Log("[火典籍]判定到生成的卡牌为焚，进行激活");
            card.isActive = true;
        }
    }
}