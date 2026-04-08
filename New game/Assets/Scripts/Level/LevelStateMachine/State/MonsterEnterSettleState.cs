using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEnterSettleState : BaseLevelState
{
    /// <summary>
    /// 状态运行标识
    /// </summary>
    private bool isMonsterEnterSettling = true;

    /// <summary>
    /// 对最右边一列格子上，存在的防御塔造成的伤害
    /// </summary>
    public int systemDemage;

    public override E_LevelState myStateType => E_LevelState.MonsterTurn_EnterSettle;

    public override void EnterState()
    {
        Debug.Log("进入monsterEnter状态");
        //清空部首牌
        Dealer.Instance.RemoveAllRadicalCard();
        //发起trigger,让幽灵防御塔攻击怪物
        TypeSafeEventCenter.Instance.Trigger<OnEnterMonsterSettelEvent>(new OnEnterMonsterSettelEvent());
        //更新玩家的状态
        GamePlayer.Instance.OnRound();
        //更新怪物的位置和状态
        UpdateMonstersState();
        isMonsterEnterSettling = false;
        //对最右列的格子进行检查，如果存在建筑物，会扣除伤害
        List<Cell> tempCells = GridMgr.Instance.GetColumnCells(GridMgr.Instance.gridWideCount - 1);
        Debug.Log($"[清理建筑物]清理的建筑物的列数{GridMgr.Instance.gridWideCount - 1}");
        for(int i = 0; i < tempCells.Count; i++)
        {
            if (tempCells[i].nowStateType == CellStateType.EntityOccupied)
            {
                BaseDefTower tower = tempCells[i].nowObj as BaseDefTower;
                if (tower != null)
                {
                    tower.HurtWithSystem(systemDemage);
                }
            }
        }
    }

    public override void ExitState()
    {
        isMonsterEnterSettling = true;
        Debug.Log("退出monsterEnter状态");

    }

    public override void OnState()
    {
        if (!isMonsterEnterSettling)
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_Move);
    }

    public void UpdateMonstersState()
    {
       List<BaseMonsterCore> monsterList = MonsterCreater.Instance.GetAllAliveMonsters();
        for(int i = 0; i < monsterList.Count; i++)
        {
            monsterList[i].OnRoundUpdate();
        }
    }
}
