using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radical_Ke : BaseRadicalCard
{
    public override E_RadicalCardType radicalCardType => E_RadicalCardType.Ke;

    public override string MyResName => DataCenter.Instance.cardResNameData.radical_ke;
}
