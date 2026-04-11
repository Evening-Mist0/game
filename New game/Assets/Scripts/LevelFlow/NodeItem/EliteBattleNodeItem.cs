using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 精英战斗节点
/// 核心逻辑：进入节点→加载精英战斗→胜利后结算1经验+必掉1本典籍+3选1遗物
/// </summary>
public class EliteBattleNodeItem : BaseNodeItem
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


    protected override void Awake()
    {
        base.Awake();
        // 监听带节点ID的精英战斗胜利事件
        EventCenter.Instance.AddEventListener<string>(E_EventType.Battle_EliteBattleWin, OnBattleWin);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.Instance.RemoveEventListener<string>(E_EventType.Battle_EliteBattleWin, OnBattleWin);
    }

    protected override void OnNodeClick()
    {
        base.OnNodeClick();

        //切换战斗音乐
        Debug.Log("精英关卡点击");
        AudioMgr.Instance.PlayBGM("精英关_墨影阵图");


        // 构建战斗信息
        BattleInfo info = new BattleInfo
        {
            nodeId = nodeId,
            battleType = E_TowerNodeType.EliteBattle,
            monsterCounts = Random.Range(minMonsterCounts, maxMonsterCounts + 1),
            eliteMonsterAppearWaveCount = eliteMonsterAppearWaveCount,
            eliteMonsterAppearProb = eliteMonsterAppearProb,
            eliteAppearGrowthProb = eliteAppearGrowthProb,
            maxEliteCount = maxEliteCount,

        }; 

        // 通过战斗管理器启动战斗
        BattleMgr.Instance.StartBattle(info);


    }

    /// <summary>
    /// 精英战斗胜利回调
    /// </summary>
    private void OnBattleWin(string winNodeId)
    {
        if (winNodeId != nodeId) return; // 只处理自己的胜利

        // 1. 基础奖励：1点执照经验
        int rewardExp = 1;

        // 2. 必掉1本未拥有的典籍（若未满2本）
        if (GrowthMgr.Instance.growthData.ownedBooks.Count < GrowthMgr.Instance.growthData.maxBookCount)
        {
            var bookList = GrowthMgr.Instance.GetRandomUnownedBooks(1);
            if (bookList.Count > 0)
            {
                var book = bookList[0];
                GrowthMgr.Instance.AddBook(book.bookId);
                EventCenter.Instance.EventTrigger(E_EventType.Growth_GetBook, book);
            }
        }

        // 3. 必掉奇物（随机品质：白20%/绿50%/蓝30%）
        var relic = GrowthMgr.Instance.GetRandomRelicForElite();
        if (relic != null)
        {
            GrowthMgr.Instance.AddRelic(relic.relicId);

            EventCenter.Instance.EventTrigger(E_EventType.Growth_GetRelic, relic);
        }
        else
        {
            Debug.LogWarning("精英战斗未掉落奇物，请检查奇物配置");
        }

        // 4. 完成节点
        LevelFlowMgr.Instance.CompleteNode(nodeId, rewardExp);
        // 5. 显示爬塔面板
        UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
    }
}
