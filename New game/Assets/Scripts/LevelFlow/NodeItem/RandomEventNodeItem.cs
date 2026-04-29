using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 随机事件节点
/// </summary>
public class RandomEventNodeItem : BaseNodeItem
{
    protected override void Awake()
    {
        base.Awake();

    }

    protected override void OnNodeClick()
    {
        base.OnNodeClick();
        // 随机选择一个新事件类型
        E_RandomEventType eventType = GetRandomNewEventType();
        // 根据类型打开对应的专用面板
        switch (eventType)
        {
            case E_RandomEventType.Healer:
                OpenHealerPanel();
                break;
            case E_RandomEventType.Scholar:
                OpenScholarPanel();
                break;
            case E_RandomEventType.Gambler:
                OpenGamblerPanel();
                break;
            case E_RandomEventType.TreasureHouse:
                OpenTreasureHousePanel();
                break;
            case E_RandomEventType.ScaleTrade:
                OpenScaleTradePanel();
                break;
            case E_RandomEventType.AltarUpgrade:
                OpenAltarUpgradePanel();
                break;
        }
    }

    private E_RandomEventType GetRandomNewEventType()
    {
        var types = System.Enum.GetValues(typeof(E_RandomEventType));
        return (E_RandomEventType)types.GetValue(Random.Range(0, types.Length));
    }

    private void OpenHealerPanel()
    {
        //UIMgr.Instance.ShowPanel<HealerPanel>(E_UILayerType.middle);
        //var panel = UIMgr.Instance.GetPanel<HealerPanel>();
        //panel.Init(nodeId);
    }

    private void OpenScholarPanel()
    {
        //UIMgr.Instance.ShowPanel<ScholarPanel>(E_UILayerType.middle);
        //var panel = UIMgr.Instance.GetPanel<ScholarPanel>();
        //panel.Init(nodeId);
    }

    private void OpenGamblerPanel()
    {
        //UIMgr.Instance.ShowPanel<GamblerPanel>(E_UILayerType.middle);
        //var panel = UIMgr.Instance.GetPanel<GamblerPanel>();
        //panel.Init(nodeId);
    }

    private void OpenTreasureHousePanel()
    {
        //UIMgr.Instance.ShowPanel<TreasureHousePane>(E_UILayerType.middle);
        //var panel = UIMgr.Instance.GetPanel<TreasureHousePane>();
        //panel.Init(nodeId);
    }

    private void OpenScaleTradePanel()
    {
        //UIMgr.Instance.ShowPanel<ScaleTradePanel>(E_UILayerType.middle);
        //var panel = UIMgr.Instance.GetPanel<ScaleTradePanel>();
        //panel.Init(nodeId);
    }

    private void OpenAltarUpgradePanel()
    {
        //UIMgr.Instance.ShowPanel<AltarUpgradePanel>(E_UILayerType.middle);
        //var panel = UIMgr.Instance.GetPanel<AltarUpgradePanel>();
        //panel.Init(nodeId);
    }

    private void FinishEvent()
    {
        LevelFlowMgr.Instance.CompleteNode(nodeId);
        UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
    }
}
