using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BOSS战节点
/// 核心逻辑：进入节点→加载BOSS战斗→胜利后结算2经验+必掉蓝色遗物→触发通关
/// </summary>
public class BossBattleNodeItem : BaseNodeItem
{
    protected override void Awake()
    {
        base.Awake();
        EventCenter.Instance.AddEventListener<string>(E_EventType.Battle_BossBattleWin, OnBattleWin);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.Instance.RemoveEventListener<string>(E_EventType.Battle_BossBattleWin, OnBattleWin);
    }

    protected override void OnNodeClick()
    {
        base.OnNodeClick();
        EventCenter.Instance.EventTrigger(E_EventType.Battle_LoadBossBattle, nodeId);
    }

    private void OnBattleWin(string winNodeId)
    {
        if (winNodeId != nodeId) return;

        int rewardExp = 2;
        var blueRelics = GrowthMgr.Instance.GetRandomRelicsByQuality(E_RelicQuality.Blue, 1);
        if (blueRelics.Count > 0)
        {
            GrowthMgr.Instance.AddRelic(blueRelics[0].relicId);
            EventCenter.Instance.EventTrigger(E_EventType.Growth_GetRelic, blueRelics[0]);
        }

        LevelFlowMgr.Instance.CompleteNode(nodeId, rewardExp);
        // UIMgr.Instance.ShowPanel<GameWinPanel>(E_UILayerType.top);
    }
}
