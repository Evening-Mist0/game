using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 攻击动画类型枚举，用于播放对应攻击动作
public enum E_AttackAnimType
{
    Normal,        // 普通攻击
    Boss_God_FireFormAtk,   //  Boss神火形态攻击
    Boss_God_WaterFormAtk,  // Boss神水形态攻击
    Boss_God_EarthFormAtk,  // Boss神地形态攻击
}

/// <summary>
/// 图标类型枚举
/// </summary>
public enum E_IconType
{
    /// <summary>
    /// 燃烧图标
    /// </summary>
    Burn,
    /// <summary>
    /// 禁锢图标
    /// </summary>
    Imprison,
    /// <summary>
    /// 加速图标
    /// </summary>
    Speed,
}

/// <summary>
/// 怪物特效控制组件，挂载在怪物身上，负责所有表现效果
/// </summary>
public class MonsterEffectControl : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;

    // 异常状态图标位置
    private MonsterBuffEffectControl debuffControl;

    // 血条控制
    private BloodEffectControl bloodControl;

    private void Awake()
    {

    }

    /// <summary>
    /// 获取UI组件并初始化血条
    /// </summary>
    /// <param name="hp">怪物的当前血量</param>
    public void Init(int hp)
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        bloodControl = this.gameObject.GetComponentInChildren<BloodEffectControl>();

        debuffControl = this.gameObject.GetComponentInChildren<MonsterBuffEffectControl>();

        if (debuffControl == null)
            Debug.LogError("怪物状态显示组件未挂载");

        if (bloodControl == null)
            Debug.LogError("血条组件未挂载");

        UpdateBlood(hp);
    }

    /// <summary>
    /// 更新血条显示
    /// </summary>
    public void UpdateBlood(int hp)
    {
        bloodControl.UpdateBlood(hp);
    }

    /// <summary>
    /// 更新异常状态图标
    /// </summary>
    public void UpdateDebuff()
    {

    }

    /// <summary>
    /// 播放攻击动画
    /// </summary>
    public void PlayAtkAnimation(E_AttackAnimType type)
    {
        switch (type)
        {
            case E_AttackAnimType.Normal:
                Debug.Log("播放普通攻击动画");
                break;
            case E_AttackAnimType.Boss_God_FireFormAtk:
                animator.SetTrigger("FireForm_Atk");
                break;
            case E_AttackAnimType.Boss_God_WaterFormAtk:
                Debug.Log("播放Boss水形态攻击动画");
                animator.SetTrigger("WaterForm_Atk");
                break;
            case E_AttackAnimType.Boss_God_EarthFormAtk:
                animator.SetTrigger("EarthForm_Atk");
                break;
        }
    }

    /// <summary>
    /// 播放移动动画
    /// </summary>
    public void PlayMoveAnimation()
    {
        Debug.Log("播放移动动画");
    }

    /// <summary>
    /// 播放死亡动画
    /// </summary>
    public void PlayDeadAnimation()
    {
        Debug.Log("播放死亡动画");
    }

    /// <summary>
    /// 显示BUFF/DEBUFF图标
    /// </summary>
    public void DisplayIcon(E_IconType iconType)
    {
        switch (iconType)
        {
            case E_IconType.Burn:
                Debug.Log("[显示图标]显示燃烧图标");
                break;
            case E_IconType.Imprison:
                Debug.Log("[显示图标]显示禁锢图标");
                break;
            default:
                Debug.LogWarning("[显示图标]显示图标类型未处理");
                break;
        }
    }

    /// <summary>
    /// 销毁BUFF/DEBUFF图标
    /// </summary>
    public void DestoryIcon(E_IconType iconType)
    {
        switch (iconType)
        {
            case E_IconType.Burn:
                Debug.Log("[销毁图标]移除燃烧图标");
                break;
            case E_IconType.Imprison:
                Debug.Log("[销毁图标]移除禁锢图标");
                break;
            default:
                Debug.LogWarning("[销毁图标]销毁图标类型未处理");
                break;
        }
    }

    public void PlayBurnEffect()
    {

    }

    public void PlayImprisonEffect()
    {

    }

    public void PlaySpeedUpEffect()
    {

    }
}