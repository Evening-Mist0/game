using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_LevelType
{
    /// <summary>
    /// 普通关
    /// </summary>
    Normal,
    /// <summary>
    /// 精英关
    /// </summary>
    Elite,
    /// <summary>
    /// boss关
    /// </summary>
    Boss,
}
/// <summary>
/// 关卡管理器：管理关卡内的游戏流程
/// </summary>

public class LevelStepMgr : MonoBehaviour 
{
    private static LevelStepMgr instance;
    public static LevelStepMgr Instance => instance;

    public LevelStateMachine machine = null;



    /// <summary>
    /// 当前关卡的关卡类型
    /// </summary>
    public E_LevelType nowLevelType;

    /// <summary>
    /// 本次关卡怪物生成的总数量
    /// </summary>
    public int monsterCounts = 2;
    /// <summary>
    /// 当前的生成波次
    /// </summary>
    public int currentWave;
    //到第几波开始刷精英怪
    public int eliteMonsterAppearWaveCount;
    //出现精英怪的初始概率
    public int eliteMonsterAppearProbability;
    ///出现精英怪每回合增长的概率（从下回合开始，100%则满）
    public int eliteAppearGrowthProbability;
    //到第几波出现boss(直接为100%刷新)
    public int bossMonsterAppearWaveCount;
    //当前精英怪的数量
    public int currentEliteCount;
    //精英怪的最多存在数量
    public int maxEliteCount;
    //当前Boss的数量
    public int currentBossCount;
    //Boss的最多存在数量
    public int maxBossCount;

    /// <summary>
    /// 当前怪物还存在的数量
    /// </summary>
    public int monsterAliveCount;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        if (machine == null)
            Debug.LogError("请为LevelStepMgr添加子对象并挂载LevelStateMachine脚本");    
    }

    private void Start()
    {
        //machine.ChangeState(E_LevelState.Init);
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



    public void UpdatMonsterAliveCount()
    {
        monsterAliveCount--;
        if (monsterAliveCount == 0)
            LevelStepMgr.instance.machine.ChangeState(E_LevelState.LevelWin);
    }

    /// <summary>
    /// 进入怪物初始化创建状态
    /// </summary>
    public void EnterCreatMonsterState()
    {
        monsterAliveCount = monsterCounts;
        machine.ChangeState(E_LevelState.MonsterTurn_CreatMonster);
    }

}
