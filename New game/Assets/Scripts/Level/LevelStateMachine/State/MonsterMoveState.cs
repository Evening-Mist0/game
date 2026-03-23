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
            //清理需要在移动后清理的负面状态
            isAllowedMonsterMove = false;
        }
        
    }

    public override void ExitState()
    {
        Debug.Log("退出怪物移动阶段");

        //重置玩家受伤动画是否可以播放
        GamePlayer.Instance.effectControl.ResetPlayHurt();

        isAllowedMonsterMove = true;

    }

    public override void OnState()
    {
        //状态机切换交给MonsterMoveMgr内部管理
    }
}
