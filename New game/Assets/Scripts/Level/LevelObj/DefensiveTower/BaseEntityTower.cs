using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EntityTowerData", menuName = "Game/DefTower/EntityTowerData")]

public abstract class BaseEntityTower : BaseDefTower
{
    /// <summary>
    /// 防御塔持有技能
    /// </summary>
    //[HideInInspector]
    public List<EntityTowerSkillPair> skills;

    protected override void InitValue()
    {
        base.InitValue();
        EntityTowerScriptableData newData = data as EntityTowerScriptableData;
        if (newData != null)
            skills = newData.skills;
        else
            Debug.Log("防御塔配置里氏替换失败,请检查Inspector窗口类型是否挂载正确");
    }

  
}
