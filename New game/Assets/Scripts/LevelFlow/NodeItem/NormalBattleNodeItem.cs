using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 普通战斗节点
/// 核心逻辑：进入节点→加载普通战斗→胜利后结算1点经验+40%概率白色/绿色遗物
/// </summary>
public class NormalBattleNodeItem : BaseBattleNodeItem
{
    //怪物最大数量   
    [Tooltip("怪物最大数量")]
    public int maxMonsterCounts;
    //怪物最小数量
    [Tooltip("怪物最小数量")]
    public int minMonsterCounts;
    protected override void Awake()
    {
        base.Awake();
        // 监听带参数的事件
        EventCenter.Instance.AddEventListener<string>(E_EventType.Battle_NormalBattleWin, OnBattleWin);
    }

  
    protected override void OnNodeClick()
    {
        base.OnNodeClick();

        //切换战斗音乐
        AudioMgr.Instance.PlayBGM("普通关_墨影阵图");

        // 构建战斗信息
        
        BattleInfo info = new BattleInfo
        {
            nodeId = nodeId,
            battleType = E_TowerNodeType.NormalBattle, // nodeType 为 E_TowerNodeType.NormalBattle
            monsterCounts = Random.Range(battleInfo.minMonsterCounts, battleInfo.maxMonsterCounts + 1)
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
        // 2. 40%概率掉落奇物
        RelicConfig droppedRelic = null;
        if (Random.Range(0, 100) < 40)
            droppedRelic = GrowthMgr.Instance.GetRandomRelicByDropRate();

        // 3. 如果有掉落，添加到数据
        if (droppedRelic != null)
            GrowthMgr.Instance.AddRelic(droppedRelic.relicId);
            EventCenter.Instance.EventTrigger(E_EventType.Growth_GetRelic, droppedRelic);

        // 4. 显示奖励面板，点击确定后完成节点并返回爬塔界面
        List<RelicConfig> relics = droppedRelic != null ? new List<RelicConfig> { droppedRelic } : new List<RelicConfig>();
        UIMgr.Instance.ShowPanel<RewardPanel>(E_UILayerType.top, (panel) =>
        {
            panel.ShowRewards(relics, null, () =>
            {
            // 完成节点（增加经验）
            LevelFlowMgr.Instance.CompleteNode(nodeId, rewardExp);
            // 显示爬塔面板
            UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
            });
        });
    }
}
