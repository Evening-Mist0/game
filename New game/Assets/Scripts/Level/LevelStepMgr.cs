using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 关卡管理器：管理关卡内的游戏流程
/// </summary>

public class LevelStepMgr : MonoBehaviour 
{
    private static LevelStepMgr instance;
    public static LevelStepMgr Instance => instance;

    public LevelStateMachine machine = null;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (machine == null)
            Debug.LogError("请为LevelStepMgr添加子对象并挂载LevelStateMachine脚本");
    }

    private void Start()
    {
        machine.ChangeState(E_LevelState.PlayerTurn_CardOperate);
        Debug.Log("切换状态" + machine.NowStateType);
    }
}
