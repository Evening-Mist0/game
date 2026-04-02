using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static RandomEventNodeItem;

public class EventPanel : BasePanel
{
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    private RandomEventConfig currentConfig;
    private string currentNodeId;

    /// <summary>
    /// 初始化事件面板
    /// </summary>
    public void Init(RandomEventConfig config, string nodeId)
    {
        currentConfig = config;
        currentNodeId = nodeId;
        descText.text = config.eventDesc;

        // 清空旧按钮
        foreach (Transform child in buttonContainer)
            Destroy(child.gameObject);

        // 生成选项按钮
        foreach (string optionText in config.optionTexts)
        {
            GameObject btnObj = Instantiate(buttonPrefab, buttonContainer);
            Button btn = btnObj.GetComponent<Button>();
            TextMeshProUGUI btnText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = optionText;
            btn.onClick.AddListener(() => OnOptionClick(optionText));
        }
    }

    private void OnOptionClick(string optionText)
    {
        // 根据事件类型执行不同逻辑
        switch (currentConfig.eventType)
        {
            case E_RandomEventType.GetExp:
                GrowthMgr.Instance.AddLicenseExp(2);
                FinishEvent();
                break;

            case E_RandomEventType.TakeDamage:
                GrowthMgr.Instance.PlayerTakeDamage(5);
                FinishEvent();
                break;

            case E_RandomEventType.SellRelic:
                // 打开奇物出售面板（选择拥有的奇物，兑换经验）
                OpenRelicSellPanel();
                break;

            case E_RandomEventType.SellBook:
                OpenBookSellPanel();
                break;

            case E_RandomEventType.RecoverHpByRelic:
                OpenRelicRecoverPanel();
                break;
        }
    }

    private void FinishEvent()
    {
        EventCenter.Instance.EventTrigger(E_EventType.Event_OptionConfirm, currentNodeId);
        HideMe();
    }

    private void OpenRelicSellPanel()
    {
        // 获取玩家拥有的奇物列表（需从 GrowthMgr 获取）
        var ownedRelics = GetOwnedRelicConfigs(); // 需实现
        if (ownedRelics.Count == 0)
        {
            Debug.Log("没有可出售的奇物");
            FinishEvent(); // 或者直接结束事件
            return;
        }
        // 显示奇物选择面板，选择后获得经验（白色1，绿色2，蓝色3），移除奇物
        UIMgr.Instance.ShowPanel<RelicSelectPanel>(E_UILayerType.middle);
        var relicSelectPanel = UIMgr.Instance.GetPanel<RelicSelectPanel>();
        relicSelectPanel.Init(E_RelicSelectMode.Sell, ownedRelics, (selectedRelic) =>
        {
            int expGain = 0;
            switch (selectedRelic.quality)
            {
                case E_RelicQuality.White: expGain = 1; break;
                case E_RelicQuality.Green: expGain = 2; break;
                case E_RelicQuality.Blue: expGain = 3; break;
            }
            GrowthMgr.Instance.AddLicenseExp(expGain);
            GrowthMgr.Instance.RemoveRelic(selectedRelic.relicId); // 需实现
            FinishEvent();
        });
    }

    private void OpenBookSellPanel()
    {
        // 变卖典籍：固定获得2点经验，移除一本典籍（需选择）
        var ownedBooks = GrowthMgr.Instance.GetOwnedBookConfigs(); // 列表
        if (ownedBooks.Count == 0)
        {
            Debug.Log("没有可出售的典籍");
            FinishEvent();
            return;
        }
        UIMgr.Instance.ShowPanel<BookSelectPanel>(E_UILayerType.middle);
        var bookSelectPanel = UIMgr.Instance.GetPanel<BookSelectPanel>();
        // 显示拥有的典籍列表
        bookSelectPanel.Init(E_BookSelectMode.Sell, ownedBooks, (selectedBook) =>
        {
            GrowthMgr.Instance.AddLicenseExp(2);
            GrowthMgr.Instance.RemoveBook(selectedBook.bookId); // 需实现
            FinishEvent();
        });
    }

    private void OpenRelicRecoverPanel()
    {
        var ownedRelics = GetOwnedRelicConfigs();
        if (ownedRelics.Count == 0)
        {
            Debug.Log("没有可消耗的奇物");
            FinishEvent();
            return;
        }
        UIMgr.Instance.ShowPanel<RelicSelectPanel>(E_UILayerType.middle);
        var relicSelectPanel = UIMgr.Instance.GetPanel<RelicSelectPanel>();
        relicSelectPanel.Init(E_RelicSelectMode.Recover, ownedRelics, (selectedRelic) =>
        {
            int recoverHp = 0;
            switch (selectedRelic.quality)
            {
                case E_RelicQuality.White: recoverHp = 3; break;
                case E_RelicQuality.Green: recoverHp = 6; break;
                case E_RelicQuality.Blue: recoverHp = 9; break;
            }
            GrowthMgr.Instance.PlayerRecoverHp(recoverHp);
            GrowthMgr.Instance.RemoveRelic(selectedRelic.relicId);
            FinishEvent();
        });
    }

    private List<RelicConfig> GetOwnedRelicConfigs()
    {
        List<RelicConfig> list = new List<RelicConfig>();
        foreach (var relicId in GrowthMgr.Instance.growthData.ownedRelicIds)
        {
            var cfg = GrowthMgr.Instance.GetRelicConfig(relicId);
            if (cfg != null) list.Add(cfg);
        }
        return list;
    }
}
