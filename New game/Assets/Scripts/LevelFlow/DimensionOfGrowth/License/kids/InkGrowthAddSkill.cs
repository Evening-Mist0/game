using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkGrowthAddSkill : BasePlayerSkill
{
    private int extraInkGrowValue = 5;
    public override E_LevelUpOptionType SkillType => E_LevelUpOptionType.InkGrowthAddSkill;

    public override void OnGetSkill()
    {
        Debug.Log("每回合笔墨获得增加");
        GamePlayer.Instance.inkGrowValue += extraInkGrowValue;
    }

    public override void OnSetClear()
    {
        Debug.Log("清理笔墨技能");

        GamePlayer.Instance.inkGrowValue -= extraInkGrowValue;

    }
}
