using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BookIconControl : MonoBehaviour
{
    public Image imgBook;
    public E_BookType myType;
    public string description;

    [Header("长按设置")]
    public float longPressDuration = 0.01f;   // 长按判定时间（秒），恢复合理值

    [Header("气泡设置")]
    public Canvas targetCanvas;   // 拖拽你的 Canvas 对象进来（或者通过代码查找）

    public Vector3 tipOffsetPos = new Vector3(0, 0, 0);
    private GameObject currentBubble;
    private Coroutine longPressCoroutine;
    private bool isPointerDown = false;

    private void Awake()
    {
        // 如果没有手动指定 Canvas，尝试获取场景中第一个激活的 Canvas
        if (targetCanvas == null)
            targetCanvas = UIMgr.Instance.canvas;
    }

    private void OnDisable()
    {
        HideBubble();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("鼠标按下");
        ShowBubble();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("鼠标抬起");
        isPointerDown = false;
        HideBubble();   // 恢复隐藏
    }


    private void ShowBubble()
    {
        if (currentBubble != null) HideBubble();
        if (targetCanvas == null) { Debug.LogError("没有找到 Canvas，无法显示气泡！"); return; }

        currentBubble = PoolMgr.Instance.GetObj("UI/DescriptionBubbleUI");
        if (currentBubble == null) return;

        DescriptionBubbleUI bubble = currentBubble.GetComponent<DescriptionBubbleUI>();
        if (bubble == null) { Debug.LogError("DescriptionBubbleUI 组件未找到！"); return; }

        // 确保气泡一开始不可见（但依然参与布局计算）
        CanvasGroup cg = bubble.GetComponent<CanvasGroup>();
        if (cg == null) cg = bubble.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        bubble.transform.SetParent(UIMgr.Instance.canvas.transform, false);
        bubble.UpdateDescibe(description);

        StartCoroutine(SetBubblePositionAfterLayout(bubble, cg));
    }

    private IEnumerator SetBubblePositionAfterLayout(DescriptionBubbleUI bubble, CanvasGroup cg)
    {
        yield return null;
        yield return null; // 等待两帧确保布局完成

        if (currentBubble == null || bubble == null) yield break;

        Vector3 pos = this.transform.position + new Vector3(bubble.GetLeftEdgeToCenterDistance(), 0, 0) + tipOffsetPos;
        bubble.transform.position = pos;

        // 位置设置完成，恢复可见
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    private void HideBubble()
    {
        if (currentBubble != null)
        {
            PoolMgr.Instance.PushObj(currentBubble.gameObject);
            currentBubble = null;
        }
    }
}
