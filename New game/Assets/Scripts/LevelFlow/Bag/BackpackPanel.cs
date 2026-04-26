using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackpackPanel : BasePanel
{
    [SerializeField] private Button cardTabBtn;
    [SerializeField] private Button bookTabBtn;
    [SerializeField] private Button relicTabBtn;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject itemIconPrefab; // 物品图标预制体（包含图片+鼠标悬停显示描述）
    [SerializeField] private Button closeBtn;

    private Animator animator;

    private enum TabType { Card, Book, Relic }
    private TabType currentTab = TabType.Card;

    protected override void Awake()
    {
        base.Awake();
        cardTabBtn.onClick.AddListener(() => SwitchTab(TabType.Card));
        bookTabBtn.onClick.AddListener(() => SwitchTab(TabType.Book));
        relicTabBtn.onClick.AddListener(() => SwitchTab(TabType.Relic));
        closeBtn.onClick.AddListener(ClosePanel);

        animator = GetComponent<Animator>();
    }

    public override void ShowMe()
    {
        base.ShowMe();
        SwitchTab(currentTab);
    }

    private void SwitchTab(TabType tab)
    {
        currentTab = tab;
        // 清空旧内容
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        switch (tab)
        {
            case TabType.Card:
                ShowCards();
                break;
            case TabType.Book:
                ShowBooks();
                break;
            case TabType.Relic:
                ShowRelics();
                break;
        }
    }

    private void ShowCards()
    {
        // 从卡牌管理模块获取玩家拥有的卡牌列表
        //List<CardConfig> ownedCards = GetOwnedCards(); // 需实现
        //foreach (var card in ownedCards)
        //{
        //    CreateItemIcon(card.cardIcon, card.cardName, card.cardDesc);
        //}
    }

    private void ShowBooks()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        var books = GrowthMgr.Instance.GetOwnedBookConfigs();
        foreach (var book in books)
        {
            CreateItemIcon(book.bookIcon, book.bookName, book.bookDesc);
        }
    }

    private void ShowRelics()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        var relics = GrowthMgr.Instance.GetOwnedRelicConfigs();
        foreach (var relic in relics)
        {
            CreateItemIcon(relic.relicIcon, relic.relicName, relic.relicDesc);
        }
    }

    private void CreateItemIcon(Sprite icon, string name, string desc)
    {
        GameObject iconObj = Instantiate(itemIconPrefab, contentRoot);
        Image img = iconObj.GetComponent<Image>();
        img.sprite = icon;

        // 添加鼠标悬停描述
        HoverDescription hover = iconObj.GetComponent<HoverDescription>();
        if (hover == null) hover = iconObj.AddComponent<HoverDescription>();
        hover.Init(name, desc);
    }

    private void ClosePanel()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        animator.SetTrigger("isHide");      
    }

    public void HidePanel()
    {
        HideMe();    
    }



    // 临时方法：获取玩家拥有的卡牌（需要与卡牌模块对接）
    //private List<CardConfig> GetOwnedCards()
    //{
        // 实际应从 CardMgr 获取，这里返回模拟数据
    //    return new List<CardConfig>();
    //}
}


