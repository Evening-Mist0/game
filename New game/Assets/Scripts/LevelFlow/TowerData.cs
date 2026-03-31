using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 塔楼节点数据 </summary>
[System.Serializable]
public class TowerNodeData
{
    [Header("节点唯一ID")]
    public string nodeId;
    [Header("节点类型")]
    public E_TowerNodeType nodeType;
    [Header("节点状态")]
    public E_NodeState nodeState;
    [Header("所属楼层")]
    public int layerIndex;
    [Header("UI世界坐标（纵向爬塔用）")]
    public Vector2 uiPosition;
    [Header("前置节点ID列表（支持多前置，分叉路用）")]
    public List<string> preNodeIds = new List<string>();
    [Header("后置节点ID列表（支持多后置，分叉路用）")]
    public List<string> nextNodeIds = new List<string>();
    [Header("是否为分叉路独占节点（选一个其他锁定）")]
    public bool isBranchExclusive;
}

/// <summary> 塔楼楼层数据 </summary>
[System.Serializable]
public class TowerLayerData
{
    [Header("楼层索引（1-8，从上到下/从下到上可配置）")]
    public int layerIndex;
    [Header("楼层UI纵向坐标基准")]
    public float layerBaseY;
    [Header("该楼层所有节点")]
    public List<TowerNodeData> nodeList = new List<TowerNodeData>();
    [Header("该楼层是否为BOSS终局层")]
    public bool isBossLayer;
}

/// <summary> 节点路线数据 </summary>
[System.Serializable]
public class TowerRouteData
{
    public string routeId;
    public string startNodeId;
    public string endNodeId;
    public E_NodeState routeState; // 与节点状态联动：锁定/解锁/已通过
}