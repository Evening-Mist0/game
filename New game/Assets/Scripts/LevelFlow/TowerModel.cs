using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
public class TowerModel : BaseMgr<TowerModel>
{
    private TowerModel()
    {

    }
    // 核心数据
    private int currentLayer = 1;          // 当前层数
    private int maxLayer = 8;              // 最大层数（8层）
    private List<List<TowerNodeData>> towerData = new List<List<TowerNodeData>>(); // 每层节点数据

    // 对外属性
    public int CurrentLayer => currentLayer;
    public int MaxLayer => maxLayer;

    // 初始化爬塔数据（游戏开局调用）
    public void InitTowerData()
    {
        towerData.Clear();
        currentLayer = 1;

        // 按需求生成8层节点（2营地+5战斗+1BOSS）
        for (int layer = 1; layer <= maxLayer; layer++)
        {
            List<TowerNodeData> layerNodes = new List<TowerNodeData>();

            // 固定节点配置（按需求：2/6层营地，3层精英，8层BOSS）
            NodeType fixedNodeType = NodeType.NormalBattle;
            if (layer == 2 || layer == 6) fixedNodeType = NodeType.Camp;
            if (layer == 3) fixedNodeType = NodeType.EliteBattle;
            if (layer == 8) fixedNodeType = NodeType.Boss;

            // 添加固定节点
            layerNodes.Add(new TowerNodeData
            {
                nodeId = $"Layer{layer}_Fixed",
                nodeType = fixedNodeType,
                nodeState = layer == 1 ? NodeState.Unlocked : NodeState.Locked, // 仅第1层解锁
                layerIndex = layer
            });

            // 非BOSS层添加1-2个随机战斗节点
            if (layer != 8)
            {
                int randomNodeCount = UnityEngine.Random.Range(1, 3);
                for (int i = 0; i < randomNodeCount; i++)
                {
                    layerNodes.Add(new TowerNodeData
                    {
                        nodeId = $"Layer{layer}_Random{i}",
                        nodeType = NodeType.NormalBattle,
                        nodeState = layer == 1 ? NodeState.Unlocked : NodeState.Locked,
                        layerIndex = layer
                    });
                }
            }

            towerData.Add(layerNodes);
        }
    }

    // 获取指定层数的所有节点
    public List<TowerNodeData> GetLayerNodes(int layer)
    {
        if (layer < 1 || layer > maxLayer) return null;
        return towerData[layer - 1];
    }

    // 标记节点为已完成
    public void CompleteNode(string nodeId)
    {
        // 遍历找到对应节点
        for (int i = 0;i < towerData.Count;i++)
        {
            var layerNodes = towerData[i];
            var node = layerNodes.Find(n => n.nodeId == nodeId);
            if (node != null)
            {
                node.nodeState = NodeState.Completed;
                break;
            }
        }

        if(IsLayerAllNodesCompleted(currentLayer))
        {
            UnlockNextLayer();
        }

    }

    // 检查当前层所有节点是否完成
    private bool IsLayerAllNodesCompleted(int layer)
    {
        var layerNodes = GetLayerNodes(layer);
        foreach (var node in layerNodes)
        {
            if (node.nodeState != NodeState.Completed) return false;
        }
        return true;
    }

    // 解锁下一层
    private void UnlockNextLayer()
    {
        if (currentLayer >= maxLayer) return;

        currentLayer++;
        var nextLayerNodes = GetLayerNodes(currentLayer);
        foreach (var node in nextLayerNodes)
        {
            node.nodeState = NodeState.Unlocked; // 解锁下一层所有节点
        }

        // 触发层数变更事件（供LevelFlowMgr监听）

    }

}
