using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家类,并不是真正的玩家,用于测试
/// </summary>
public class PlayerTest : BaseLevelObject
{
    private static PlayerTest instance;

    public static PlayerTest Instance => instance;

    
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public override E_LevelObjectType levelObjectType => E_LevelObjectType.Player;

    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="value">受到的伤害值</param>
    public void Hurt(int value)
    {
        Debug.Log("玩家受到伤害" + value);
    }

    /// <summary>
    /// 玩家得到治愈
    /// </summary>
    /// <param name="value">治愈的值</param>
    public void GetHeal(int value)
    {
        Debug.Log("玩家得到治愈效果" + value);
    }
}
