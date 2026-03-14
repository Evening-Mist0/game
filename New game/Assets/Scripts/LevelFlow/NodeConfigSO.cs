using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 单个节点的数据模型
[Serializable]
public class TowerNodeData
{
    public string nodeId;          // 唯一ID（如"Layer2_Node1"）
    public NodeType nodeType;      // 节点类型
    public NodeState nodeState;    // 节点状态
    public int layerIndex;         // 所属层数
}

// 节点类型）
public enum NodeType { NormalBattle, EliteBattle, Camp, Boss, Event }
// 节点状态
public enum NodeState { Locked, Unlocked, Completed }