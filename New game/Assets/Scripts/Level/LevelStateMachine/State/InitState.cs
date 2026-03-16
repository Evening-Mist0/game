using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitState : BaseLevelState
{
    /// <summary>
    /// 状态运行标识
    /// </summary>
    private bool isIniting = true;

    int count = 0;
    public override E_LevelState myStateType => E_LevelState.Init;


    public override void EnterState()
    {
        if(isIniting)
        {
            Debug.Log("进入Init状态,初始化地图,生成怪物");
            GridMgr.Instance.CreatGridMap();
            if(count < 3)
            MonsterCreater.Instance.CreateMonster(DataCenter.Instance.resNameData.water01_waterWisp, 2);
            count++;
        }
        isIniting = false;
    }

    public override void ExitState()
    {
        Debug.Log("退出Init状态");
        isIniting = true;
    }

    public override void OnState()
    {
        if (!isIniting)
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_CardOperate);
        else
            Debug.Log("处于Init状态");

    }

    public override void Init()
    {
        base.Init();
    }
}
