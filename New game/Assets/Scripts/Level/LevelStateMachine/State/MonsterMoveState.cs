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
        TypeSafeEventCenter.Instance.Trigger<OnExitMonsterMoveStateEvent>(new OnExitMonsterMoveStateEvent());
        GamePlayer.Instance.ClearDef();


        //重置玩家受伤动画是否可以播放
        //GamePlayer.Instance.effectControl.ResetPlayHurt();
    }

    public override void OnState()
    {
        //状态机切换交给MonsterMoveMgr内部管理
    }
}
