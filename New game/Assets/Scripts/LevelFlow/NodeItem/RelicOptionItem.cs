using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 奇物选项组件
public class RelicOptionItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Image qualityBg; // 品质背景色
    [SerializeField] private Button button;

    public void Init(RelicConfig relic, Action onClick)
    {
        icon.sprite = relic.relicIcon;
        nameText.text = relic.relicName;
        descText.text = relic.relicDesc;
        // 根据品质设置背景颜色
        Color color = relic.quality switch
        {
            E_RelicQuality.White => Color.white,
            E_RelicQuality.Green => Color.green,
            E_RelicQuality.Blue => Color.blue,
            _ => Color.gray
        };
        qualityBg.color = color;

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
    }
}
