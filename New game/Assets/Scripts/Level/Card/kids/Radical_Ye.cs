using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radical_Ye : BaseRadicalCard
{
    public override E_RadicalCardType radicalCardType => E_RadicalCardType.Ye;

    public override string MyResName => DataCenter.Instance.resNameData.radical_ye;
}
