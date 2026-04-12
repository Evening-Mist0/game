using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DescriptionPanel : MonoBehaviour
{
    public static DescriptionPanel Instance { get; private set; }

    [Header("组件引用")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("自适应设置")]
    [SerializeField] private Vector2 padding = new Vector2(20f, 15f);
    [SerializeField] private Vector2 offset = new Vector2(25f, -25f);

    private RectTransform rectTransform;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        rectTransform = GetComponent<RectTransform>();
        Hide();
    }

    public void Show(string title, string description, Vector2 mouseScreenPosition)
    {
        // 组合文本
        string finalText = string.IsNullOrEmpty(title) ? description : $"<b>{title}</b>\n{description}";
        descText.text = finalText;
        descText.ForceMeshUpdate();

        // 获取文本尺寸
        Vector2 textSize = descText.GetRenderedValues(false);
        
        // 调整背景大小
        if (backgroundImage != null)
        {
            RectTransform bgRect = backgroundImage.GetComponent<RectTransform>();
            bgRect.sizeDelta = new Vector2(textSize.x + padding.x, textSize.y + padding.y);
        }

        // 强制刷新布局，确保获取正确的面板大小
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        Vector2 panelSize = rectTransform.sizeDelta;

        // 计算目标位置（屏幕坐标 + 偏移）
        Vector2 targetPos = mouseScreenPosition + offset;
        targetPos = ClampToScreen(targetPos, panelSize);

        // 对于 Screen Space - Overlay 模式，直接设置屏幕坐标
        rectTransform.position = targetPos;

        canvasGroup.alpha = 1f;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 0f;
    }

    private Vector2 ClampToScreen(Vector2 position, Vector2 panelSize)
    {
        float minX = panelSize.x / 2f;
        float maxX = Screen.width - panelSize.x / 2f;
        float minY = panelSize.y / 2f;
        float maxY = Screen.height - panelSize.y / 2f;
        position.x = Mathf.Clamp(position.x, minX, maxX);
        position.y = Mathf.Clamp(position.y, minY, maxY);
        return position;
    }
}