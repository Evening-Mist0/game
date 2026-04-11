using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitArmorSkill : BasePlayerSkill
{
    // 每回合更新的初始护甲值
    private int armor = 2;

    public override E_LevelUpOptionType SkillType => E_LevelUpOptionType.InitArmor;

    public override void OnGetSkill()
    {
        GrowthMgr.Instance.AddArmor(armor);
        GrowthMgr.Instance.growthData.playerExtraDef += armor;
    }

    public override void OnSetClear()
    {
        GrowthMgr.Instance.growthData.playerExtraDef -= armor;
    }
}
