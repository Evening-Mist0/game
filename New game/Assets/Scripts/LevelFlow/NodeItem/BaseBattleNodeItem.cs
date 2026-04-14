using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBattleNodeItem : BaseNodeItem
{
    protected BattleItemLevelConfigSO battleInfo; // 当前节点的战斗信息
    private void Start()
    {
        //读取战斗节点关卡数据
        switch (nodeType)
        {
            case E_TowerNodeType.None:
            case E_TowerNodeType.Camp:
            case E_TowerNodeType.RandomEvent:
                break;
            case E_TowerNodeType.NormalBattle:
                Debug.Log("节点" + nodeType + "所在层数：" + layerIndex);
                battleInfo = Resources.Load<BattleItemLevelConfigSO>("BattleItemLevelSO/Normal/NormalBattleNodeLevel" + layerIndex);
                break;
            case E_TowerNodeType.EliteBattle:
                Debug.Log("节点" + nodeType + "所在层数：" + layerIndex);
                battleInfo = Resources.Load<BattleItemLevelConfigSO>("BattleItemLevelSO/Elite/EliteBattleNodeLevel" + layerIndex);
                break;
            case E_TowerNodeType.BossBattle:
                Debug.Log("节点" + nodeType + "所在层数：" + layerIndex);
                battleInfo = Resources.Load<BattleItemLevelConfigSO>("BattleItemLevelSO/Boss/BossBattleNodeLevel" + layerIndex);
                break;
        }
    }
}
