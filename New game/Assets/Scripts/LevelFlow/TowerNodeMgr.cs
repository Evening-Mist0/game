using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 塔楼节点管理器
/// 负责全楼层节点生成、状态管理、分叉路逻辑、路线数据维护
/// </summary>
public class TowerNodeMgr : BaseMgr<TowerNodeMgr>
{
    #region 配置参数（匹配策划需求+UI设计）
    // 爬塔总层数（固定8层，匹配肉鸽需求）
    private readonly int _totalLayerCount = 8;
    // UI纵向间距配置（每层之间的高度间隔，适配全节点同屏）
    private readonly float _layerVerticalInterval = 180f;
    // 同楼层节点横向间距
    private readonly float _nodeHorizontalInterval = 220f;
    // 面板中心基准X坐标
    private readonly float _centerX = 0f;
    // 8层固定配置规则（匹配策划需求）
    private readonly Dictionary<int, LayerGenerateRule> _layerRuleDic = new Dictionary<int, LayerGenerateRule>()
    {
        {1, new LayerGenerateRule(){ layerIndex=1, fixedNodeType=E_TowerNodeType.NormalBattle, minNodeCount=1, maxNodeCount=1, hasBranch=false }},
        {2, new LayerGenerateRule(){ layerIndex=2, fixedNodeType=E_TowerNodeType.Camp, minNodeCount=3, maxNodeCount=4, hasBranch=true, mustContainType=new List<E_TowerNodeType>(){ E_TowerNodeType.Camp } }},
        {3, new LayerGenerateRule(){ layerIndex=3, fixedNodeType=E_TowerNodeType.EliteBattle, minNodeCount=2, maxNodeCount=3, hasBranch=true, mustContainType=new List<E_TowerNodeType>(){ E_TowerNodeType.EliteBattle } }},
        {4, new LayerGenerateRule(){ layerIndex=4, minNodeCount=2, maxNodeCount=4, hasBranch=true }},
        {5, new LayerGenerateRule(){ layerIndex=5, minNodeCount=2, maxNodeCount=4, hasBranch=true }},
        {6, new LayerGenerateRule(){ layerIndex=6, fixedNodeType=E_TowerNodeType.Camp, minNodeCount=2, maxNodeCount=4, hasBranch=true, mustContainType=new List<E_TowerNodeType>(){ E_TowerNodeType.Camp } }},
        {7, new LayerGenerateRule(){ layerIndex=7, minNodeCount=2, maxNodeCount=4, hasBranch=true }},
        {8, new LayerGenerateRule(){ layerIndex=8, fixedNodeType=E_TowerNodeType.BossBattle, minNodeCount=1, maxNodeCount=1, hasBranch=false, isBossLayer=true }},
    };
    #endregion

    #region 运行时数据
    // 全楼层数据
    private List<TowerLayerData> _allLayerData = new List<TowerLayerData>();
    // 全节点字典（快速查找）
    private Dictionary<string, TowerNodeData> _allNodeDic = new Dictionary<string, TowerNodeData>();
    // 全路线数据
    private List<TowerRouteData> _allRouteData = new List<TowerRouteData>();
    // 当前选中的节点
    private TowerNodeData _currentSelectedNode;
    // 已完成的最后一个节点（用于解锁下一批节点）
    private TowerNodeData _lastCompletedNode;
    #endregion

    #region 构造与生命周期
    private TowerNodeMgr()
    {
        Debug.Log("=== TowerNodeMgr单例已初始化 ===");
        // 注册全局事件监听
        EventCenter.Instance.AddEventListener(E_EventType.Tower_NodeBattleWin, OnNodeChallengeCompleted);
        EventCenter.Instance.AddEventListener(E_EventType.Tower_CampCompleted, OnNodeChallengeCompleted);
        EventCenter.Instance.AddEventListener(E_EventType.Tower_EventCompleted, OnNodeChallengeCompleted);
    }

    /// <summary>
    /// 新局初始化塔楼全数据（开局调用）
    /// </summary>
    public void InitNewTowerData()
    {

        Debug.Log("=== 开始初始化爬塔数据 ==="); 

        _allLayerData.Clear();
        _allNodeDic.Clear();
        _allRouteData.Clear();
        _currentSelectedNode = null;
        _lastCompletedNode = null;

        // 1. 生成所有楼层与节点
        GenerateAllLayersAndNodes();
        // 2. 构建节点前后连接关系与路线
        BuildNodeConnectionAndRoutes();
        // 3. 初始化节点状态（仅首层节点解锁，其余全锁定）
        InitNodeDefaultState();
        // 4. 触发初始化完成事件（UI监听刷新）
        EventCenter.Instance.EventTrigger(E_EventType.Tower_InitComplete);

    }
    #endregion

