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
       
            Debug.Log("进入MonsterMoveState");
            MonsterMoveMgr.Instance.StartBatchMove();
            //清理需要在移动后清理的负面状态
            isAllowedMonsterMove = false;     
    }

    public override void ExitState()
    {
        Debug.Log("退出MonsterMoveState");
        CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();
        if (panel != null)
        {
            panel.EnableOverMyTurnButton();
        }
        TypeSafeEventCenter.Instance.Trigger<OnExitMonsterMoveStateEvent>(new OnExitMonsterMoveStateEvent());
    }

    public override void OnState()
    {
        //状态机切换交给MonsterMoveMgr内部管理
    }
}
