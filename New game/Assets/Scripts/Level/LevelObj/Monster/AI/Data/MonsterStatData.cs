//// MonsterStats.cs - 运行时状态数据
//using System.Collections.Generic;
//using UnityEngine;

//public class MonsterStatData
//{
//    public int currentHp;
//    public int currentAttack;
//    public int currentDefense;
//    public int currentRound;
//    public GridPos currentPosition;
//    public bool isAlive => currentHp > 0;

//    // 状态效果
//    public int burnRemainingRounds;
//    public int imprisonRemainingRounds;
//    public List<E_CardSkill> activeEffects = new List<E_CardSkill>();

//    public void Reset(MonsterData data)
//    {
//        currentHp = data.maxHp;
//        currentAttack = data.attack;
//        currentRound = 0;
//        burnRemainingRounds = 0;
//        imprisonRemainingRounds = 0;
//        activeEffects.Clear();
//    }
//}