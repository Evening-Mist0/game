using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class TipPanel : BasePanel
{
    [SerializeField] private TextMeshProUGUI  tipTxt;
    private Animator animator;

    protected override void Awake()
    {
        base.Awake();
        animator = GetComponent<Animator>();
        animator.SetTrigger("isShow");

    }
    /// <summary>
    /// 初始化提示面板
    /// </summary>
    public void Init(string texts)
    {
        tipTxt.text = texts;
    }

    public void Hideme()
    {
        UIMgr.Instance.HidePanel<TipPanel>();
    }


}
