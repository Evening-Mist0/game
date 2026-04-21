using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TreasureIconControl : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public Image imgTreasure;
    public Image imgNumber;
    public E_TreasureType myType;
    public string description;
    public bool isNumberImgVisible;

    [Header("长按设置")]
    public float longPressDuration = 0.5f;   // 长按判定时间（秒），恢复合理值

    [Header("气泡设置")]
    public Canvas targetCanvas;   // 拖拽你的 Canvas 对象进来（或者通过代码查找）

    private Vector3 tipOffsetPos = new Vector3(2.5f, 0, 0);
    private GameObject currentBubble;
    private Coroutine longPressCoroutine;
    private bool isPointerDown = false;

    private void Awake()
    {
        // 如果没有手动指定 Canvas，尝试获取场景中第一个激活的 Canvas
        if (targetCanvas == null)
            targetCanvas = UIMgr.Instance.canvas;

            imgNumber.gameObject.SetActive(isNumberImgVisible);
    }

    private void OnDisable()
    {
        CancelLongPress();
        HideBubble();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("鼠标按下");
        isPointerDown = true;
        if (longPressCoroutine != null)
            StopCoroutine(longPressCoroutine);
        longPressCoroutine = StartCoroutine(StartLongPressTimer());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("鼠标抬起");
        isPointerDown = false;
        CancelLongPress();
        //HideBubble();   // 恢复隐藏
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isPointerDown)
        {
            isPointerDown = false;
            CancelLongPress();
            //HideBubble();   // 恢复隐藏
        }
    }

    private IEnumerator StartLongPressTimer()
    {
        yield return new WaitForSeconds(longPressDuration);
        if (isPointerDown)
        {
            ShowBubble();
        }
    }

    private void ShowBubble()
    {
        if (currentBubble != null)
            HideBubble();

        if (targetCanvas == null)
        {
            Debug.LogError("没有找到 Canvas，无法显示气泡！");
            return;
        }

        currentBubble = PoolMgr.Instance.GetObj("UI/DescriptionBubbleUI");
        if (currentBubble == null) return;

        // 1. 父物体必须是 Canvas（否则 UI 不渲染）
        currentBubble.transform.SetParent(targetCanvas.transform, false);

        DescriptionBubbleUI bubble = currentBubble.GetComponent<DescriptionBubbleUI>();
        if (bubble == null)
        {
            Debug.LogError("DescriptionBubbleUI 组件未找到！");
            return;
        }

        RectTransform bubbleRect = currentBubble.GetComponent<RectTransform>();
        if (bubbleRect == null) return;

        // 2. 获取图标的世界坐标
        Vector3 targetWorldPos = transform.position;

        // 3. 根据 Canvas 的摄像机模式，将世界坐标转换为 UI 坐标
        Camera canvasCam = targetCanvas.worldCamera;
        if (canvasCam == null) canvasCam = Camera.main;

        // 将世界坐标转为屏幕坐标
        Vector3 screenPos = canvasCam.WorldToScreenPoint(targetWorldPos);

        // 将屏幕坐标转为 Canvas 下的世界坐标（ScreenSpaceCamera 模式专用）
        // 注意：需要给 Z 赋值，表示距离摄像机的距离（这里用 Canvas 平面距离，一般设为 canvasCam.nearClipPlane + 1）
        screenPos.z = canvasCam.nearClipPlane + 1f;
        Vector3 worldPosInCanvas = canvasCam.ScreenToWorldPoint(screenPos);

        // 4. 设置气泡的世界坐标（此时气泡在 Canvas 下，位置就是 worldPosInCanvas）
        bubbleRect.position = worldPosInCanvas;

        // 可选：重置锚点为中心，避免偏移
        bubbleRect.anchorMin = new Vector2(0.5f, 0.5f);
        bubbleRect.anchorMax = new Vector2(0.5f, 0.5f);

        // 5. 更新文字内容（内部会调整背景大小）
        bubble.UpdateDescibe(description);
    }

    private void HideBubble()
    {
        if (currentBubble != null)
        {
            PoolMgr.Instance.PushObj(currentBubble);
            currentBubble = null;
        }
    }

    private void CancelLongPress()
    {
        if (longPressCoroutine != null)
        {
            StopCoroutine(longPressCoroutine);
            longPressCoroutine = null;
        }
    }

    public void UpdateMyIconCount(int count)
    {
        if (!isNumberImgVisible) return;
        if (count < 0 || count > 9) return;
        string path = "Number/" + count.ToString();
        imgNumber.sprite = Resources.Load<Sprite>(path);
    }
}