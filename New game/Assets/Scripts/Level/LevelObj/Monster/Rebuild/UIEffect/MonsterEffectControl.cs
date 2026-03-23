using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 攻击动画类型枚举（怪物有多段攻击效果）
public enum E_AttackAnimType
{
    Normal,// 普通攻击
    Boss_God_FireFormAtk,  
    Boss_God_WaterFormAtk,  
    Boss_God_EarthFormAtk,   
}

/// <summary>
/// 图标类型枚举
/// </summary>
public enum E_IconType
{
    /// <summary>
    /// 灼烧图标
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
/// 管理怪物的美术表现效果，必须挂载在含有BaseMosnter脚本的对象上
/// </summary>
public class MonsterEffectControl : MonoBehaviour
{
     private Animator animator;
    private SpriteRenderer sr;

    //负面状态图标位置
    private MonsterBuffEffectControl debuffControl;

    //血条
    private BloodEffectControl bloodControl;
    
    private void Awake()
    {
       
    }

    /// <summary>
    /// 获取UI层面的组件，初始化血量
    /// </summary>
    /// <param name="hp">怪物的当前血量</param>
    public void Init(int hp)
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        bloodControl = this.gameObject.GetComponentInChildren<BloodEffectControl>();

        debuffControl = this.gameObject.GetComponentInChildren<MonsterBuffEffectControl>();

        if (debuffControl == null)
            Debug.LogError("负面状态显示父对象未挂载");

        if (bloodControl == null)
            Debug.LogError("血条控件没有挂载");
        UpdateBlood(hp);
    }
    

    /// <summary>
    /// 更新血量
    /// </summary>
    public void UpdateBlood(int hp)
    {
        bloodControl.UpdateBlood(hp);
    }

    /// <summary>
    /// 更新负面状态图标
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
                Debug.Log("播放普通怪物攻击动画");
                break;
            case E_AttackAnimType.Boss_God_FireFormAtk:
                animator.SetTrigger("FireForm_Atk");
                break;
            case E_AttackAnimType.Boss_God_WaterFormAtk:
                Debug.Log("播放boss水形态攻击动画");
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

    public void DisplayIcon(E_IconType iconType)
    {
        switch (iconType)
        {         
            case E_IconType.Burn:
                Debug.Log("[显示图标]加载火焰图标");
                break;          
            case E_IconType.Imprison:
                Debug.Log("[显示图标]加载禁锢图标");
                break;
            default:
                Debug.LogWarning("[显示图标]显示该图标的枚举还没有加载路径");
                break;         
        }
    }

    public void DestoryIcon(E_IconType iconType)
    {
        switch (iconType)
        {
            case E_IconType.Burn:
                Debug.Log("[显示图标]删除火焰图标");
                break;
            case E_IconType.Imprison:
                Debug.Log("[显示图标]删除禁锢图标");
                break;
            default:
                Debug.LogWarning("[显示图标]删除该图标的枚举还没有加载路径");
                break;
        }
    }
}