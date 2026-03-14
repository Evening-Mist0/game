using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BeginPanel : BasePanel
{
    public Image imgCard;
    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch(name)
        {
            case "btnTest":
                print("测试按钮点击");
                break;
        }
    }
    protected override void Awake()
    {
        UIMgr.Instance.AddCustomEventListener<Image>(imgCard,EventTriggerType.PointerEnter,(data) => { print("鼠标进入"); });
        UIMgr.Instance.AddCustomEventListener<Image>(imgCard,EventTriggerType.Drag,(data) => { print("鼠标正在拖拽"); });
        UIMgr.Instance.AddCustomEventListener<Image>(imgCard,EventTriggerType.PointerExit,(data) => { print("鼠标离开"); });
    }
    public override void HideMe()
    {
        base.HideMe();
        print("BeginPanel隐藏");
    }
    public override void ShowMe()
    {
        base.ShowMe();
        print("BeginPanel显示");
    }

    public void Test()
    {
        print("BeginPanel测试函数");
    }

}
