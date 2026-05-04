using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BookChoicePanel : BasePanel
{
    [SerializeField] private Transform contentRoot;

    [SerializeField] private GameObject bookOptionPrefab;

    [SerializeField] private TextMeshProUGUI descText;

    private Action<BookConfig> onConfirm;        // 选择回调

    public void Init(String desctxt,List<BookConfig> bookList, Action<BookConfig> onConfirm)
    {
        descText.text = desctxt;
        this.onConfirm = onConfirm;

        // 清空旧选项
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        foreach (var book in bookList)
        {
            GameObject opt = Instantiate(bookOptionPrefab, contentRoot);
            BookOptionItem item = opt.GetComponent<BookOptionItem>();
            item.Init(book, () => OnBookSelected(book));
        }
    }

    public void OnBookSelected(BookConfig book)
    {
        onConfirm?.Invoke(book);
        ClosePanel();

    }

    private void ClosePanel()
    {
        UIMgr.Instance.HidePanel<BookChoicePanel>();
    }


    




}
