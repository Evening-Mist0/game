using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpPanel : BasePanel
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform optionContainer;
    [SerializeField] private GameObject optionPrefab;


    private List<LevelUpOptionConfig> currentOptions;
    private bool isSelected = false;

    protected override void Awake()
    {
        base.Awake();
    }

    /// <summary>
    /// 显示升级面板并初始化选项
    /// </summary>
    public void ShowWithOptions(List<LevelUpOptionConfig> options)
    {
        currentOptions = options;
        isSelected = false;

        // 清空旧选项
        foreach (Transform child in optionContainer)
            Destroy(child.gameObject);

        // 生成所有选项按钮
        foreach (var opt in options)
        {
            GameObject optObj = Instantiate(optionPrefab, optionContainer);
            LevelUpOptionItem item = optObj.GetComponent<LevelUpOptionItem>();
            item.Init(opt, () => OnOptionSelected(opt.optionType));
        }

        ShowMe();
    }

    private void OnOptionSelected(E_LevelUpOptionType optionType)
    {
        if (isSelected) return;
        isSelected = true;

        // 调用 GrowthMgr 记录选择并应用即时效果
        GrowthMgr.Instance.SelectLevelUpOption(optionType);

        // 触发选项选择事件（供其他模块刷新UI等）
        //EventCenter.Instance.EventTrigger(E_EventType.Growth_LevelUpOptionSelected, optionType);

        // 关闭面板
        HideMe();
    }

}
