using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthBook_Zhuo : BaseEffectBook
{
    public string cardID_combine_earth_zhuo = "combine_earth_zhuo";
    public override E_BookType BookType => E_BookType.Earth_Zhuo;

    public override CardSkillPair extraSkillAddition => new CardSkillPair(E_CardSkill.Burn, 0, 0);

    public override EffectCardScriptable effectCardData => Resources.Load<EffectCardScriptable>("BaseCardScriptableObject/combine_earth_zhuo");



    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard(card);
        if (card.cardID == cardID_combine_earth_zhuo)
        {
            Debug.Log("[土典籍]判定到生成的卡牌为圴，进行激活");
            card.isActive = true;
        }
    }
}
