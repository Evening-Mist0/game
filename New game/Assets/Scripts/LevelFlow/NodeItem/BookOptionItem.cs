using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// 典籍选项挂载在选项预制体上
public class BookOptionItem : MonoBehaviour
{
    [SerializeField] private Image imgBook;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Button button;

    public void Init(BookConfig book, Action onClick)
    {
        imgBook.sprite = book.bookIcon;
        nameText.text = book.bookName;
        descText.text = book.bookDesc;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
    }
}
