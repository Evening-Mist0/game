using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BookSelectPanel : BasePanel
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject bookOptionPrefab;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private TextMeshProUGUI tipText; // 多选时显示已选数量

    private List<BookOptionItem> optionItems = new List<BookOptionItem>();
    private List<BookConfig> selectedBooks = new List<BookConfig>();
    private Action<List<BookConfig>> onConfirmMulti;   // 多选回调
    private Action<BookConfig> onConfirmSingle;        // 单选回调
    private E_BookSelectMode currentMode;
    private int maxSelectCount = 2; // 多选时的最大数量

    protected override void Awake()
    {
        base.Awake();
        confirmBtn.onClick.AddListener(OnConfirm);
    }

    /// <summary>
    /// 多选模式（悟道）
    /// </summary>
    public void Init(List<BookConfig> bookList, int maxSelect, Action<List<BookConfig>> onConfirm)
    {
        currentMode = E_BookSelectMode.Acquire;
        this.onConfirmMulti = onConfirm;
        this.maxSelectCount = maxSelect;
        selectedBooks.Clear();
        UpdateTip();

        // 清空旧选项
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);
        optionItems.Clear();

        foreach (var book in bookList)
        {
            GameObject opt = Instantiate(bookOptionPrefab, contentRoot);
            BookOptionItem item = opt.GetComponent<BookOptionItem>();
            item.Init(book, () => OnBookToggled(book, item));
            optionItems.Add(item);
        }

        titleText.text = $"请选择 {maxSelect} 本典籍";
        confirmBtn.interactable = false;
        tipText.gameObject.SetActive(true);
    }

    /// <summary>
    /// 单选模式（出售）
    /// </summary>
    public void Init(string desctxt,List<BookConfig> bookList, Action<BookConfig> onConfirm)
    {
        currentMode = E_BookSelectMode.Sell;
        this.onConfirmSingle = onConfirm;
        selectedBooks.Clear();

        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);
        optionItems.Clear();

        foreach (var book in bookList)
        {
            GameObject opt = Instantiate(bookOptionPrefab, contentRoot);
            BookOptionItem item = opt.GetComponent<BookOptionItem>();
            item.Init(book, () => OnBookSelectedSingle(book));
            optionItems.Add(item);
        }

        titleText.text = desctxt;
        confirmBtn.gameObject.SetActive(false); // 单选模式直接点击选项即完成，不需要确认按钮
        tipText.gameObject.SetActive(false);
    }

    private void OnBookToggled(BookConfig book, BookOptionItem item)
    {
        AudioMgr.Instance.PlaySFX("选牌音效");

        if (selectedBooks.Contains(book))
        {
            selectedBooks.Remove(book);
            item.SetSelected(false);
        }
        else
        {
            if (selectedBooks.Count >= maxSelectCount)
            {
                Debug.Log($"最多只能选择 {maxSelectCount} 本典籍");
                return;
            }
            selectedBooks.Add(book);
            item.SetSelected(true);
        }
        UpdateTip();
        confirmBtn.interactable = (selectedBooks.Count == maxSelectCount);
    }

    private void OnBookSelectedSingle(BookConfig book)
    {

        onConfirmSingle?.Invoke(book);
        ClosePanel();
    }

    private void UpdateTip()
    {
        tipText.text = $"已选择 {selectedBooks.Count}/{maxSelectCount}";
    }

    private void OnConfirm()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        if (currentMode == E_BookSelectMode.Acquire)
        {
            if (selectedBooks.Count != maxSelectCount) return;
            onConfirmMulti?.Invoke(selectedBooks);
            ClosePanel();
        }
    }

    private void ClosePanel()
    {
        UIMgr.Instance.HidePanel<BookSelectPanel>();
        UIMgr.Instance.HidePanel<CampPanel>();
        UIMgr.Instance.ShowPanel<TowerPanel>();
    }
}


