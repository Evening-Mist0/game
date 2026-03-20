using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectControl : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void PlayAtk()
    {
        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("PlayerAtk"))
            animator.Play("PlayerAtk", 0, 0f);
        else
            animator.SetTrigger("Atk");
    }

    public void PlayDead()
    {
        Debug.Log("播放玩家死亡动画，当前无动画");
    }

    public void PlayerHurt()
    {
        Debug.Log("播放玩家受伤动画，当前无动画");

    }
}
