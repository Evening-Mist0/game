using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterMoveState : BaseLevelState
{
   /// <summary>
   /// 临时变量，怪物移动只是进行移动，进入状态就会立马退出
   /// </summary>
    private bool isAllowedMonsterMove = true;
    public override E_LevelState myStateType => E_LevelState.MonsterTurn_Move;

    public override void EnterState()
    {
        if(isAllowedMonsterMove)
        {
            Debug.Log("进入怪物移动阶段");
            MonsterMoveMgr.Instance.StartBatchMove();
            isAllowedMonsterMove = false;
        }
        
    }

    public override void ExitState()
    {
        Debug.Log("退出怪物移动阶段");
        isAllowedMonsterMove = true;

    }

    public override void OnState()
    {
        if (!isAllowedMonsterMove)
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.Init);
    }
}
