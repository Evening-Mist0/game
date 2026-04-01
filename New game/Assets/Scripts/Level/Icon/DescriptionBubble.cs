//using TMPro;
//using UnityEngine;

//public class DescriptionBubble : MonoBehaviour
//{
//    [Header("拖入你的世界空间 TMP")]
//    public TextMeshPro textDescribe;

//    [Header("拖入你的背景 SpriteRenderer")]
//    public SpriteRenderer bgSprite;

//    [Header("内边距 (世界单位)")]
//    public Vector2 padding = new Vector2(0.2f, 0.15f);
//    [Header("背景中心相对于文字中心的偏移量")]
//    public Vector3 centerOffset;

//    public void UpdateDescibe(string content)
//    {
//        if (textDescribe == null || bgSprite == null)
//        {
//            Debug.LogError("Missing reference!");
//            return;
//        }

//        // 1. 更新文字内容
//        textDescribe.text = content;

//        // 2. 强制立即重建网格 (关键!)
//        textDescribe.ForceMeshUpdate(true, true);

//        // 3. 获取文字在世界空间中的真实包围盒
//        Bounds textBounds = textDescribe.bounds;
//        Vector2 textSize = textBounds.size;

//        // 调试：看看文字尺寸是否正确变化
//        Debug.Log($"文字尺寸: {textSize}");

//        // 4. 计算背景需要的尺寸
//        Vector2 targetBgSize = new Vector2(
//            textSize.x + padding.x,
//            textSize.y + padding.y
//        );

//        // 5. 直接设置背景大小 (适用于 Simple / Sliced 模式)
//        bgSprite.size = targetBgSize;

//        // 6. 让背景中心对准文字中心
//        bgSprite.transform.position = textDescribe.transform.position;
//        bgSprite.transform.position += centerOffset;

//        Debug.Log($"背景尺寸已设置为: {targetBgSize}");
//    }

//    [ContextMenu("测试自适应")]
//    private void Test()
//    {
//        UpdateDescibe(textDescribe.text);
//    }
//}

using System.Collections;
using TMPro;
using UnityEngine;

public class DescriptionBubble : MonoBehaviour
{
    public TextMeshPro textDescribe;
    public SpriteRenderer bgSprite;
    public Vector2 padding = new Vector2(0.2f, 0.15f);
    public Vector3 centerOffset;

    public void UpdateDescibe(string content)
    {
        if (textDescribe == null || bgSprite == null) return;

        textDescribe.text = content;
        textDescribe.ForceMeshUpdate(true, true);
        StartCoroutine(AdjustAfterLayout());
    }

    private IEnumerator AdjustAfterLayout()
    {
        yield return null; // 等待网格完全更新


        //使用 bounds（多行时更准）
         Bounds textBounds = textDescribe.bounds;
        Vector2 textSize = textBounds.size;

        Vector2 targetBgSize = new Vector2(
            textSize.x + padding.x,
            textSize.y + padding.y
        );

        // 将世界尺寸转换为背景的本地尺寸（处理父物体缩放）
        Transform parent = bgSprite.transform.parent;
        Vector3 worldSize = new Vector3(targetBgSize.x, targetBgSize.y, 1);
        Vector3 localSize = parent != null ? parent.InverseTransformVector(worldSize) : worldSize;
        bgSprite.size = new Vector2(localSize.x, localSize.y);

        // 让背景中心对准文字包围盒中心（而非物体中心）
        // 如果用 bounds：bgSprite.transform.position = textBounds.center + centerOffset;
        bgSprite.transform.position = textDescribe.transform.position + centerOffset;

        Debug.Log($"文字尺寸: {textSize}, 背景尺寸: {targetBgSize}");
    }

    [ContextMenu("测试自适应")]
    private void Test()
    {
        UpdateDescibe(textDescribe.text);
    }
}

