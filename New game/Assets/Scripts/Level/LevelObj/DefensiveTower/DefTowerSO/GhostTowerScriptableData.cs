using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GhostTowerData", menuName = "Game/DefTower/GhostTowerData")]

public class GhostTowerScriptableData : DefTowerScriptableData
{
    public int existRound;
    public List<GhostTowerSkillPair> skills;
}
