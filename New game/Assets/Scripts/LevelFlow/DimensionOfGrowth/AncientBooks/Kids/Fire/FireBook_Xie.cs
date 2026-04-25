using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBook_Xie : BaseEffectBook
{
    public override E_BookType BookType => E_BookType.Fire_Xie;

    public override EffectCardScriptable effectCardData => Resources.Load<EffectCardScriptable>("BaseCardScriptableObject/combine_efire_xie");

    public override CardSkillPair extraSkillAddition => new CardSkillPair(E_CardSkill.StimulateBurn, 0, 0);

    private string cardID_combine_fire_xie = "combine_fire_xie";
    
    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard (card);
        if (card.cardID == cardID_combine_fire_xie)
        {
            Debug.Log("[火典籍]判定到生成的卡牌为灺，进行激活");
            card.isActive = true;
        }
    }

}
