using System;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// 战斗信息，用于启动战斗时传递必要数据
/// </summary>
public class BattleInfo
{
    public string nodeId;           // 关联的节点ID
    public E_TowerNodeType battleType; // 战斗类型：普通/精英/BOSS
    //public List<EnemyConfig> enemies; // 敌人配置列表（由关卡数据提供）
    // 可根据需要扩展其他字段，如地图配置、环境效果等
}

/// <summary>
/// 战斗管理器，负责协调爬塔模块与战斗模块
/// </summary>
public class BattleMgr : BaseMgr<BattleMgr>
{
    private BattleMgr() { }

    // 当前正在进行的战斗信息（可选）
    private BattleInfo currentBattleInfo;

    // 是否正在战斗中（防止重复进入）
    private bool isInBattle = false;

    #region 启动战斗
    /// <summary>
    /// 启动一场战斗（由爬塔节点调用）
    /// </summary>
    /// <param name="battleInfo">战斗信息，必须包含 nodeId 和 battleType</param>
    public void StartBattle(BattleInfo battleInfo)
    {
        if (isInBattle)
        {
            Debug.LogWarning("已有战斗进行中，无法开始新战斗");
            return;
        }

        if (battleInfo == null || string.IsNullOrEmpty(battleInfo.nodeId))
        {
            Debug.LogError("战斗信息无效，缺少 nodeId");
            return;
        }

        currentBattleInfo = battleInfo;
        isInBattle = true;

        // 根据战斗类型触发不同的事件（供战斗模块监听）
        switch (battleInfo.battleType)
        {
            case E_TowerNodeType.NormalBattle:
                EventCenter.Instance.EventTrigger(E_EventType.Battle_LoadNormalBattle, battleInfo.nodeId);
                break;
            case E_TowerNodeType.EliteBattle:
                EventCenter.Instance.EventTrigger(E_EventType.Battle_LoadEliteBattle, battleInfo.nodeId);
                break;
            case E_TowerNodeType.BossBattle:
                EventCenter.Instance.EventTrigger(E_EventType.Battle_LoadBossBattle, battleInfo.nodeId);
                break;
            default:
                Debug.LogError($"不支持的战斗类型：{battleInfo.battleType}");
                break;
        }
        //切换战斗场景
        SceneMgr.Instance.LoadSceneAsync("LevelScene",() => {
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.Init);
        });

    }
    #endregion

    #region 战斗结果回调（由战斗模块调用）
    /// <summary>
    /// 战斗胜利回调，由战斗模块在战斗胜利时调用
    /// </summary>
    /// <param name="nodeId">完成的节点ID</param>
    /// <param name="rewardExp">额外经验（可选，默认0）</param>
    public void OnBattleWin(string nodeId, int rewardExp = 0)
    {
        if (!isInBattle)
        {
            Debug.LogWarning("没有进行中的战斗，无法处理胜利");
            return;
        }

        if (currentBattleInfo == null || currentBattleInfo.nodeId != nodeId)
        {
            Debug.LogError($"节点ID不匹配：当前战斗节点 {currentBattleInfo?.nodeId}，胜利节点 {nodeId}");
            return;
        }

        // 根据战斗类型触发对应胜利事件，供爬塔节点监听
        switch (currentBattleInfo.battleType)
        {
            case E_TowerNodeType.NormalBattle:
                EventCenter.Instance.EventTrigger(E_EventType.Battle_NormalBattleWin, nodeId);
                break;
            case E_TowerNodeType.EliteBattle:
                EventCenter.Instance.EventTrigger(E_EventType.Battle_EliteBattleWin, nodeId);
                break;
            case E_TowerNodeType.BossBattle:
                EventCenter.Instance.EventTrigger(E_EventType.Battle_BossBattleWin, nodeId);
                break;
        }

        // 清理战斗状态
        CleanupBattle();
    }

    /// <summary>
    /// 战斗失败回调，由战斗模块在战斗失败时调用
    /// </summary>
    /// <param name="nodeId">失败的节点ID</param>
    public void OnBattleLose(string nodeId)
    {
        if (!isInBattle)
        {
            Debug.LogWarning("没有进行中的战斗，无法处理失败");
            return;
        }

        if (currentBattleInfo == null || currentBattleInfo.nodeId != nodeId)
        {
            Debug.LogError($"节点ID不匹配：当前战斗节点 {currentBattleInfo?.nodeId}，失败节点 {nodeId}");
            return;
        }

        // 触发战斗失败事件（爬塔模块监听，如显示失败界面）
        EventCenter.Instance.EventTrigger(E_EventType.Battle_BattleLose, nodeId);

        // 清理战斗状态
        CleanupBattle();
    }
    #endregion

    #region 辅助方法
    private void CleanupBattle()
    {
        isInBattle = false;
        currentBattleInfo = null;
    }

    /// <summary>
    /// 获取当前战斗信息（供战斗模块查询）
    /// </summary>
    public BattleInfo GetCurrentBattleInfo()
    {
        return currentBattleInfo;
    }

    /// <summary>
    /// 是否正在战斗中
    /// </summary>
    public bool IsInBattle => isInBattle;
    #endregion

    #region 测试辅助（战斗模块未实现时使用）
    /// <summary>
    /// 模拟战斗胜利（用于测试爬塔流程，直接调用 OnBattleWin）
    /// </summary>
    public void SimulateBattleWin()
    {
        if (currentBattleInfo != null)
            OnBattleWin(currentBattleInfo.nodeId);
        else
            Debug.LogWarning("没有进行中的战斗，无法模拟胜利");
    }

    /// <summary>
    /// 模拟战斗失败
    /// </summary>
    public void SimulateBattleLose()
    {
        if (currentBattleInfo != null)
            OnBattleLose(currentBattleInfo.nodeId);
        else
            Debug.LogWarning("没有进行中的战斗，无法模拟失败");
    }
    #endregion
}