    #region 节点生成核心逻辑
    /// <summary>
    /// 生成所有楼层与节点
    /// </summary>
    private void GenerateAllLayersAndNodes()
    {
        for (int layerIndex = 1; layerIndex <= _totalLayerCount; layerIndex++)
        {
            Debug.Log($"正在生成楼层：{layerIndex}"); // 新增日志
            if (!_layerRuleDic.TryGetValue(layerIndex, out var rule))
            {
                Debug.LogError($"楼层{layerIndex}无生成规则！"); // 新增错误日志
                continue;
            }

            // 创建楼层数据
            TowerLayerData layerData = new TowerLayerData()
            {
                layerIndex = layerIndex,
                isBossLayer = rule.isBossLayer,
                layerBaseY = (layerIndex - 1) * _layerVerticalInterval, // 从下往上堆叠，首层在最下方
                nodeList = new List<TowerNodeData>()
            };

            // 生成该楼层节点
            int nodeCount = Random.Range(rule.minNodeCount, rule.maxNodeCount + 1);
            // 固定类型楼层强制覆盖
            if (rule.fixedNodeType.HasValue)
            {
                nodeCount = 1;
            }

            // 计算节点横向坐标（居中排布）
            float startX = _centerX - (nodeCount - 1) * _nodeHorizontalInterval / 2f;

            for (int nodeIndex = 0; nodeIndex < nodeCount; nodeIndex++)
            {
                string nodeId = $"{layerIndex}_{nodeIndex}";
                E_TowerNodeType nodeType = GetRandomNodeType(layerIndex, rule, nodeIndex);

                TowerNodeData nodeData = new TowerNodeData()
                {
                    nodeId = nodeId,
                    layerIndex = layerIndex,
                    nodeType = nodeType,
                    nodeState = E_NodeState.Locked,
                    uiPosition = new Vector2(startX + nodeIndex * _nodeHorizontalInterval, layerData.layerBaseY),
                    isBranchExclusive = rule.hasBranch && nodeCount >= 2 // 多节点楼层标记为分叉独占
                };

                layerData.nodeList.Add(nodeData);
                _allNodeDic.Add(nodeId, nodeData);
            }

            _allLayerData.Add(layerData);
        }
    }

    /// <summary>
    /// 随机生成节点类型（匹配楼层规则）
    /// </summary>
    private E_TowerNodeType GetRandomNodeType(int layerIndex, LayerGenerateRule rule, int nodeIndex)
    {
        // 固定类型直接返回
        if (rule.fixedNodeType.HasValue)
            return rule.fixedNodeType.Value;

        // 强制包含的类型（如营地楼层必须有一个营地节点）
        if (rule.mustContainType != null && rule.mustContainType.Count > 0 && nodeIndex == 0)
            return rule.mustContainType[0];

        // 权重随机（普通战斗>随机事件>精英战斗>营地）
        float randomValue = Random.Range(0f, 100f);
        return randomValue switch
        {
            < 50f => E_TowerNodeType.NormalBattle,
            < 75f => E_TowerNodeType.RandomEvent,
            < 90f => E_TowerNodeType.EliteBattle,
            _ => E_TowerNodeType.Camp
        };
    }

