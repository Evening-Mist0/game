using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood_MuWater : BaseEffectCard
{

    [Tooltip("治愈回合持续数")]
    public int healLastCount;
    [Tooltip("一回合治愈量")]
    public int healValue;

    public override void Effect_Heal(BaseMonsterCore monster, Cell coreCell)
    {
        base.Effect_Heal(monster, coreCell);
        GamePlayer.Instance.GetHeal(healValue, healLastCount);
    }
}
