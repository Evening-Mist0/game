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
                HandlePanelExplain();
            break;
            case "btnCardComposite":
                HandleCardComposite();
            break;
            case "btnCardCombo":
                HandleCardCombo();
            break;
            case "btnRouge":
                HandleRouge();
            break;
            case "btnExit":
                HandleExit();
            break;
        }    
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
