using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


// 典籍选项挂载在选项预制体上
public class RewordOptionItem : MonoBehaviour
{

    [SerializeField] private Image iconImage;          
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;

    public void Init(Sprite icon, string name, string desc)
    {
        // 设置图标
        if (iconImage != null)
            iconImage.sprite = icon;
        nameText.text = name;
        descText.text = desc;
    }

}
