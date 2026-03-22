using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood_Sen : BaseCard
{
    public override string MyResName => DataCenter.Instance.cardResNameData.combine_wood_sen;

    public override string MyDefTowerResName => DataCenter.Instance.defTowerResNameData.DefTower_Wood_Mu;
}
