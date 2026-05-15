using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllRulePanel : BasePanel
{
    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch(name)
        {
            case "btnPanelExplain":
                AudioMgr.Instance.PlaySFX("按钮点击");
                HandlePanelExplain();
            break;
            case "btnCardComposite":
                AudioMgr.Instance.PlaySFX("按钮点击");

                HandleCardComposite();
            break;
            case "btnCardCombo":
                AudioMgr.Instance.PlaySFX("按钮点击");

                HandleCardCombo();
            break;
            case "btnRouge":
                AudioMgr.Instance.PlaySFX("按钮点击");

                HandleRouge();
            break;
            case "btnExit":
                AudioMgr.Instance.PlaySFX("按钮点击");

                HandleExit();
            break;
        }    
    }

    public override void ShowMe()
    {
        base.ShowMe();
        UIMgr.Instance.HidePanel<PlayerInfoPanel>();
    }

    public override void HideMe()
    {
        base.HideMe();
        UIMgr.Instance.ShowPanel<PlayerInfoPanel>(E_UILayerType.system);


    }

    public void HandlePanelExplain()
    {
                UIMgr.Instance.ShowPanel<RulePanelExplainPanel>();

    }

    public void HandleCardComposite()
    {
        UIMgr.Instance.ShowPanel<RuleCardCompositePanel>();

    }

    public void HandleCardCombo()
    {
        UIMgr.Instance.ShowPanel<RuleCardComboPanel>();

    }

    public void HandleRouge()
    {
        UIMgr.Instance.ShowPanel<RuleRougePanel>();

    }

    public void HandleExit()
    {
        UIMgr.Instance.ShowPanel<PlayerInfoPanel>(E_UILayerType.system);
        HideMe();

    }
}
