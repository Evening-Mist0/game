using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家类,并不是真正的玩家,用于测试
/// </summary>
public class PlayerTest : BaseGameObject
{
    private static PlayerTest instance;

    public static PlayerTest Instance => instance;

    public int hp = 1;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
    public override E_GameObjectType gameObjectType => E_GameObjectType.Player;

    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="value">受到的伤害值</param>
    public void Hurt(int value)
    {
        Debug.Log("玩家受到伤害" + value);
        hp -= value;
        if (hp <= 0)
            Debug.Log("[游戏结算]玩家游戏失败");
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
