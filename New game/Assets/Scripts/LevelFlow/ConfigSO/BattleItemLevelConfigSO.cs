using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewNodeItemata", menuName = "Game/NodeItem/BaseBattleNodeItemData")]

public class BattleItemLevelConfigSO : ScriptableObject
{
    //在max-min之间随机出本轮关卡的怪物生成总量
    [Header("怪物生成总量(本次怪物生成的总数量将在min和max之间随机)")]
    [Tooltip("最大怪物数量")]
    public int maxMonsterCounts;
    [Tooltip("最小怪物数量")]
    public int minMonsterCounts;

    public BattleInfo info;
}
