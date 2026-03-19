using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 爬塔模块总管理器
/// 负责爬塔流程、节点管理、层间流程控制
/// </summary>
public class LevelFlowMgr : BaseMgr<LevelFlowMgr>
{
    //配置数据
    private TowerLayerConfigSO towerLayerConfig;


    //运行时数据
    public TowerRuntimeData towerRuntimeData { get; private set; }
    public bool isInTower { get; private set; } // 是否正在爬塔中


    // 私有构造函数 符合BaseMgr单例规范
    private LevelFlowMgr()
    {
        towerRuntimeData = new TowerRuntimeData();
        // 加载配置数据(Resources路径可根据项目调整)
        towerLayerConfig = Resources.Load<TowerLayerConfigSO>("Config/TowerLayerConfig");
    }

    #region 爬塔生命周期
    /// <summary> 开始新的爬塔 从开始界面点击开始游戏时调用 </summary>
    public void StartNewTower()
    {
        isInTower = true;
        // 重置爬塔数据
        towerRuntimeData.ResetData();
        // 初始化玩家成长数据
        GrowthMgr.Instance.InitNewGameData();
        // 初始化第一层节点
        InitLayerNodes(towerRuntimeData.currentLayerIndex);
        // 通知UI刷新爬塔面板
        EventCenter.Instance.EventTrigger(E_EventType.Tower_InitComplete);
        // 显示爬塔面板
        //UIMgr.Instance.ShowPanel<TowerPanel>();
    }

    /// <summary> 初始化指定楼层的节点 </summary>
    private void InitLayerNodes(int layerIndex)
    {
        LayerConfig layerConfig = GetLayerConfig(layerIndex);
        if (layerConfig == null) return;

        // 初始化节点状态
        foreach (var node in layerConfig.nodeConfigs)
        {
            // 第一层节点默认解锁
            E_NodeState initState = layerIndex == 1 ? E_NodeState.Unlocked : E_NodeState.Locked;
            if (!towerRuntimeData.nodeStateDic.ContainsKey(node.nodeId))
                towerRuntimeData.nodeStateDic.Add(node.nodeId, initState);
        }
    }

    /// <summary> 进入指定节点 </summary>
    public void EnterNode(string nodeId)
    {
        if (!towerRuntimeData.nodeStateDic.ContainsKey(nodeId)) return;
        if (towerRuntimeData.nodeStateDic[nodeId] != E_NodeState.Unlocked) return;

        // 获取节点配置
        NodeConfig nodeConfig = GetNodeConfig(nodeId);
        if (nodeConfig == null) return;

        // 更新节点状态
        towerRuntimeData.nodeStateDic[nodeId] = E_NodeState.Current;
        EventCenter.Instance.EventTrigger(E_EventType.Tower_NodeStateChanged, nodeId);

        // 分发进入节点事件
        EventCenter.Instance.EventTrigger(E_EventType.Tower_EnterNode, nodeConfig);

        // 根据节点类型执行对应逻辑
        switch (nodeConfig.nodeType)
        {
            case E_TowerNodeType.NormalBattle:
            case E_TowerNodeType.EliteBattle:
            case E_TowerNodeType.BossBattle:
                // 关闭爬塔面板，通知战斗模块加载对应战斗场景
                //UIMgr.Instance.HidePanel<TowerPanel>(false);
                // 这里触发战斗加载事件，战斗模块监听该事件即可
                EventCenter.Instance.EventTrigger(E_EventType.loadProgrees, 0f);
                break;
            case E_TowerNodeType.Camp:
                // 打开营地面板
                //UIMgr.Instance.ShowPanel<CampPanel>();
                break;
            case E_TowerNodeType.RandomEvent:
                // 打开随机事件面板
                //UIMgr.Instance.ShowPanel<EventPanel>();
                break;
        }
    }

    /// <summary> 节点完成回调 战斗胜利/事件完成后调用 </summary>
    public void OnNodeComplete(string nodeId)
    {
        if (!towerRuntimeData.nodeStateDic.ContainsKey(nodeId)) return;

        // 标记节点为已完成
        towerRuntimeData.nodeStateDic[nodeId] = E_NodeState.Completed;
        towerRuntimeData.completedNodeCount++;
        EventCenter.Instance.EventTrigger(E_EventType.Tower_NodeStateChanged, nodeId);

        // 解锁下一层
        UnlockNextLayer();
    }

    /// <summary> 解锁下一层 </summary>
    private void UnlockNextLayer()
    {
        int nextLayer = towerRuntimeData.currentLayerIndex + 1;
        LayerConfig nextLayerConfig = GetLayerConfig(nextLayer);
        if (nextLayerConfig == null)
        {
            // 所有楼层完成 爬塔通关
            OnTowerComplete();
            return;
        }

        // 楼层+1，初始化下一层节点
        towerRuntimeData.currentLayerIndex = nextLayer;
        InitLayerNodes(nextLayer);
        // 通知楼层变更
        EventCenter.Instance.EventTrigger(E_EventType.Tower_LayerChanged, towerRuntimeData.currentLayerIndex);
        // 重新显示爬塔面板
        //UIMgr.Instance.ShowPanel<TowerPanel>();
    }

    /// <summary> 爬塔失败 玩家血量为0时调用 </summary>
    public void OnTowerFailed()
    {
        isInTower = false;
        // 触发失败事件
        EventCenter.Instance.EventTrigger(E_EventType.Tower_Failed);
        // 重置所有数据
        ResetAllData();
        // 跳转回开始界面
        //UIMgr.Instance.HidePanel<TowerPanel>();
        UIMgr.Instance.ShowPanel<BeginPanel>();
    }

    /// <summary> 爬塔通关 </summary>
    private void OnTowerComplete()
    {
        isInTower = false;
        // 触发通关事件
        EventCenter.Instance.EventTrigger(E_EventType.Tower_Complete);
        // 重置数据
        ResetAllData();
        // 后续通关结算逻辑可扩展
    }

    /// <summary> 重置所有爬塔与成长数据 </summary>
    private void ResetAllData()
    {
        towerRuntimeData.ResetData();
        GrowthMgr.Instance.ResetGrowthData();
    }
    #endregion

    #region 配置获取工具方法
    /// <summary> 获取楼层配置 </summary>
    public LayerConfig GetLayerConfig(int layerIndex)
    {
        return towerLayerConfig.layerConfigs.Find(l => l.layerIndex == layerIndex);
    }

    /// <summary> 获取节点配置 </summary>
    public NodeConfig GetNodeConfig(string nodeId)
    {
        foreach (var layer in towerLayerConfig.layerConfigs)
        {
            var node = layer.nodeConfigs.Find(n => n.nodeId == nodeId);
            if (node != null) return node;
        }
        Debug.LogError($"未找到节点ID:{nodeId}的配置");
        return null;
    }

    /// <summary> 获取当前楼层的所有节点 </summary>
    public List<NodeConfig> GetCurrentLayerNodes()
    {
        return GetLayerConfig(towerRuntimeData.currentLayerIndex)?.nodeConfigs;
    }

    /// <summary> 获取节点状态 </summary>
    public E_NodeState GetNodeState(string nodeId)
    {
        if (towerRuntimeData.nodeStateDic.TryGetValue(nodeId, out var state))
            return state;
        return E_NodeState.Locked;
    }
    #endregion
}
