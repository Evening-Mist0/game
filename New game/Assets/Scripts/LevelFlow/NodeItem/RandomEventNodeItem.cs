using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

/// <summary>
/// 随机事件节点
/// 核心逻辑：进入节点→随机抽取事件→打开事件面板→完成事件结算
/// </summary>
public class RandomEventNodeItem : BaseNodeItem
{
    // 随机事件配置类
    [Serializable]
    public class RandomEventConfig
    {
        public E_RandomEventType eventType;
        public string eventDesc;
        public List<string> optionTexts;
    }

    private RandomEventConfig _currentConfig;

    protected override void Awake()
    {
        base.Awake();
        EventCenter.Instance.AddEventListener<string>(E_EventType.Event_OptionConfirm, OnEventOptionConfirm);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.Instance.RemoveEventListener<string>(E_EventType.Event_OptionConfirm, OnEventOptionConfirm);
    }

    protected override void OnNodeClick()
    {
        base.OnNodeClick();
        // 随机抽取事件
        _currentConfig = GetRandomEventConfig();
        // 打开事件面板
        UIMgr.Instance.ShowPanel<EventPanel>(E_UILayerType.middle);
        var eventPanel = UIMgr.Instance.GetPanel<EventPanel>();
        eventPanel.Init(_currentConfig, nodeId);
    }

    /// <summary>
    /// 随机生成事件配置
    /// </summary>
    private RandomEventConfig GetRandomEventConfig()
    {
        E_RandomEventType randomType = (E_RandomEventType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(E_RandomEventType)).Length);
        RandomEventConfig config = new RandomEventConfig
        {
            eventType = randomType,
            optionTexts = new List<string>()
        };

        switch (randomType)
        {
            case E_RandomEventType.GetExp:
                config.eventDesc = "偶遇世外高人指点，获得2点执照经验！";
                config.optionTexts.Add("多谢指点");
                break;
            case E_RandomEventType.SellRelic:
                config.eventDesc = "遇到神秘商人，可变卖奇物兑换执照经验（白色=1、绿色=2、蓝色=3）";
                config.optionTexts.Add("前往变卖");
                config.optionTexts.Add("拒绝");
                break;
            case E_RandomEventType.SellBook:
                config.eventDesc = "遇到古籍收藏家，可变卖典籍兑换2点执照经验";
                config.optionTexts.Add("前往变卖");
                config.optionTexts.Add("拒绝");
                break;
            case E_RandomEventType.TakeDamage:
                config.eventDesc = "误入陷阱，受到5点伤害！";
                config.optionTexts.Add("自认倒霉");
                break;
            case E_RandomEventType.RecoverHpByRelic:
                config.eventDesc = "遇到生命泉水，可消耗奇物恢复血量（白色=3、绿色=6、蓝色=9）";
                config.optionTexts.Add("饮用泉水");
                config.optionTexts.Add("离开");
                break;
        }
        return config;
    }

    /// <summary>
    /// 事件选项确认回调
    /// </summary>
    private void OnEventOptionConfirm(string confirmNodeId)
    {
        if (confirmNodeId != nodeId) return;
        LevelFlowMgr.Instance.CompleteNode(nodeId);
        UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
    }
}
