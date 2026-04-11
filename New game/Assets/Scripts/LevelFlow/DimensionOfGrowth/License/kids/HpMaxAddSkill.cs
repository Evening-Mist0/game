using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpMaxAddSkill : BasePlayerSkill
{
    // 更新的最大生命值
    private int extraHpMax = 20;

    public override E_LevelUpOptionType SkillType => E_LevelUpOptionType.HpMaxAdd;

    public override void OnGetSkill()
    {
        GrowthMgr.Instance.AddPlayerMaxHp(extraHpMax);
    }

    public override void OnSetClear()
    {
        GrowthMgr.Instance.AddPlayerMaxHp(-extraHpMax);

    }
}
