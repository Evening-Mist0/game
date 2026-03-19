using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TowerLayerConfig", menuName = "游戏配置/爬塔/楼层配置")]
public class TowerLayerConfigSO : ScriptableObject
{
    [Header("楼层总数")]
    public int totalLayerCount = 8;
    [Header("每层配置")]
    public List<LayerConfig> layerConfigs = new List<LayerConfig>();
}

[System.Serializable]
public class LayerConfig
{
    [Header("楼层索引(1-8)")]
    public int layerIndex;
    [Header("该层节点列表")]
    public List<NodeConfig> nodeConfigs = new List<NodeConfig>();
}

[System.Serializable]
public class NodeConfig
{
    [Header("节点唯一ID（必须与预设中的nodeId一致）")]
    public string nodeId;
    [Header("所属楼层索引")]
    public int layerIndex; // 新增：用于判断节点层级
    [Header("节点类型")]
    public E_TowerNodeType nodeType;
    // 移除uiPos字段，因为不需要动态设置位置
}


