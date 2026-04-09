using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUpOptionItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Button button;

    public void Init(LevelUpOptionConfig config, System.Action onClick)
    {
        nameText.text = config.optionName;
        descText.text = config.optionDesc;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onClick());
    }
}
