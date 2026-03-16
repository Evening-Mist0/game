using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StartPanel : BasePanel
{
    private Button startbtn;
    private Button endbtn;
    private Button rulebtn;
    protected override void Awake()
    {
        /// 先调用基类Awake，让框架自动收集控件
        base.Awake();

        /// 通过框架方法获取控件（控件名必须和预设体中的名字一致）
        startbtn = GetControl<Button>("startbtn");
        endbtn = GetControl<Button>("endbtn");
        rulebtn = GetControl<Button>("rulebtn");
    }
    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch (name)
        {
            case "startbtn":
                OnStartClick();
                break;
            case "endbtn":
                OnEndClick();
                break;
            case "rulebtn":
                OnRuleClick();
                break;
        }
    }
    private void OnStartClick()
    {
        Debug.Log("点击了开始按钮！");
    }
    private void OnEndClick()
    {
        Debug.Log("退出游戏！");
    }
    private void OnRuleClick()
    {
        UIMgr.Instance.HidePanel<StartPanel>(false);
        UIMgr.Instance.ShowPanel<RulePanel>(E_UILayerType.top);
    }
}    