    /// <summary>
    /// 构建节点前后连接关系与路线数据
    /// </summary>
    private void BuildNodeConnectionAndRoutes()
    {
        for (int i = 0; i < _allLayerData.Count - 1; i++)
        {
            TowerLayerData currentLayer = _allLayerData[i];
            TowerLayerData nextLayer = _allLayerData[i + 1];

            // 上下楼层节点连接规则：
            // 1. 下一层1个节点：所有上一层节点都连到这个节点
            // 2. 下一层多个节点：上一层每个节点连到下一层对应位置的节点，分叉路逻辑
            if (nextLayer.nodeList.Count == 1)
            {
                var nextNode = nextLayer.nodeList[0];
                foreach (var currentNode in currentLayer.nodeList)
                {
                    currentNode.nextNodeIds.Add(nextNode.nodeId);
                    nextNode.preNodeIds.Add(currentNode.nodeId);
                    // 生成路线
                    AddRouteData(currentNode.nodeId, nextNode.nodeId);
                }
            }
            else
            {
                // 多节点分叉路，按索引就近连接
                for (int nodeIndex = 0; nodeIndex < currentLayer.nodeList.Count; nodeIndex++)
                {
                    var currentNode = currentLayer.nodeList[nodeIndex];
                    // 匹配下一层对应索引的节点，超出则连最后一个
                    int nextNodeIndex = Mathf.Min(nodeIndex, nextLayer.nodeList.Count - 1);
                    var nextNode = nextLayer.nodeList[nextNodeIndex];

                    currentNode.nextNodeIds.Add(nextNode.nodeId);
                    nextNode.preNodeIds.Add(currentNode.nodeId);
                    // 生成路线
                    AddRouteData(currentNode.nodeId, nextNode.nodeId);
                }
            }
        }
    }

    /// <summary>
    /// 添加路线数据
    /// </summary>
    private void AddRouteData(string startNodeId, string endNodeId)
    {
        TowerRouteData routeData = new TowerRouteData()
        {
            routeId = $"{startNodeId}_{endNodeId}",
            startNodeId = startNodeId,
            endNodeId = endNodeId,
            routeState = E_NodeState.Locked
        };
        _allRouteData.Add(routeData);
    }

    /// <summary>
    /// 初始化节点默认状态
    /// </summary>
    private void InitNodeDefaultState()
    {
        // 首层第一个节点解锁
        if (_allLayerData.Count > 0 && _allLayerData[0].nodeList.Count > 0)
        {
            var firstNode = _allLayerData[0].nodeList[0];
            firstNode.nodeState = E_NodeState.Unlocked;
            _lastCompletedNode = firstNode;
            // 同步解锁该节点的出发路线
            UpdateRouteStateByNode(firstNode);
            // 触发状态变更事件
            EventCenter.Instance.EventTrigger(E_EventType.Tower_NodeStateChanged, firstNode.nodeId);
        }

        // BOSS节点初始强制锁定
        var bossLayer = _allLayerData.FirstOrDefault(l => l.isBossLayer);
        if (bossLayer != null && bossLayer.nodeList.Count > 0)
        {
            bossLayer.nodeList[0].nodeState = E_NodeState.Locked;
        }
    }
    #endregion

    #region 节点状态管理核心逻辑
    /// <summary>
    /// 节点点击入口（UI调用）
    /// </summary>
    public void OnNodeClick(string nodeId)
    {
        if (!_allNodeDic.TryGetValue(nodeId, out var nodeData))
        {
            Debug.LogError($"未找到节点：{nodeId}");
            return;
        }

        // 状态校验：仅解锁状态可点击
        if (nodeData.nodeState != E_NodeState.Unlocked)
        {
            Debug.LogWarning($"节点{nodeId}状态为{nodeData.nodeState}，不可点击");
            return;
        }

        // 1. 标记当前选中节点
        _currentSelectedNode = nodeData;
        // 2. 处理分叉路独占逻辑：同楼层其他解锁节点锁定
        HandleBranchExclusiveLock(nodeData);
        // 3. 触发进入节点事件
        EventCenter.Instance.EventTrigger(E_EventType.Tower_EnterNode, nodeData);
    }

    /// <summary>
    /// 处理分叉路独占锁定（选一个，其他变暗锁住）
    /// </summary>
    private void HandleBranchExclusiveLock(TowerNodeData selectedNode)
    {
        if (!selectedNode.isBranchExclusive) return;

        var sameLayerNodes = _allLayerData.First(l => l.layerIndex == selectedNode.layerIndex).nodeList;
        foreach (var node in sameLayerNodes)
        {
            if (node.nodeId != selectedNode.nodeId && node.nodeState == E_NodeState.Unlocked)
            {
                node.nodeState = E_NodeState.Locked;
                EventCenter.Instance.EventTrigger(E_EventType.Tower_NodeStateChanged, node.nodeId);
            }
        }
    }

