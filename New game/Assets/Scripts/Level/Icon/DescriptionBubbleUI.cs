using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 适用于 UGUI 的气泡提示，背景为 Image，文字为 TextMeshProUGUI
/// </summary>
public class DescriptionBubbleUI : MonoBehaviour
{
    public TMP_Text text;               // 文本
    public Image bgRenderer;            // 气泡背景 Image
    public Vector2 padding = new Vector2(0.2f, 0.15f); // 背景比文字多出的边距
    public Vector3 centerOffset;        // 背景相对于文字的位置偏移（局部坐标）

    private void Awake()
    {
        EventCenter.Instance.AddEventListener(E_EventType.CardPlayingPanel_ClickOverTurn,ForceHideMe);

    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener(E_EventType.CardPlayingPanel_ClickOverTurn,ForceHideMe);
    }

    private void OnEnable()
    {
        this.transform.localScale = Vector3.one;
    }

  

    public void UpdateDescibe(string content)
    {
        if (text == null || bgRenderer == null) return;

        text.text = content;
        text.ForceMeshUpdate(true, true);
        StartCoroutine(AdjustAfterLayout());
    }

    private IEnumerator AdjustAfterLayout()
    {
        yield return null; // 等待一帧，让文本布局完成

        // 获取文本的包围盒尺寸
        Bounds textBounds = text.textBounds;
        Vector2 textSize = textBounds.size;

        // 计算背景应该的大小（边距是绝对值，不是比例）
        Vector2 targetBgSize = new Vector2(
            textSize.x + padding.x,
            textSize.y + padding.y
        );

        // Image 用 RectTransform.sizeDelta 修改尺寸（适合 Sliced 模式）
        RectTransform bgRect = bgRenderer.rectTransform;
        bgRect.sizeDelta = targetBgSize;

        // 设置背景位置：相对于文本物体的局部偏移（保持父子关系清晰）
        // 注意：文本和背景应该在同一个 Canvas 下，或者至少同一个坐标系中
        bgRect.localPosition = text.rectTransform.localPosition + centerOffset;

        Debug.Log($"文字大小: {textSize}, 背景大小: {targetBgSize}");
    }

    // 获取背景顶部到中心点的 Y 偏移（可用于箭头定位等）
    public float GetTopToCenterYOffset()
    {
        if (bgRenderer == null)
        {
            Debug.LogError("背景 Image 未赋值！");
            return 0f;
        }
        return bgRenderer.rectTransform.sizeDelta.y / 2f;
    }

    [ContextMenu("测试自适应")]
    private void Test()
    {
        UpdateDescibe(text.text);
    }

    public float GetLeftEdgeToCenterDistance()
    {
        if (bgRenderer == null) return 0f;
        return bgRenderer.rectTransform.rect.width * 0.5f;
    }

    /// <summary>
    /// 强制隐藏自己
    /// </summary>
    private void ForceHideMe()
    {
        PoolMgr.Instance.PushObj(this.gameObject);
    }
}