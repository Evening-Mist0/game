using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEnterSettleState : BaseLevelState
{
    /// <summary>
    /// 状态运行标识
    /// </summary>
    private bool isMonsterEnterSettling = true;

    public override E_LevelState myStateType => E_LevelState.MonsterTurn_EnterSettle;

    public override void EnterState()
    {
        UpdateMonstersState();
        isMonsterEnterSettling = false;
    }

    public override void ExitState()
    {
        isMonsterEnterSettling = true;
    }

    public override void OnState()
    {
        if (!isMonsterEnterSettling)
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_Move);
    }

    public void UpdateMonstersState()
    {
       List<BaseMonster> monsterList = MonsterCreater.Instance.GetAllAliveMonsters();
        for(int i = 0; i < monsterList.Count; i++)
        {
            monsterList[i].OnRoundUpdate();
        }
    }
}
