using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TowerRuntimeData
{
    //当前所在楼层
    public int currentLayerIndex;
    //所有楼层的节点状态字典 key:节点ID value:节点状态
    public Dictionary<string, E_NodeState> nodeStateDic = new Dictionary<string, E_NodeState>();
    //已完成的节点数量
    public int completedNodeCount;


    // 重置爬塔数据
    public void ResetData()
    {
        currentLayerIndex = 1;
        nodeStateDic.Clear();
        completedNodeCount = 0;
    }
}
