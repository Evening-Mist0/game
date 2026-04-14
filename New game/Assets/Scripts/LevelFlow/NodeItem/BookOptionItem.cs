using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// 典籍选项挂载在选项预制体上
public class BookOptionItem : MonoBehaviour
{

    [SerializeField] private Image iconImage;          // 新增：典籍图标
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Button button;
    [SerializeField] private Image selectedBg; // 选中时的背景高亮

    private BookConfig book;
    private Action onClick;

    public void Init(BookConfig book, Action onClick)
    {
        // 设置图标
        if (iconImage != null)
            iconImage.sprite = book.bookIcon;
        this.book = book;
        this.onClick = onClick;
        nameText.text = book.bookName;
        descText.text = book.bookDesc;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (selectedBg != null)
            selectedBg.gameObject.SetActive(selected);
    }
}
