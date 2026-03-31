using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 关卡流程管理器（优化版）
/// 核心流程：单节点完成→锁整层→点击启程→解锁下一层
/// </summary>
public class LevelFlowMgr : BaseMgr<LevelFlowMgr>
{
    #region 核心数据结构
    /// <summary> 节点核心数据 </summary>
    public class NodeData
    {
        public string nodeId;          // 节点唯一ID
        public E_TowerNodeType type;   // 节点类型
        public int layerIndex;         // 所属楼层
        public E_NodeState state;      // 节点状态
    }
    #endregion

    #region 配置与运行时数据
    public int totalLayerCount = 8;    // 总楼层数
    public int startLayerIndex = 1;    // 起始楼层
    public string startNodeId = "Layer1_Start"; // 起始节点固定ID

    // 运行时核心数据
    private Dictionary<string, NodeData> _allNodeDic = new Dictionary<string, NodeData>();
    private Dictionary<int, List<string>> _layerNodeDic = new Dictionary<int, List<string>>();
    public int currentActiveLayerIndex { get; private set; }   // 当前已解锁的可操作楼层（玩家当前所在楼层）
    public int currentCompletedLayerIndex { get; private set; } // 当前已完成的楼层（上一楼）
    public bool isDepartBtnInteractable { get; private set; }   // 启程按钮是否可点击
    private string _currentCompletedNodeId;                     // 当前层已完成的节点ID
    private bool _isGameEnd = false;                            // 游戏是否结束（通关/失败）

    // 标记是否已初始化（避免重复初始化）
    private bool _isInitialized = false;
    #endregion

    private LevelFlowMgr() { }

    /// <summary>
    /// 初始化楼层容器（确保所有楼层都有列表）
    /// </summary>
    private void InitLayerContainers()
    {
        _layerNodeDic.Clear();
        for (int i = 1; i <= totalLayerCount; i++)
        {
            if (!_layerNodeDic.ContainsKey(i))
            {
                _layerNodeDic.Add(i, new List<string>());
                Debug.Log($"【LevelFlowMgr】楼层{i}容器创建完成");
            }
        }
    }

    #region 游戏生命周期核心入口
    /// <summary>
    /// 初始化新游戏（游戏开始时调用）
    /// </summary>
    public void InitNewGame()
    {
        // 避免重复初始化（防止多次调用导致状态错乱）
        if (_isInitialized)
        {
            Debug.LogWarning("游戏已经初始化，跳过重复初始化");
            return;
        }

        // 1. 初始化楼层容器
        InitLayerContainers();

        // 2. 重置游戏状态（不包含节点数据，节点数据由 TowerPanel 生成时填充）
        ResetGameState();

        // 3. 触发节点生成事件（TowerPanel 会监听并生成节点，同时调用 RegisterNode）
        EventCenter.Instance.EventTrigger(E_EventType.Tower_Bron);

        // 4. 初始化成长数据
        GrowthMgr.Instance.InitNewGameData();

        // 5. 通知 UI 初始化完成，所有节点刷新状态
        EventCenter.Instance.EventTrigger(E_EventType.Tower_InitComplete);

        _isInitialized = true;
        Debug.Log("【LevelFlowMgr】新游戏初始化完成");
    }

    /// <summary>
    /// 重置游戏状态（不清除节点数据，只重置流程状态）
    /// </summary>
    private void ResetGameState()
    {
        _isGameEnd = false;
        _currentCompletedNodeId = string.Empty;
        currentActiveLayerIndex = startLayerIndex;
        currentCompletedLayerIndex = 0;
        isDepartBtnInteractable = false;

        // 注意：不清空 _allNodeDic 和 _layerNodeDic，它们由节点生成时填充
        // 但需要清空旧的节点数据（如果存在），由 TowerPanel 在生成节点前调用 ClearTowerPanel
        // 这里只重置流程变量

        // 监听玩家血量变化（失败判定），注意避免重复添加
        EventCenter.Instance.RemoveEventListener<(int, int)>(E_EventType.Growth_PlayerHpChanged, OnPlayerHpChanged);
        EventCenter.Instance.AddEventListener<(int, int)>(E_EventType.Growth_PlayerHpChanged, OnPlayerHpChanged);
    }

