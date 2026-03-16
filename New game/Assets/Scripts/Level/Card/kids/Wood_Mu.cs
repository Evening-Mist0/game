using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wood_Mu : BaseCard
{
    public override string MyResName => DataCenter.Instance.resNameData.base_wood_mu;

    public override string MyDefTowerResName => DataCenter.Instance.resNameData.defTower_wood_mu;

    private void Start()
    {
        Debug.Log("[测试打印]获取的木防御塔加载路径" + MyDefTowerResName);
    }


}
