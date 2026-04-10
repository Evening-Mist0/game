using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

/// <summary>
/// 全局描述面板，用于显示物品详情（悬浮气泡）
/// </summary>
public class DescriptionPanel : MonoBehaviour
{
    public static DescriptionPanel Instance { get; private set; }

    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descText;
    //[SerializeField] private Image background;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("偏移设置")]
    [SerializeField] private Vector2 offset = new Vector2(15, -15); // 相对于鼠标位置的偏移

    private RectTransform rectTransform;
    private Canvas parentCanvas;

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
        parentCanvas = GetComponentInParent<Canvas>();
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        Hide();
    }

    /// <summary>
    /// 显示描述面板
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="description">描述</param>
    /// <param name="position">基准位置（通常为鼠标屏幕坐标）</param>
    public void Show(string title, string description, Vector2 position)
    {
        if (titleText != null) titleText.text = title;
        if (descText != null) descText.text = description;

        // 调整面板位置，考虑边界
        Vector2 targetPos = position + offset;

        rectTransform.position = targetPos;

        // 显示面板
        canvasGroup.alpha = 1;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 隐藏面板
    /// </summary>
    public void Hide()
    {
        gameObject.SetActive(false);
        canvasGroup.alpha = 0;
    }

}
