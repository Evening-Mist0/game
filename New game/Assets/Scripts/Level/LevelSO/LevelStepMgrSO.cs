using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelStepMgrSO", menuName = "BaseMgrScriptable/LevelStepMgrSO")]

public class LevelStepMgrSO : SingleMgrScriptableObject<LevelStepMgrSO>
{
    [Tooltip("玩家出生的位置")]
    public Vector3 playerPos;
}
