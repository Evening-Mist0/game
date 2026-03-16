using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_LevelObjectType
{
    /// <summary>
    /// 玩家
    /// </summary>
    Player,
    /// <summary>
    /// 防御塔
    /// </summary>
    Monster,
    /// <summary>
    /// 怪物
    /// </summary>
    DefTower,
}
/// <summary>
/// 关卡内的所有物体实例父类，包含怪物、防御塔,玩家
/// </summary>
public abstract class BaseLevelObject : MonoBehaviour
{
    public abstract E_LevelObjectType levelObjectType { get; }

}


