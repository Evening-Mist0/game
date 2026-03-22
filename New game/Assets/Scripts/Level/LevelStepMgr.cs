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


    public int waveMonsterCounts;
    /// <summary>
    /// 本次关卡怪物生成的总数量
    /// </summary>
    [HideInInspector]
    public int monsterCounts;

    /// <summary>
    /// 当前怪物还存在的数量
    /// </summary>
    [HideInInspector]

    private int monsterAliveCount;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (machine == null)
            Debug.LogError("请为LevelStepMgr添加子对象并挂载LevelStateMachine脚本");    
    }

    private void Start()
    {
        machine.ChangeState(E_LevelState.Init);
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



    /// <summary>
    /// 在固定区间随机一个数作为本次关卡的怪物生成数量
    /// </summary>
    /// <returns></returns>
    private int CreatWaveCount()
    {
        return waveMonsterCounts;
    }

    public void UpdatMonsterAliveCount()
    {
        monsterAliveCount--;
        if (monsterAliveCount == 0)
            Debug.Log("[游戏结算]显示胜利面板和结算");
    }

    /// <summary>
    /// 进入初始化状态
    /// </summary>
    public void EnterInitState()
    {
        monsterCounts = CreatWaveCount();
        monsterAliveCount = monsterCounts;
        machine.ChangeState(E_LevelState.MonsterTurn_CreatMonster);
    }

}
