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
    public static LevelStepMgr Instance
    {
        get
        {
            if (instance == null)
            {
                // 尝试在场景中查找
                instance = FindObjectOfType<LevelStepMgr>();
                //if (instance == null)
                //{
                //    Debug.LogError("场景中没有找到 LevelStepMgr 实例，请确保在启动场景中挂载该脚本");
                //}
            }
            return instance;
        }
    }

    public LevelStateMachine machine = null;

    ///// <summary>
    ///// 当前关卡的关卡类型
    ///// </summary>
    //public E_LevelType nowLevelType;


    /// <summary>
    /// 本次怪物生成信息
    /// </summary>
    public BattleInfo currentBattleInfo;
   
    /// <summary>
    /// 当前的生成波次
    /// </summary>
    public int currentWave;

    /// <summary>
    /// 当前怪物还存在的数量
    /// </summary>
    public int monsterAliveCount;

    /// <summary>
    /// 当前精英怪存在的数量
    /// </summary>
    public int currentEliteCount;

    /// <summary>
    /// 当前Boss怪存在的数量
    /// </summary>
    public int currentBossCount;

    private void Awake()
    {
        Debug.Log("LevelStepMgr执行一次Awake");
        // 单例冲突处理
        if (instance != null && instance != this)
        {
            Debug.LogWarning("发现重复的 LevelStepMgr，销毁多余实例", gameObject);
            Destroy(gameObject);
            return;
        }
        instance = this;
        // 如果需要跨场景保留，手动添加
        DontDestroyOnLoad(gameObject);

        // 初始化其他组件（如状态机）
        if (machine == null)
            machine = GetComponentInChildren<LevelStateMachine>();
        if (machine == null)
            Debug.LogError("请为 LevelStepMgr 添加子对象并挂载 LevelStateMachine 脚本");
    }

    private void Start()
    {
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.Idle);
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
        {
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.LevelWin);
        }
    }

    /// <summary>
    /// 进入怪物初始化创建状态
    /// </summary>
    public void EnterCreatMonsterState()
    {
        machine.ChangeState(E_LevelState.MonsterTurn_CreatMonster);
    }

    /// <summary>
    /// 重置关卡数据,恢复到初始状态(当局游戏结束调用)
    /// </summary>
    public void ResetMe()
    {
        //重置关卡数据
        currentBattleInfo = null;
        currentWave = 0;
        monsterAliveCount = 0;
        currentBossCount = 0;
        currentEliteCount = 0;
    }

    /// <summary>
    /// 读取战斗节点信息，在点击对应节点后调用
    /// </summary>

    public void UpdateBattleInfo(BattleInfo info)
    {
        Debug.Log("关卡信息初始化完成,进入初始化状态");
        currentBattleInfo = info;
        monsterAliveCount = info.monsterCounts;
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.Init);
    }

    private void OnDestroy()
    {
        // 如果销毁的是当前实例，清空静态引用
        if (instance == this)
            instance = null;
    }


}
