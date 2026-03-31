using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{

    public TowerPanel towerPanel;
    // Start is called before the first frame update
    void Start()
    {
        LevelFlowMgr.Instance.ClearAllData();
        GrowthMgr.Instance.ResetGrowthData();
        // 重新初始化爬塔面板
        UIMgr.Instance.GetPanel<TowerPanel>()?.ClearTowerPanel();

        UIMgr.Instance.ShowPanel<TowerPanel>(E_UILayerType.middle);
        // 2. 初始化游戏流程
        LevelFlowMgr.Instance.InitNewGame();



    }

    public void SimulateNormalBattleWin()
    {
        // 触发普通战斗胜利事件
        string currentNodeId = "Layer1_Start";
        EventCenter.Instance.EventTrigger(E_EventType.Battle_NormalBattleWin, currentNodeId);
    }
    public void SimulateNormalBattleWin2()
    {
        // 触发普通战斗胜利事件
        string currentNodeId = "Layer2_Random_1";
        EventCenter.Instance.EventTrigger(E_EventType.Battle_NormalBattleWin, currentNodeId);
    }

}
