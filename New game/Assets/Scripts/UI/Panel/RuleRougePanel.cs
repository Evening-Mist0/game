using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RuleRougePanel : BasePanel
{
    [Header("规则页面配置")]
    [SerializeField] private Image ruleContentImage; // 规则图显示主体
    [SerializeField] private Sprite[] rulePages;      // 所有规则页

    private int currentPage = 0; // 当前页码

    protected override void Awake()
    {
        base.Awake(); // 基类自动注册所有按钮
        InitFirstPage();
    }

    /// <summary>
    /// 初始化第一页
    /// </summary>
    private void InitFirstPage()
    {
        if (rulePages == null || rulePages.Length == 0)
            return;

        currentPage = 0;
        RefreshPage();
    }

    /// <summary>
    /// 刷新页面显示 + 按钮状态
    /// </summary>
    private void RefreshPage()
    {
        ruleContentImage.sprite = rulePages[currentPage];

        // 第一页隐藏上一页，最后一页隐藏下一页
        GetControl<Button>("left").gameObject.SetActive(currentPage > 0);
        GetControl<Button>("right").gameObject.SetActive(currentPage < rulePages.Length - 1);
    }

    /// <summary>
    /// 按钮点击监听（BasePanel 自动调用）
    /// </summary>
    protected override void ButtonClick(string name)
    {
        switch (name)
        {
            // 左翻页
            case "left":
                if (currentPage > 0)
                {
                    currentPage--;
                    RefreshPage();
                }
                break;

            // 右翻页
            case "right":
                if (currentPage < rulePages.Length - 1)
                {
                    currentPage++;
                    RefreshPage();
                }
                break;
            case "btnExit":
                HideMe();
                break;
        }
    }

    /// <summary>
    /// 面板显示时
    /// </summary>
    public override void ShowMe()
    {
        base.ShowMe();
        InitFirstPage(); // 每次打开都回到第一页
    }
}