    /// <summary>
    /// 清理所有数据（用于游戏结束后重新开始）
    /// </summary>
    public void ClearAllData()
    {
        _isGameEnd = false;
        _currentCompletedNodeId = string.Empty;
        currentActiveLayerIndex = startLayerIndex;
        currentCompletedLayerIndex = 0;
        isDepartBtnInteractable = false;
        _allNodeDic.Clear();
        foreach (var kv in _layerNodeDic) kv.Value.Clear();

        // 移除血量监听，避免残留
        EventCenter.Instance.RemoveEventListener<(int, int)>(E_EventType.Growth_PlayerHpChanged, OnPlayerHpChanged);

        _isInitialized = false;
        Debug.Log("【LevelFlowMgr】所有数据已清理");
    }
    #endregion

    #region 节点注册（TowerPanel 生成节点时调用）
    /// <summary>
    /// 注册节点到流程管理器
    /// </summary>
    public void RegisterNode(string nodeId, E_TowerNodeType nodeType, int layerIndex)
    {
        if (_allNodeDic.ContainsKey(nodeId))
        {
            Debug.LogWarning($"节点 {nodeId} 已存在，跳过注册");
            return;
        }

        // 创建节点数据，默认锁定状态
        NodeData nodeData = new NodeData
        {
            nodeId = nodeId,
            type = nodeType,
            layerIndex = layerIndex,
            state = E_NodeState.Locked
        };

        // 加入字典
        _allNodeDic.Add(nodeId, nodeData);
        if (!_layerNodeDic.ContainsKey(layerIndex))
        {
            _layerNodeDic.Add(layerIndex, new List<string>());
        }
        _layerNodeDic[layerIndex].Add(nodeId);

        // 起始节点特殊处理：初始解锁
        if (nodeId == startNodeId)
        {
            Debug.Log($"起始节点 {nodeId} 已解锁");
            UpdateNodeState(nodeId, E_NodeState.Unlocked);
        }
    }
    #endregion

    #region 节点状态管理
    public E_NodeState GetNodeState(string nodeId)
    {
        if (_allNodeDic.TryGetValue(nodeId, out var nodeData))
        {
            return nodeData.state;
        }
        return E_NodeState.Locked;
    }

    public void UpdateNodeState(string nodeId, E_NodeState newState)
    {
        if (!_allNodeDic.TryGetValue(nodeId, out var nodeData))
        {
            Debug.LogError($"节点 {nodeId} 不存在，无法更新状态");
            return;
        }

        // 状态转换合法性校验（可选）
        if (!IsValidStateTransition(nodeData.state, newState))
        {
            Debug.LogWarning($"节点 {nodeId} 状态转换非法：{nodeData.state} -> {newState}");
            return;
        }

        nodeData.state = newState;
        // 触发全局状态变更事件，所有 BaseNodeItem 自动监听刷新
        EventCenter.Instance.EventTrigger(E_EventType.Tower_NodeStateChanged, nodeId);
    }

