using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 管理怪物的美术表现效果，必须挂载在含有BaseMosnter脚本的对象上
/// </summary>
[RequireComponent(typeof(BaseMonster)), RequireComponent(typeof(Animator))]
public class MonsterEffectControl : MonoBehaviour
{
     private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }


    /// <summary>
    /// 播放动画
    /// </summary>
    /// <param name="type">对应状态下的怪物动画</param>
    public void PlayAnimation(E_AIStateType type)
    {
        //目前是播放删帧动画
        switch (type)
        {
            case E_AIStateType.Move:
                Debug.Log("播放移动动画");
                break;
            case E_AIStateType.Atk:
                Debug.Log("播放攻击动画");
                break;
            case E_AIStateType.Dead:
                Debug.Log("播放死亡动画");
                break;
        }
    }
}