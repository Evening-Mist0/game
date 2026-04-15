using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerLevelViewControl : MonoBehaviour
{
    //当前玩家的等级
    public TMP_Text nowLevel;
    //还有多少经验值升级
    public TMP_Text levelCapacity;

    public void UpdateNowLevel()
    {
        int level = GrowthMgr.Instance.growthData.licenseLevel;
        nowLevel.text = "当前等级:" + level.ToString();
    }

    public void UpdateLevelCapacity()
    {
        int exp = GrowthMgr.Instance.growthData.licenseExp % 2;
        levelCapacity.text = "当前经验:" + exp.ToString()+ "/2";
    }
}