    /// <summary>
    /// 检查状态转换是否合法（可根据需求扩展）
    /// </summary>
    private bool IsValidStateTransition(E_NodeState from, E_NodeState to)
    {
        // 允许从任何状态转换到 Completed（完成节点）
        if (to == E_NodeState.Completed) return true;
        // 从 Locked 只能转换到 Unlocked（由启程解锁）
        if (from == E_NodeState.Locked && to == E_NodeState.Unlocked) return true;
        // 从 Unlocked 可以转换到 Current（进入节点）或 Completed（完成节点）
        if (from == E_NodeState.Unlocked && (to == E_NodeState.Current || to == E_NodeState.Completed)) return true;
        // 从 Current 可以转换到 Completed（完成节点）或 Unlocked（取消选中）
        if (from == E_NodeState.Current && (to == E_NodeState.Completed || to == E_NodeState.Unlocked)) return true;
        // 从 Completed 不允许再转换（已完成节点不可变）
        if (from == E_NodeState.Completed) return false;
        // Boss 解锁状态允许进入和完成
        if (from == E_NodeState.BossUnlocked && (to == E_NodeState.Current || to == E_NodeState.Completed)) return true;
        return true; // 默认允许，可根据需要收紧
    }

    /// <summary>
    /// 锁定指定楼层的所有节点（除了已完成节点）
    /// </summary>
    private void LockLayerAllNodes(int layerIndex)
    {
        if (!_layerNodeDic.TryGetValue(layerIndex, out var nodeIds)) return;

        foreach (var nodeId in nodeIds)
        {
            // 已完成的节点保留 Completed 状态，其他节点锁定
            if (nodeId != _currentCompletedNodeId)
            {
                // 只将非 Completed 的节点锁定
                var currentState = GetNodeState(nodeId);
                if (currentState != E_NodeState.Completed)
                {
                    UpdateNodeState(nodeId, E_NodeState.Locked);
                }
            }
        }
    }

    /// <summary>
    /// 解锁指定楼层的所有节点（仅锁定状态 -> 解锁）
    /// </summary>
    private void UnlockLayerAllNodes(int layerIndex)
    {
        if (!_layerNodeDic.TryGetValue(layerIndex, out var nodeIds)) return;

        foreach (var nodeId in nodeIds)
        {
            // 仅解锁当前为锁定状态的节点
            if (GetNodeState(nodeId) == E_NodeState.Locked)
            {
                UpdateNodeState(nodeId, E_NodeState.Unlocked);
            }
        }
    }
    #endregion

    #region 核心流程逻辑
    /// <summary>
    /// 进入节点
    /// </summary>
    public void EnterNode(string nodeId)
    {
        if (_isGameEnd)
        {
            Debug.Log("游戏已结束，无法进入节点");
            return;
        }

        if (!_allNodeDic.TryGetValue(nodeId, out var nodeData))
        {
            Debug.LogError($"节点 {nodeId} 未注册");
            return;
        }

        // 仅 Unlocked / BossUnlocked 状态可进入
        if (nodeData.state != E_NodeState.Unlocked && nodeData.state != E_NodeState.BossUnlocked)
        {
            Debug.LogWarning($"节点 {nodeId} 当前状态为 {nodeData.state}，无法进入");
            return;
        }

        // 标记节点为当前选中状态
        UpdateNodeState(nodeId, E_NodeState.Current);

        // 触发节点进入事件（战斗/事件/营地模块监听）
        EventCenter.Instance.EventTrigger(E_EventType.Tower_EnterNode, nodeData);
    }

