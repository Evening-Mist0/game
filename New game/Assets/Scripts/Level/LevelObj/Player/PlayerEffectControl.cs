using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectControl : MonoBehaviour
{
    /// <summary>
    /// 角色状态机
    /// </summary>
    private Animator animator;


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

    /// <summary>
    /// 玩家受伤时视觉层面更新
    /// </summary>
    /// <param name="nowHp">当前玩家的血量</param>
    public void PlayerHurt(int nowHp, int maxHp,int nowDef)
    {

        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("PlayerHurt"))
            animator.Play("PlayerHurt", 0, 0f);
        else
            animator.SetTrigger("Hurt");

        //更新护甲
        bloodControl.UpdateSpriteDef(nowDef);
        //更新血条
        bloodControl.UpdateSpriteBlood(nowHp,maxHp);
    }
}
