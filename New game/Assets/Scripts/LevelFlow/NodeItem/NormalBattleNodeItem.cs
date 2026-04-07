using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 普通战斗节点
/// 核心逻辑：进入节点→加载普通战斗→胜利后结算1点经验+40%概率白色/绿色遗物
/// </summary>
public class NormalBattleNodeItem : BaseNodeItem
{
    protected override void Awake()
    {
        base.Awake();
        // 监听带参数的事件
        EventCenter.Instance.AddEventListener<string>(E_EventType.Battle_NormalBattleWin, OnBattleWin);
    }

    protected override void OnNodeClick()
    {
        base.OnNodeClick();

        // 构建战斗信息
        BattleInfo info = new BattleInfo
        {
            nodeId = nodeId,
            battleType = E_TowerNodeType.NormalBattle, // nodeType 为 E_TowerNodeType.NormalBattle
        };

        // 通过战斗管理器启动战斗
        BattleMgr.Instance.StartBattle(info);
    }

    /// <summary>
    /// 普通战斗胜利回调
    /// </summary>
    /// <param name="winNodeId">胜利的节点ID</param>
    private void OnBattleWin(string winNodeId)
    {
        // 只处理属于自己的胜利
        if (winNodeId != nodeId) return;

        // 1. 结算基础奖励：1点执照经验
        int rewardExp = 1;
        // 2. 40%概率掉落遗物（白色70%/绿色30%）
        if (Random.Range(0, 100) < 40)
        {
            var relic = GrowthMgr.Instance.GetRandomRelicByDropRate();
            if (relic != null)
            {
                GrowthMgr.Instance.AddRelic(relic.relicId);
                EventCenter.Instance.EventTrigger(E_EventType.Growth_GetRelic, relic);
            }
        }
        // 3. 完成节点锁整层
        LevelFlowMgr.Instance.CompleteNode(nodeId, rewardExp);
        // 4. 显示爬塔面板
        UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
    }
}