    /// <summary>
    /// 完成节点（节点战斗/事件结束后调用）
    /// </summary>
    /// <param name="nodeId">完成的节点ID</param>
    /// <param name="rewardExp">奖励执照经验（可选）</param>
    public void CompleteNode(string nodeId, int rewardExp = 0)
    {
        if (_isGameEnd)
        {
            Debug.Log("游戏已结束，无法完成节点");
            return;
        }

        if (string.IsNullOrEmpty(nodeId))
        {
            Debug.LogError("节点ID为空，无法完成");
            return;
        }

        if (!_allNodeDic.TryGetValue(nodeId, out var nodeData))
        {
            Debug.LogError($"节点 {nodeId} 不存在，无法完成");
            return;
        }

        // 防止重复完成
        if (nodeData.state == E_NodeState.Completed)
        {
            Debug.LogWarning($"节点 {nodeId} 已经完成，跳过");
            return;
        }

        // 1. 结算奖励经验（如果节点子类没有单独调用，这里兜底）
        if (rewardExp > 0)
        {
            GrowthMgr.Instance.AddLicenseExp(rewardExp);
        }

        // 2. 标记节点为已完成
        _currentCompletedNodeId = nodeId;
        UpdateNodeState(nodeId, E_NodeState.Completed);

        // 3. 锁定当前楼层所有其他节点（已完成节点除外）
        LockLayerAllNodes(nodeData.layerIndex);

        // 4. 更新已完成楼层，设置启程按钮可点击
        currentCompletedLayerIndex = nodeData.layerIndex;
        //SetDepartBtnState(true);
        OnDepartBtnClick();

        // 5. 触发楼层完成事件
        EventCenter.Instance.EventTrigger(E_EventType.Tower_LayerComplete, nodeData.layerIndex);

        // 6. 校验是否通关（BOSS 节点完成）
        if (nodeData.type == E_TowerNodeType.BossBattle)
        {
            OnTowerComplete();
        }

        Debug.Log($"节点 {nodeId} 完成，当前楼层 {nodeData.layerIndex} 已锁定，启程按钮已启用");
    }

    /// <summary>
    /// 启程按钮点击事件
    /// </summary>
    public void OnDepartBtnClick()
    {
        if (_isGameEnd)
        {
            Debug.Log("游戏已结束，无法启程");
            return;
        }

        //if (!isDepartBtnInteractable)
        //{
        //    Debug.Log("启程按钮不可用，请先完成当前楼层节点");
        //    return;
        //}

        // 计算下一层索引
        int nextLayerIndex = currentCompletedLayerIndex + 1;

        // 校验是否超出总楼层
        if (nextLayerIndex > totalLayerCount)
        {
            Debug.LogWarning("已到达最终楼层，无法继续启程");
            return;
        }

        // 解锁下一层所有节点
        currentActiveLayerIndex = nextLayerIndex;
        UnlockLayerAllNodes(nextLayerIndex);

        // 启程后按钮置灰，不可重复点击
        //SetDepartBtnState(false);

        Debug.Log($"启程成功，解锁第 {nextLayerIndex} 层");
    }

    /// <summary>
    /// 设置启程按钮可交互状态
    /// </summary>
    private void SetDepartBtnState(bool isInteractable)
    {
        if (isDepartBtnInteractable == isInteractable) return;
        isDepartBtnInteractable = isInteractable;
        EventCenter.Instance.EventTrigger(E_EventType.UI_DepartBtnStateChanged, isInteractable);
    }
    #endregion

    #region 游戏结束判定
    /// <summary>
    /// 玩家血量变化监听，判定游戏失败
    /// </summary>
    private void OnPlayerHpChanged((int currentHp, int maxHp) hpData)
    {
        if (hpData.currentHp <= 0 && !_isGameEnd)
        {
            OnTowerFailed();
        }
    }

    /// <summary>
    /// 爬塔失败处理
    /// </summary>
    public void OnTowerFailed()
    {
        if (_isGameEnd) return;
        _isGameEnd = true;
        SetDepartBtnState(false);
        EventCenter.Instance.EventTrigger(E_EventType.Tower_Failed);
        Debug.Log("爬塔失败！");
    }

    /// <summary>
    /// 爬塔通关处理
    /// </summary>
    private void OnTowerComplete()
    {
        if (_isGameEnd) return;
        _isGameEnd = true;
        SetDepartBtnState(false);
        EventCenter.Instance.EventTrigger(E_EventType.Tower_Complete);
        Debug.Log("爬塔通关！");
    }
    #endregion

    //调试
    public void DumpNodeStates()
    {
        foreach (var kv in _allNodeDic)
        {
            Debug.Log($"节点 {kv.Key} 楼层 {kv.Value.layerIndex} 状态 {kv.Value.state}");
        }
    }
}