    /// <summary>
    /// 节点挑战完成回调（战斗/营地/事件完成后调用）
    /// </summary>
    private void OnNodeChallengeCompleted()
    {
        if (_currentSelectedNode == null) return;

        // 1. 标记节点为已完成
        _currentSelectedNode.nodeState = E_NodeState.Completed;
        _lastCompletedNode = _currentSelectedNode;
        EventCenter.Instance.EventTrigger(E_EventType.Tower_NodeStateChanged, _currentSelectedNode.nodeId);

        // 2. 更新路线状态
        UpdateRouteStateByNode(_currentSelectedNode);

        // 3. 解锁后置节点
        UnlockNextNodes(_currentSelectedNode);

        // 4. 清空当前选中
        _currentSelectedNode = null;
    }

    /// <summary>
    /// 解锁当前节点的后置节点
    /// </summary>
    private void UnlockNextNodes(TowerNodeData completedNode)
    {
        foreach (var nextNodeId in completedNode.nextNodeIds)
        {
            if (!_allNodeDic.TryGetValue(nextNodeId, out var nextNode)) continue;

            // 校验前置节点：所有前置都完成，才解锁该节点
            bool allPreCompleted = nextNode.preNodeIds.All(preId =>
                _allNodeDic.TryGetValue(preId, out var preNode) && preNode.nodeState == E_NodeState.Completed);

            if (allPreCompleted && nextNode.nodeState == E_NodeState.Locked)
            {
                nextNode.nodeState = E_NodeState.Unlocked;
                EventCenter.Instance.EventTrigger(E_EventType.Tower_NodeStateChanged, nextNode.nodeId);
                // 同步路线状态
                UpdateRouteStateByNode(nextNode);
            }
        }
    }

    /// <summary>
    /// 根据节点状态更新关联路线状态
    /// </summary>
    private void UpdateRouteStateByNode(TowerNodeData nodeData)
    {
        // 节点已完成：更新出发路线为已通过
        if (nodeData.nodeState == E_NodeState.Completed)
        {
            foreach (var route in _allRouteData.Where(r => r.startNodeId == nodeData.nodeId))
            {
                route.routeState = E_NodeState.Completed;
                EventCenter.Instance.EventTrigger(E_EventType.Tower_RouteStateChanged, route.routeId);
            }
        }
        // 节点已解锁：更新进入路线为解锁
        else if (nodeData.nodeState == E_NodeState.Unlocked)
        {
            foreach (var route in _allRouteData.Where(r => r.endNodeId == nodeData.nodeId))
            {
                route.routeState = E_NodeState.Unlocked;
                EventCenter.Instance.EventTrigger(E_EventType.Tower_RouteStateChanged, route.routeId);
            }
        }
    }

    /// <summary>
    /// 修改节点状态（外部调用）
    /// </summary>
    public void ChangeNodeState(string nodeId, E_NodeState targetState)
    {
        if (!_allNodeDic.TryGetValue(nodeId, out var nodeData)) return;
        nodeData.nodeState = targetState;
        EventCenter.Instance.EventTrigger(E_EventType.Tower_NodeStateChanged, nodeId);
    }
    #endregion

    #region 外部获取数据接口
    /// <summary>
    /// 获取所有楼层数据
    /// </summary>
    public List<TowerLayerData> GetAllLayerData() => _allLayerData;

    /// <summary>
    /// 获取所有节点数据
    /// </summary>
    public Dictionary<string, TowerNodeData> GetAllNodeDic() => _allNodeDic;

    /// <summary>
    /// 获取所有路线数据
    /// </summary>
    public List<TowerRouteData> GetAllRouteData() => _allRouteData;

    /// <summary>
    /// 根据ID获取节点数据
    /// </summary>
    public TowerNodeData GetNodeDataById(string nodeId)
    {
        _allNodeDic.TryGetValue(nodeId, out var nodeData);
        return nodeData;
    }

    /// <summary>
    /// 根据ID获取路线数据
    /// </summary>
    public TowerRouteData GetRouteDataById(string routeId)
    {
        return _allRouteData.FirstOrDefault(r => r.routeId == routeId);
    }
    #endregion

    #region 内部辅助类
    /// <summary> 楼层生成规则 </summary>
    private class LayerGenerateRule
    {
        public int layerIndex; // 楼层索引
        public E_TowerNodeType? fixedNodeType; // 固定节点类型（可空）
        public int minNodeCount; // 最小节点数
        public int maxNodeCount; // 最大节点数
        public bool hasBranch; // 是否有分叉路
        public bool isBossLayer; // 是否为BOSS层
        public List<E_TowerNodeType> mustContainType; // 必须包含的节点类型
    }
    #endregion
}