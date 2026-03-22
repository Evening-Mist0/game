using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth_Di : BaseCard
{
    public override string MyResName => DataCenter.Instance.cardResNameData.combine_earth_di;

    [Tooltip("玩家可获得的护甲值")]
    public int defValue;

    public override void Effect_GetDef(BaseMonsterCore monster, Cell coreCell)
    {
        base.Effect_GetDef(monster, coreCell);
        GamePlayer.Instance.GetDef(defValue);
    }
}
