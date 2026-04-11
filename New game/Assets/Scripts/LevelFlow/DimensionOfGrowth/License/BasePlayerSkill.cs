using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerSkill : BaseGrowthObj
{
    public abstract E_LevelUpOptionType SkillType { get; }
    public abstract void OnGetSkill();

    public abstract void OnSetClear();

}
