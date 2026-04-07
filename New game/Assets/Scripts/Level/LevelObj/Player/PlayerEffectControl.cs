using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectControl : MonoBehaviour
{
    /// <summary>
    /// 角色状态机
    /// </summary>
    private Animator animator;


    private BloodEffectControl bloodControl;

    // 效果图标控件
    private BuffEffectControl buffControl;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        bloodControl = GetComponentInChildren<BloodEffectControl>();
        if (bloodControl == null)
            Debug.LogError("BloodEffectControl为空");

        buffControl = this.GetComponentInChildren<BuffEffectControl>();
        if (buffControl == null)
            Debug.LogError("Buff组件未挂载");
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
    public void PlayerHurt(int damage,int nowHp, int maxHp,int nowDef)
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

        GameObject obj = PoolMgr.Instance.GetObj("TextSpriteDamage");
        TextSpriteDamage text = obj.GetComponent<TextSpriteDamage>();
        text.ShowDamage(damage, this.transform.position);
    }

    public void AddBuffIcon (E_BuffIconType type)=> buffControl.AddBuffIcon(type);

    public void UpdateIconCount(E_BuffIconType type,int round) => buffControl.UpdateIconCount(type, round);

    public void RemoveBuffIcon(E_BuffIconType type) => buffControl.RemoveBuffIcon(type);
    public void UpdateSpriteBlood(int hp,int maxHp) => bloodControl.UpdateSpriteBlood(hp,maxHp);
    public void UpdateSpriteDef(int currentDef) => bloodControl.UpdateSpriteDef(currentDef);
}
