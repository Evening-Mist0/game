using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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
[RequireComponent(typeof(BaseMonster)), RequireComponent(typeof(Animator)), RequireComponent(typeof(SpriteRenderer))]
public class MonsterEffectControl : MonoBehaviour
{
     private Animator animator;
    private SpriteRenderer sr;
    //怪物的图片
    public Sprite monsterSprite;
    //预设体图标位置（grid）
    public GameObject iconPrefab;
    
    private void Awake()
    {
        animator = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
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