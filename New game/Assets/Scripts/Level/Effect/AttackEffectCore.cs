using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 所有的出牌/回合结算控件
/// </summary>
public class AttackEffectCore : MonoBehaviour
{
    private Animator animator;
    private bool isAlive;
    [HideInInspector]
    public Transform target;


    /// <summary>
    /// 每次激活特效等待多久放回对象池（要让特效播完再放，所以有延迟）
    /// </summary>
    public float destoryTime;

    private void Awake()
    {
        animator = this.transform.GetComponent<Animator>();
        if (animator == null)
            Debug.Log("请为子对象挂载Animator组件");
        
    }

    private void OnEnable()
    {
        isAlive = true;
        animator.SetTrigger("Play");
        Invoke("DestoryMe", destoryTime);
    }

    private void OnDisable()
    {
        isAlive = false;

    }

    public void DestoryMe()
    {
        //重置播放状态
        animator.Play(0, 0, 0f);
        //放入对象池
        PoolMgr.Instance.PushObj(this.gameObject);
    }


    private void LateUpdate()
    {
        if (target != null && (isAlive = true))
        {
            transform.position = target.position;
            transform.rotation = target.rotation;
        }
    }
}
