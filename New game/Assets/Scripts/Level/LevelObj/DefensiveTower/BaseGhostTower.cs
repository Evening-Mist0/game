using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGhostTower : BaseDefTower
{
    /// <summary>
    /// 防御塔持续回合
    /// </summary>

    [HideInInspector]
    public int existRound;

    /// <summary>
    /// 防御塔持有技能
    /// </summary>
    [HideInInspector]
    public List<GhostTowerSkillPair> skills;

    protected override void InitValue()
    {
        base.InitValue();
        GhostTowerScriptableData newData = data as GhostTowerScriptableData;
        if (newData != null)
        {
            skills = newData.skills;
            existRound = newData.existRound;
            Debug.Log("更新建筑物" + myTowerName.ToString() + "持续回合为" + existRound);
        }
        else
            Debug.Log("防御塔配置里氏替换失败,请检查Inspector窗口类型是否挂载正确");
    }

}
