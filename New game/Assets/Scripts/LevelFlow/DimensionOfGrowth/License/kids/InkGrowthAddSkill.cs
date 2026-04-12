using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkGrowthAddSkill : BasePlayerSkill
{
    private int extraInkGrowValue = 5;
    public override E_LevelUpOptionType SkillType => E_LevelUpOptionType.InkGrowthAddSkill;

    public override void OnGetSkill()
    {
        GamePlayer.Instance.inkGrowValue += extraInkGrowValue;
    }

    public override void OnSetClear()
    {
        GamePlayer.Instance.inkGrowValue -= extraInkGrowValue;

    }
}
