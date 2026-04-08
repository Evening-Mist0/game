using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefTower_Di : BaseDefTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;

    
    [Tooltip("领域持续回合")]
    public int roundTime;
    //领域还剩多少回合
    private int nowRoundTime;

    //每次赋予禁锢效果的回合数
    private int effectCount = 1;


    protected override void InitValue()
    {
        base.InitValue();
        nowRoundTime = roundTime;
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
                monster.GetImprison(effectCount);
            }
        }
        myCell.nowStateType = CellStateType.GhostOccupied;
        nowRoundTime--;
        if (nowRoundTime <= 0)
        {
            DestroyMe();
        }
    }

    public override void OnHurt(OnDefTowerHurtByMonsterEvents evt)
    {
     
    }
}
