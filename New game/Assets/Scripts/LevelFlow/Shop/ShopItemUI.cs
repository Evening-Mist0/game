using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItemUI : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Button buyBtn;
    [SerializeField] private GameObject soldMark;


    private ShopItem item;
    private System.Action onBuy;

    public void Init(ShopItem item, System.Action onBuy)
    {
        this.item = item;
        this.onBuy = onBuy;
        icon.sprite = item.icon;
        nameText.text = item.name;
        descText.text = item.description;
        priceText.text = item.price.ToString();
        buyBtn.onClick.AddListener(() => onBuy());
        soldMark.SetActive(item.isSold);
        buyBtn.interactable = !item.isSold;
    }

    public void SetInteractable(bool interactable)
    {
        if (!item.isSold)
            buyBtn.interactable = interactable;
    }

    public void MarkAsSold()
    {
        soldMark.SetActive(true);
        buyBtn.interactable = false;
    }
    
    public ShopItem GetShopItem() => item;
    
}
