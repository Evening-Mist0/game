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
    [SerializeField] private Button closeBtn;

    private Action<BookConfig> onConfirm; // 选中后的回调

    protected override void Awake()
    {
        base.Awake();
        if (closeBtn != null) closeBtn.onClick.AddListener(CloseWithoutSelect);
    }

    /// <summary>
    /// 初始化面板
    /// </summary>
    /// <param name="mode">模式</param>
    /// <param name="bookList">典籍列表（根据模式，可以是未拥有的列表或已拥有的列表）</param>
    /// <param name="onConfirm">选中后的回调，参数为选中的典籍配置</param>
    public void Init(E_BookSelectMode mode, List<BookConfig> bookList, Action<BookConfig> onConfirm)
    {
        this.onConfirm = onConfirm;
        titleText.text = mode == E_BookSelectMode.Acquire ? "选择典籍" : "出售典籍（获得2点经验）";

        // 清空旧选项
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        // 生成选项按钮
        foreach (var book in bookList)
        {
            GameObject opt = Instantiate(bookOptionPrefab, contentRoot);
            BookOptionItem item = opt.GetComponent<BookOptionItem>();
            item.Init(book, () => OnBookSelected(book));
        }
    }

    private void OnBookSelected(BookConfig selectedBook)
    {
        onConfirm?.Invoke(selectedBook);
        ClosePanel();
    }

    private void CloseWithoutSelect()
    {
        // 未选择直接关闭，不触发回调
        ClosePanel();
    }

    private void ClosePanel()
    {
        HideMe();
    }
}


