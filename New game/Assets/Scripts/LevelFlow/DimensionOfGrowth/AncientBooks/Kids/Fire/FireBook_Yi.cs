using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBook_Yi : BaseEffectBook
{
    private string cardID_combine_fire_yi = "combine_fire_yi";

    public override E_BookType BookType => E_BookType.Fire_Yi;



    public override CardSkillPair extraSkillAddition => new CardSkillPair(E_CardSkill.Burn, 0, 0);


    public override EffectCardScriptable effectCardData => Resources.Load<EffectCardScriptable>("BaseCardScriptableObject/combine_fire_yi");

    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard(card);
        if(card.cardID == cardID_combine_fire_yi)
        {
            Debug.Log("[火典籍]判定到生成的卡牌为燚，进行激活");
                card.isActive = true;
        }
    }
}
