using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模式，玩家游戏实体
/// </summary>
[RequireComponent(typeof(PlayerEffectControl))]
public class GamePlayer : BaseGameObject
{
    private static GamePlayer instance;
    public static GamePlayer Instance => instance;

    public override E_GameObjectType gameObjectType => E_GameObjectType.Player;

    public int hp = 1;

    private PlayerEffectControl effectControl;


    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);

        effectControl = GetComponent<PlayerEffectControl>();
    }


    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="value">受到的伤害值</param>
    public void Hurt(int value)
    {
        //播放受到伤害动画
        //播放受伤音效
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
        //播放治愈动画
        //播放治愈音效
        Debug.Log("玩家得到治愈效果" + value);
    }
}
