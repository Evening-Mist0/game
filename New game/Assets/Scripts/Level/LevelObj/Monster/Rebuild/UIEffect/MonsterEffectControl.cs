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
/// 怪物特效控制组件，挂载在怪物身上，负责所有表现效果
/// </summary>
public class MonsterEffectControl : MonoBehaviour
{
    private Animator animator;
    private SpriteRenderer sr;
    private BaseMonsterCore owner;


    // 血条控件
    private BloodEffectControl bloodControl;
    // 效果图标控件
    private BuffEffectControl buffControl;

    private void Awake()
    {

    }

    /// <summary>
    /// 获取UI组件并初始化血条
    /// </summary>
    /// <param name="hp">怪物的当前血量</param>
    public void Init(int hp,int maxHp, BaseMonsterCore owner)
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        this.owner = owner;
        bloodControl = this.gameObject.GetComponentInChildren<BloodEffectControl>();
        buffControl = this.GetComponentInChildren<BuffEffectControl>();

        if (bloodControl == null)
            Debug.LogError("血条组件未挂载");
        if (buffControl == null)
            Debug.LogError("Buff组件未挂载");

        UpdateBlood(hp,maxHp);
    }

    /// <summary>
    /// 更新血条显示
    /// </summary>
    public void UpdateBlood(int hp,int maxHp)
    {
        bloodControl.UpdateSpriteBlood(hp,maxHp);
    }

    /// <summary>
    /// 添加图标
    /// </summary>
       
    public void AddBuffIcon(E_BuffIconType type) =>buffControl.AddBuffIcon(type);

    /// <summary>
    /// 移除图标
    /// </summary>
    public void RemoveBuffIcon(E_BuffIconType type) => buffControl.RemoveBuffIcon(type);

    /// <summary>
    /// 更新图标回合数
    /// </summary>
    /// <param name="type">buff图标的类型</param>
    /// <param name="count">buff持续回合数</param>
    public void UpdateIconCount(E_BuffIconType type,int count) => buffControl.UpdateIconCount(type,count);

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
    /// 触发燃烧特效
    /// </summary>
    /// <param name="nowCell">触发燃烧特效的格子</param>
    public void PlayBurnEffect(Cell nowCell)
    {
        EffectCreater.Instance.CreatEffect(E_AttackEffectType.Fire, owner);
    }

    /// <summary>
    /// 触发禁锢特效
    /// </summary>
    /// <param name="nowCell">触发禁锢特效的格子</param>
    public void PlayImprisonEffect(Cell nowCell)
    {
        EffectCreater.Instance.CreatEffect(E_AttackEffectType.Imprison, owner);
    }

    /// <summary>
    /// 触发速度特效
    /// </summary>
    /// <param name="nowCell">触发速度特效的格子</param>
    public void PlaySpeedUpEffect(Cell nowCell)
    {
        EffectCreater.Instance.CreatEffect(E_AttackEffectType.SpeedUp, owner);

    }
}