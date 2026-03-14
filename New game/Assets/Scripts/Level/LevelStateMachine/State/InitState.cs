using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitState : BaseLevelState
{  

    public override E_LevelState myStateType => E_LevelState.Init;


    public override void EnterState()
    {
        Debug.Log("进入Init状态,初始化地图");
        GridMgr.Instance.CreatGridMap();
    }

    public override void ExitState()
    {
        Debug.Log("退出Init状态");

    }

    public override void OnState()
    {
        Debug.Log("处于Init状态");

    }

    public override void Init()
    {
        base.Init();
    }
}
