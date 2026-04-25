using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefTower_Earth_Di : BaseGhostTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;


    private GhostTowerSkillPair imprisonSkill;

    protected override void InitValue()
    {
        base.InitValue();
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i].towerSkill == E_GhostTowerSkill.Imprison)
                imprisonSkill = skills[i];
        }
    }


    protected override void Awake()
    {
        base.Awake();
        TypeSafeEventCenter.Instance.Register<OnExitCardOperateStateEvent>(this, HandleExitCardOperateStateEvent);
    }

    /// <summary>
    /// 寻找自己处于的位置又没有怪物，如果有怪物就对怪物发起攻击
    /// </summary>
    private void HandleExitCardOperateStateEvent(OnExitCardOperateStateEvent evt)
    {
        Debug.Log("地检测事件发生");
        if ((myCell.nowStateType == CellStateType.MonsterOccupied) || (myCell.nowStateType == CellStateType.GhostOccupied))
        {
            Debug.Log("[地]检查测到当前格子被怪物占据，对怪物发起攻击");
            BaseMonsterCore monster = myCell.nowObj as BaseMonsterCore;
            if (monster != null)
            {
                monster.GetImprison(imprisonSkill.roundValue);
            }
        }
        myCell.nowStateType = CellStateType.GhostOccupied;
        existRound--;
        if (existRound <= 0)
        {
            DestroyMe();
        }
    }
}
