using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopPanel : BasePanel
{
    [Header("????")]
    [SerializeField] private Transform whiteRelicContainer;
    [SerializeField] private Transform greenRelicContainer;
    [SerializeField] private Transform blueRelicContainer;

    [Header("????")]
    [SerializeField] private Transform bookContainer;

    [Header("????")]
    [SerializeField] private Transform upgradeContainer;



    [SerializeField] private GameObject shopItemPrefab;
    [SerializeField] private TextMeshProUGUI copperText;
    [SerializeField] private Button refreshBtn;
    [SerializeField] private Button closeBtn;
    [SerializeField] private TextMeshProUGUI refreshCostText;

    private Action onCloseCallback;
    // ??????
    private bool isFirstRefresh = true; 

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
        RefreshShop(); 
    }

    private void RefreshShop()
    {
        refreshCostText.text = "???????";
        int cost = ShopConfigMgr.Instance.GetRefreshCost(isFirstRefresh);
        if (cost > 0 && !GrowthMgr.Instance.SpendCopperCoins(cost))
        {
            Debug.Log("?????????");
            return;
        }
        isFirstRefresh = false;

        if(cost > 0)
        {
            refreshCostText.text = "??15????";
        }
           

        currentData = ShopConfigMgr.Instance.GenerateShopItems();
        RefreshArea(whiteRelicContainer, currentData.whiteRelics);
        RefreshArea(greenRelicContainer, currentData.greenRelics);
        RefreshArea(blueRelicContainer, currentData.blueRelics);
        RefreshArea(bookContainer, currentData.books);
        RefreshArea(upgradeContainer, currentData.upgrades);
        UpdateCopperUI();
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

        // ????????
        RefreshShop();
        UpdateCopperUI();
    }

    private void UpdateCopperUI()
    {
        copperText.text = GrowthMgr.Instance.GetCopperCoins().ToString();
    }

    private void OnCopperChanged(int newCopper)
    {
        UpdateCopperUI();
        // ???????????
        RefreshAllItemsInteractable();
    }

    private void RefreshAllItemsInteractable()
    {
        // ??????????UI???????
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
        HideMe();
    }

}
