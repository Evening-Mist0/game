using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookUpPanel : BasePanel
{
    [Header("典籍升级")]
    [SerializeField] private Transform upgradeContainer;
    [SerializeField] private ShopPanel shopPanel;
    [SerializeField] private Button closeBtn;

    private ShopAreaData currentData;

    protected override void Awake()
    {
        base.Awake();
        closeBtn.onClick.AddListener(OnClose);
        EventCenter.Instance.AddEventListener<(E_BookType, int)>(E_EventType.Book_Upgraded, OnBookUpgraded);
        EventCenter.Instance.AddEventListener(E_EventType.Book_UIUpdate, RefreshUpgradeArea);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<(E_BookType, int)>(E_EventType.Book_Upgraded, OnBookUpgraded);
        EventCenter.Instance.RemoveEventListener(E_EventType.Book_UIUpdate, RefreshUpgradeArea);
    }



    public void Init()
    {
        shopPanel = UIMgr.Instance.GetPanel<ShopPanel>();
        currentData = ShopConfigMgr.Instance.GenerateShopItems();
        shopPanel.RefreshArea(upgradeContainer, currentData.upgrades);

    }

    /// <summary>
    /// 仅刷新升级区域（典籍升级商品）
    /// </summary>
    private void RefreshUpgradeArea()
    {
        int upgradeCount = ShopConfigMgr.Instance.GetUpgradeSlotCount();
        int basePrice = ShopConfigMgr.Instance.GetUpgradeBasePrice();
        var upgradeItems = ShopConfigMgr.Instance.GenerateUpgradeItems(upgradeCount, basePrice);
        shopPanel.RefreshArea(upgradeContainer, upgradeItems);
        // 更新本地缓存（可选）
        currentData.upgrades = upgradeItems;
    }

    private void OnBookUpgraded((E_BookType bookType, int newLevel) data)
    {
        // 重新生成升级区域
        currentData.upgrades = ShopConfigMgr.Instance.GenerateUpgradeItems(1, 35); // 实际数量从配置读取
        shopPanel.RefreshArea(upgradeContainer, currentData.upgrades);
    }

    private void OnClose()
    {
        HideMe();
    }

}
