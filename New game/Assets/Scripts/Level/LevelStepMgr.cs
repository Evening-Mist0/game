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

    [Header("基础配置")]
    [Tooltip("该关卡总波次")]
    public int waveCount;

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

    private void InitValue()
    {

    }

    /// <summary>
    /// 确定LevelStepMgr的状态机处于哪个状态，如果与参数匹配正确，返回true
    /// </summary>
    /// <param name="state">想要确认的状态</param>
    public bool ComfirNowStateType(E_LevelState stateType)
    {
        if (machine.nowState.myStateType == stateType)
            return true;
        return false;
    }

    /// <summary>
    /// 返回当前的状态类型（不是枚举）
    /// </summary>
    /// <returns></returns>
    public BaseLevelState ReturnNowState()
    {
        if(machine.nowState == null)
            return null;
        return machine.nowState;
    }

    
}
