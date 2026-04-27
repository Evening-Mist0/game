using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopPanel : BasePanel
{
    [Header("奇物")]
    [SerializeField] private Transform whiteRelicContainer;
    [SerializeField] private Transform greenRelicContainer;
    [SerializeField] private Transform blueRelicContainer;

    [Header("典籍")]
    [SerializeField] private Transform bookContainer;

    [Header("典籍升级")]
    [SerializeField] private Transform upgradeContainer;



    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private Button refreshBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI refreshCostText;

    private Action onCloseCallback;
    // 是否免费刷新
    protected bool isFirstRefresh = true; 

    private ShopAreaData currentData;

    protected override void Awake()
    {
        base.Awake();
        refreshBtn.onClick.AddListener(RefreshShop);
        closeBtn.onClick.AddListener(OnClose);
        EventCenter.Instance.AddEventListener<int>(E_EventType.Growth_CopperChanged, OnCopperChanged);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.Growth_CopperChanged, OnCopperChanged);
    }


    public void Init(Action onClose)
    {
        onCloseCallback = onClose;
        refreshCostText.text = "第一次刷新免费";
        RefreshShop(); 
        isFirstRefresh = true; 
    }

    private void RefreshShop()
    {      
        int cost = ShopConfigMgr.Instance.GetRefreshCost(isFirstRefresh);
        if (cost > 0 && !GrowthMgr.Instance.SpendCopperCoins(cost))
        {
            Debug.Log("铜钱不足，无法刷新");
            return;
        }
        if (isFirstRefresh)
        {
            isFirstRefresh = false;
            refreshCostText.text = "花费15铜钱刷新";
        }
           
        currentData = ShopConfigMgr.Instance.GenerateShopItems();
        RefreshArea(whiteRelicContainer, currentData.whiteRelics);
        RefreshArea(greenRelicContainer, currentData.greenRelics);
        RefreshArea(blueRelicContainer, currentData.blueRelics);
        RefreshArea(bookContainer, currentData.books);
        RefreshArea(upgradeContainer, currentData.upgrades);
    }

    private void RefreshArea(Transform container, List<ShopItem> items)
    {
        foreach (Transform child in container) Destroy(child.gameObject);
        foreach (var item in items)
        {
            GameObject go = Instantiate(shopItemPrefab, container);
            var ui = go.GetComponent<ShopItemUI>();
            ui.Init(item, () => OnBuyItem(item));
            ui.SetInteractable(!item.isSold && GrowthMgr.Instance.GetCopperCoins() >= item.price);
        }
    }

    private void OnBuyItem(ShopItem item)
    {
        if (item.isSold) return;
        if (!GrowthMgr.Instance.SpendCopperCoins(item.price)) return;

        switch (item.type)
        {
            case E_ShopItemType.WhiteRelic:
            case E_ShopItemType.GreenRelic:
            case E_ShopItemType.BlueRelic:
                GrowthMgr.Instance.AddRelic(item.itemId);
                break;
            case E_ShopItemType.Book:
                E_BookType bookType = (E_BookType)Enum.Parse(typeof(E_BookType), item.itemId);
                GrowthMgr.Instance.AddBook(bookType);
                break;
            case E_ShopItemType.BookUpgrade:
                E_BookType upgradeType = (E_BookType)Enum.Parse(typeof(E_BookType), item.itemId);
                BookUpgradeMgr.Instance.UpgradeBook(upgradeType);
                break;
        }
        item.isSold = true;

        // 重新刷新整个商店
        RefreshShop();
    }

    private void OnCopperChanged(int newCopper)
    {
        // 刷新所有商品的按钮状态
        RefreshAllItemsInteractable();
    }

    private void RefreshAllItemsInteractable()
    {
        // 遍历所有容器中的商品UI，更新按钮交互
        var allUis = GetComponentsInChildren<ShopItemUI>(true);
        foreach (var ui in allUis)
        {
            var item = ui.GetShopItem();
            ui.SetInteractable(!item.isSold && GrowthMgr.Instance.GetCopperCoins() >= item.price);
        }
    }

    private void OnClose()
    {
        onCloseCallback?.Invoke();
        UIMgr.Instance.HidePanel<ShopPanel>();
    }

}
