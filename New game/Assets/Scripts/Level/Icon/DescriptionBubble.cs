
using System.Collections;
using TMPro;
using UnityEngine;

// 去掉自动绑定，让你自由拖拽
public class DescriptionBubble : MonoBehaviour
{
    public TextMeshPro textMesh;       // 3D文本
    public SpriteRenderer bgRenderer;  // 你的气泡背景Sprite
    public Vector2 padding = new Vector2(0.2f, 0.15f);//背景与文字的面积比例
    public Vector3 centerOffset;       // 背景与文字位置偏移

    private float _pixelsPerUnit;

    void Awake()
    {
        if (bgRenderer != null && bgRenderer.sprite != null)
        {
            _pixelsPerUnit = bgRenderer.sprite.pixelsPerUnit;
        }
    }

    public void UpdateDescibe(string content)
    {
        if (textMesh == null || bgRenderer == null) return;

        textMesh.text = content;
        textMesh.ForceMeshUpdate(true, true);
        StartCoroutine(AdjustAfterLayout());
    }

    private IEnumerator AdjustAfterLayout()
    {
        yield return null;

        // 3D文本 正确尺寸
        Bounds textBounds = textMesh.textBounds;
        Vector2 textSize = textBounds.size;

        // 目标背景大小
        Vector2 targetBgSize = new Vector2(
            textSize.x + padding.x,
            textSize.y + padding.y
        );

        // 你已经设置 Sliced，直接用 size
        bgRenderer.size = targetBgSize;

        // 对齐位置
        bgRenderer.transform.position = textMesh.transform.position + centerOffset;

        Debug.Log($"文字大小: {textSize}, 背景大小: {targetBgSize}");
    }

    public float GetTopToCenterYOffset()
    {
        if (bgRenderer == null)
        {
            Debug.LogError("背景SpriteRenderer未赋值！");
            return 0f;
        }

        return bgRenderer.size.y / 2f;
    }

    [ContextMenu("测试自适应")]
    private void Test()
    {
        UpdateDescibe(textMesh.text);
    }
}