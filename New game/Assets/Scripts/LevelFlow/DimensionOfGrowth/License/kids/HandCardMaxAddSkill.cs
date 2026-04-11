using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandCardMaxAddSkill : BasePlayerSkill
{
    // 额外手牌上限
    private int capicity = 3;

    public override E_LevelUpOptionType SkillType => E_LevelUpOptionType.HandCardMaxAdd;

    public override void OnGetSkill()
    {
        Dealer.Instance.capicity += capicity;
    }

    public override void OnSetClear()
    {
        Dealer.Instance.capicity -= capicity;
    }
}
