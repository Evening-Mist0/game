using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelWinState : BaseLevelState
{
    public override E_LevelState myStateType => E_LevelState.LevelWin;

    public override void EnterState()
    {
        Debug.Log("进入游戏胜利状态");
    }

    public override void ExitState()
    {
        Debug.Log("退出游戏胜利状态");
    }

    public override void OnState()
    {
       
    }
}
