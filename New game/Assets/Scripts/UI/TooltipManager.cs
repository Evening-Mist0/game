using UnityEngine;
using UnityEngine.UI;

// 物品悬浮描述面板管理器（全局单例，确保唯一）
public class TooltipManager : MonoBehaviour
{
    [Header("面板组件（拖拽赋值）")]
    public GameObject tooltipPanel;       // 描述面板根物体（ItemTooltipPanel）
    public Image tooltipBackground;       // 气泡背景（Background）
    public Text descText;                 // 描述文本（DescriptionText）

    [Header("位置配置（可自定义）")]
    public Vector2 offset = new Vector2(20, -10); // 面板相对鼠标的偏移（避免遮挡鼠标）
    public float maxWidth = 300;          // 描述文本最大宽度（超出自动换行）

    // 单例实例（全局访问）
    public static TooltipManager Instance;

    // 初始化：单例设置 + 初始隐藏面板
    private void Awake()
    {
        // 单例逻辑（确保全局唯一）
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换不销毁
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // 初始隐藏面板
        HideTooltip();
        // 设置文本最大宽度
        descText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, maxWidth);
    }

    // 对外提供：显示描述面板（更新文本+位置）
    /// <param name="desc">物品描述文本</param>
    /// <param name="targetPos">参考位置（物品按钮/鼠标位置）</param>
    public void ShowTooltip(string desc, Vector2 targetPos)
    {
        // 校验文本非空
        if (string.IsNullOrEmpty(desc))
        {
            HideTooltip();
            return;
        }

        // 1. 更新描述文本
        descText.text = desc;
        // ========== 新增：打印Text的首选尺寸/实际尺寸 ==========
        LayoutElement textLayout = descText.GetComponent<LayoutElement>();
        if (textLayout != null)
        {
            Debug.Log($"Text首选宽度：{textLayout.preferredWidth}，是否勾选：{textLayout.preferredWidth > 0}");
            Debug.Log($"Text最小宽度：{textLayout.minWidth}");
        }
        // 打印Text的实际渲染尺寸
        Debug.Log($"Text实际宽度：{descText.rectTransform.rect.width}，高度：{descText.rectTransform.rect.height}");

        // 强制刷新布局（先Text后Background，顺序关键）
        LayoutRebuilder.ForceRebuildLayoutImmediate(descText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipBackground.rectTransform);
        // 刷新布局后添加：手动设置Background尺寸为Text尺寸+Padding
        RectTransform bgRect = tooltipBackground.GetComponent<RectTransform>();
        RectTransform textRect = descText.rectTransform;
        // Text实际尺寸 + Background的Padding（左右15+15，上下10+10）
        float bgWidth = textRect.rect.width + 30;
        float bgHeight = textRect.rect.height + 20;
        // 兜底：最小宽度不小于200，避免还是太小
        bgWidth = Mathf.Max(bgWidth, 200);
        bgHeight = Mathf.Max(bgHeight, 60);
        // 应用尺寸
        bgRect.sizeDelta = new Vector2(bgWidth, bgHeight);
        Debug.Log($"手动设置后Background尺寸：{bgWidth}×{bgHeight}");
        // 打印Background刷新后的尺寸
        Debug.Log($"Background刷新后尺寸：{tooltipBackground.rectTransform.rect.width}×{tooltipBackground.rectTransform.rect.height}");
        // 2. 强制刷新布局（确保背景自适应文字，避免延迟）
        LayoutRebuilder.ForceRebuildLayoutImmediate(tooltipBackground.rectTransform);
        // 3. 调整面板位置（跟随鼠标/物品，避免超出屏幕）
        SetTooltipPosition(targetPos);
        // 4. 显示面板
        tooltipPanel.SetActive(true);
    }

    // 对外提供：隐藏描述面板
    public void HideTooltip()
    {
        tooltipPanel.SetActive(false);
        // 清空文本（可选，避免残留）
        descText.text = "";
    }

    // 核心：调整面板位置（避免超出屏幕，适配不同分辨率）
    private void SetTooltipPosition(Vector2 targetPos)
    {
        // 1. 获取面板和背景的实际尺寸
        RectTransform panelRect = tooltipPanel.GetComponent<RectTransform>();
        Vector2 panelSize = panelRect.rect.size; // 面板实际尺寸（100x100）

        // 2. 屏幕边界（0~屏幕宽/0~屏幕高）
        float screenMinX = 0;
        float screenMaxX = Screen.width; // 2560
        float screenMinY = 0;
        float screenMaxY = Screen.height; // 1440

        // 3. 基础偏移（鼠标右侧20，下方20，避免遮挡）
        Vector2 offset = new Vector2(20, -20);
        Vector2 finalPos = targetPos + offset;

        // ========== 边界检测：强制限制在屏幕内 ==========
        // X轴：左不超0，右不超2560-100=2460
        finalPos.x = Mathf.Clamp(finalPos.x, screenMinX, screenMaxX - panelSize.x);
        // Y轴：UI锚点是Top Left，需转换屏幕Y轴（屏幕Y向下为正，UI Y向上为正）
        // 转换公式：UI_Y = 屏幕高 - 屏幕Y
        float uiY = screenMaxY - finalPos.y;
        // 限制Y轴：下不超0+100=100，上不超1440
        uiY = Mathf.Clamp(uiY, screenMinY + panelSize.y, screenMaxY);
        // 转回屏幕Y轴
        //finalPos.y = screenMaxY - uiY;
        finalPos.y = -uiY;
        // 4. 应用位置
        panelRect.anchoredPosition = finalPos;

        // 打印修复后的日志（验证）
        Debug.Log($"修复后→面板尺寸：{panelSize} | 目标位置（鼠标）：{targetPos} | 最终位置：{finalPos} | 屏幕尺寸：{screenMaxX}x{screenMaxY}");
    }
}