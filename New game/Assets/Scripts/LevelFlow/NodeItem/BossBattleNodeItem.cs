using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BOSS战节点
/// 核心逻辑：进入节点→加载BOSS战斗→胜利后结算2经验+必掉蓝色遗物→触发通关
/// </summary>
public class BossBattleNodeItem : BaseBattleNodeItem
{
    [Tooltip("怪物最大数量")]
    public int maxMonsterCounts;
    [Tooltip("怪物最小数量")]
    public int minMonsterCounts;
    [Tooltip("到第几波开始刷精英怪")]
    public int eliteMonsterAppearWaveCount;
    [Tooltip("出现精英怪的初始概率")]
    public int eliteMonsterAppearProb;
    [Tooltip("出现精英怪每回合增长的概率（从下回合开始，100%则满）")]
    public int eliteAppearGrowthProb;
    [Tooltip("精英怪的最多存在数量")]
    public int maxEliteCount;
    [Tooltip("Boss的最多存在数量")]
    public int maxBossCount;
    [Tooltip("Boss出现在第几波")]
    public int bossMonsterAppearWaveCount;

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
        UIMgr.Instance.GetPanel<PlayerInfoPanel>()?.HideMe();

        //切换战斗音乐
        AudioMgr.Instance.PlayBGM("boss_墨阵疾行");

        // 构建战斗信息
        BattleInfo info = new BattleInfo
        {
            nodeId = nodeId,
            battleType = E_TowerNodeType.BossBattle,
            monsterCounts = Random.Range(battleInfo.minMonsterCounts, battleInfo.maxMonsterCounts + 1),
            eliteMonsterAppearWaveCount = battleInfo.info.eliteMonsterAppearWaveCount,
            eliteMonsterAppearProb = battleInfo.info.eliteMonsterAppearProb,
            eliteAppearGrowthProb = battleInfo.info.eliteAppearGrowthProb,
            maxEliteCount = battleInfo.info.maxEliteCount,
            maxBossCount = battleInfo.info.maxBossCount,
            bossMonsterAppearWaveCount = battleInfo.info.bossMonsterAppearWaveCount,
        };

        // 通过战斗管理器启动战斗
        BattleMgr.Instance.StartBattle(info);

     
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
