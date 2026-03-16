using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 规则面板（翻页版），继承框架BasePanel
/// </summary>
public class RulePanel : BasePanel
{
    // 翻页核心变量
    private int currentPageIndex = 0; // 当前页码（从0开始）
    private int maxPageCount;         // 总页数

    // 控件（通过框架GetControl获取）
    private Button upbtn;
    private Button downbtn;
    private TextMeshProUGUI txtPage; // 用TMP_Text，也可以换成Text
    private Image imgContent;        // 规则内容用图片（如果用文字，换成TextMeshProUGUI）
    private Button exit;

    // 每页的规则图片路径（Resources下的路径，比如“Rule/rule_1”）
    private string[] ruleImagePaths = new string[]
    {
        "rule/规则第一页", // 第1页图片
        "rule/规则第二页", // 第2页图片
        "rule/规则第三页"  // 第3页图片（可自行扩展）
    };

    protected override void Awake()
    {
        // 1. 必须先调用基类Awake，框架自动收集控件
        base.Awake();

        // 2. 获取控件（名字和UI里的完全一致）
        upbtn = GetControl<Button>("upbtn");
        downbtn = GetControl<Button>("downbtn");
        txtPage = GetControl<TextMeshProUGUI>("txtPage");
        imgContent = GetControl<Image>("imgcontent");
        exit = GetControl<Button>("exit");

        // 3. 初始化总页数
        maxPageCount = ruleImagePaths.Length;

        // 4. 初始化第一页内容
        UpdatePageContent();
    }

    /// <summary>
    /// 框架自动绑定的按钮点击事件
    /// </summary>
    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch (name)
        {
            case "upbtn": // 上一页
                currentPageIndex--;
                // 边界判断：不能小于0
                currentPageIndex--;
                // 边界判断：不能小于0
                currentPageIndex = Mathf.Max(0, currentPageIndex);
                UpdatePageContent();
                break;

            case "downbtn": // 下一页
                currentPageIndex++;
                currentPageIndex = Mathf.Min(maxPageCount - 1, currentPageIndex);
                UpdatePageContent();
                break;
            case "exit":
                UIMgr.Instance.HidePanel<RulePanel>(false);
                UIMgr.Instance.ShowPanel<StartPanel>(E_UILayerType.top);
                break;
        }
    }

    /// <summary>
    /// 更新当前页的内容和页码显示
    /// </summary>
    private void UpdatePageContent()
    {
        // 1. 更新页码文本（比如“1/3”）
        if (txtPage != null)
        {
            txtPage.text = $"{currentPageIndex + 1}/{maxPageCount}";
        }

        // 2. 更新规则内容（加载对应页的图片）
        if (imgContent != null && currentPageIndex < maxPageCount)
        {
            // 从Resources加载图片（需提前把规则图片放在Resources/Rule/下）
            Sprite ruleSprite = Resources.Load<Sprite>(ruleImagePaths[currentPageIndex]);
            if (ruleSprite != null)
            {
                imgContent.sprite = ruleSprite;
                imgContent.SetNativeSize(); // 适配图片原始大小（可选）
            }
            else
            {
                Debug.LogError($"未找到规则图片：{ruleImagePaths[currentPageIndex]}");
            }
            // 3. 可选：禁用/启用翻页按钮（比如第一页禁用上一页，最后一页禁用下一页）
            if (upbtn != null) upbtn.interactable = currentPageIndex > 0;
            if (downbtn != null) downbtn.interactable = currentPageIndex < maxPageCount - 1;
        }
    }      
}