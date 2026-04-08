using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class TextSpriteDamage : MonoBehaviour
{
    Animator animator;
    public TMP_Text textDamageNumber;

    [Tooltip("伤害字体存在的时间")]
    public float showTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("没有挂载Animator");
    }
    // Start is called before the first frame update

    private void OnEnable()
    {
        this.transform.localScale = Vector3.one;
        Invoke("DestroyMe", showTime);
    }

    /// <summary>
    /// 展示本次的伤害，对象池拿出后必用
    /// </summary>
    public void ShowDamage(int damage,Vector3 worldPos)
    {
        textDamageNumber.text  = "-"+damage.ToString();
        this.transform.position = worldPos;
        animator.Rebind();
        animator.SetTrigger("Play");


    }

    public void DestroyMe()
    {
        PoolMgr.Instance.PushObj(this.gameObject);
    }
}
