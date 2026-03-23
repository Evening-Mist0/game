using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectControl : MonoBehaviour
{
    private Animator animator;

    ///// <summary>
    ///// 记录是否播放过受到攻击动画，如果受到攻击，本轮不再播放受击动画,会在怪物攻击回合结束的时候设置为true
    ///// </summary>
    //public bool isPlayHurt;

    public BloodEffectControl bloodControl;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        bloodControl = GetComponentInChildren<BloodEffectControl>();
        if (bloodControl == null)
            Debug.LogError("BloodEffectControl为空");
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
        animator.SetTrigger("Dead");
    }

    public void PlayerHurt()
    {

        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("PlayerHurt"))
            animator.Play("PlayerHurt", 0, 0f);
        else
            animator.SetTrigger("Hurt");   
    }

    //public void ResetPlayHurt()
    //{
    //    isPlayHurt = true;
    //}
}
