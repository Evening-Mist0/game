using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 适用于 UGUI 的气泡提示，背景为 Image，文字为 TextMeshProUGUI
/// </summary>
public class DescriptionBubbleUI : MonoBehaviour
{
    public TextMeshProUGUI textUI;      // UI 文本
    public Image bgImage;               // UI 背景图片（需设置为 Sliced 模式）
    public Vector2 padding = new Vector2(20f, 15f);   // 背景比文字大出的像素量
    public Vector2 centerOffset;        // 背景相对于文本的位置偏移（UI 坐标）

    private RectTransform _textRect;
    private RectTransform _bgRect;

    void Awake()
    {
        if (textUI != null)
            _textRect = textUI.GetComponent<RectTransform>();
        if (bgImage != null)
            _bgRect = bgImage.GetComponent<RectTransform>();
    }

    /// <summary>
    /// 更新气泡内容并自动调整背景大小
    /// </summary>
    public void UpdateDescibe(string content)
    {
        if (textUI == null || bgImage == null) return;

        textUI.text = content;
        // 强制刷新文本布局，确保获取正确尺寸
        textUI.ForceMeshUpdate(true, true);
        StartCoroutine(AdjustAfterLayout());
    }

    private IEnumerator AdjustAfterLayout()
    {
        // 等待一帧，确保文本布局已更新
        yield return null;
        // 对于 UGUI，额外强制刷新一次画布
        Canvas.ForceUpdateCanvases();

        // 获取文本的实际大小（单位：像素）
        Vector2 textSize = new Vector2(
            textUI.preferredWidth,
            textUI.preferredHeight
        );

        // 目标背景大小 = 文本大小 + 内边距
        Vector2 targetBgSize = new Vector2(
            textSize.x + padding.x,
            textSize.y + padding.y
        );

        // 设置背景 RectTransform 的大小
        if (_bgRect != null)
        {
            _bgRect.sizeDelta = targetBgSize;
        }

        // 设置背景位置（相对文本偏移）
        if (_bgRect != null && _textRect != null)
        {
            // 注意：这里假设两个 UI 元素位于同一父级下，且锚点均为中心
            // 如果需要更精确的坐标转换，可使用 anchoredPosition
            _bgRect.anchoredPosition = (Vector2)_textRect.anchoredPosition + centerOffset;
        }

        Debug.Log($"文字大小: {textSize}, 背景大小: {targetBgSize}");
    }

    /// <summary>
    /// 获取背景顶部到中心点的 Y 轴距离（用于箭头定位等）
    /// </summary>
    public float GetTopToCenterYOffset()
    {
        if (_bgRect == null)
        {
            Debug.LogError("背景 Image 的 RectTransform 未赋值！");
            return 0f;
        }
        return _bgRect.sizeDelta.y / 2f;
    }

    [ContextMenu("测试自适应")]
    private void Test()
    {
        if (textUI != null)
            UpdateDescibe(textUI.text);
    }
}