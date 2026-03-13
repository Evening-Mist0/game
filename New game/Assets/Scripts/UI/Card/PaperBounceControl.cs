
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 仅用于鼠标与卡牌的交互效果显示
/// </summary>
public class PaperBounceControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform rect;
    private Vector2 originalPos;
    private Vector3 originalScale;
    private Coroutine animCoroutine;
    private Coroutine returnCoroutine;
    private Image imgCard;
    public BaseCard myCard;
    //引用当前卡牌的事件触发器
    private CardEventTrigger _cardEventBinder;

    // 状态管理
    private bool isLocked = false;           // 是否被点击锁定
    private bool isPointerOver = false;      // 鼠标是否在UI上
    private bool isLeftMouseButtonClicking;  // 左键是否被点击选中
    private bool isRightMouseButtonClicking; // 右键是否被点击选中
    private bool isSelected;                  // 卡牌是否被选中
    private bool isReturning = false;         // 是否正在返回动画中

    // 手动定义弹动曲线
    private AnimationCurve bounceCurve = new AnimationCurve(
        new Keyframe(0, 0, 0, 5),    // 0秒：初始状态，切线陡
        new Keyframe(0.6f, 1, 0, -3) // 0.6秒：目标状态，切线向下（回弹）
    );

    // 返回动画曲线（平滑减速）
    private AnimationCurve returnCurve = new AnimationCurve(
        new Keyframe(0, 0, 2, 2),    // 开始：快速启动
        new Keyframe(1, 1, 0, 0)      // 结束：平滑停止
    );

    [Header("弹开设置")]
    [Tooltip("弹开的水平偏移量")]
    public float bounceXOffset = 50f;
    [Tooltip("弹开的垂直偏移量")]
    public float bounceYOffset = 30f;
    [Tooltip("弹开的缩放比例增量")]
    public float bounceScaleIncrement = 0.1f;

    [Header("漂浮设置")]
    [Tooltip("漂浮的垂直浮动幅度")]
    public float floatVerticalAmplitude = 5f;
    [Tooltip("漂浮速度")]
    public float floatSpeed = 2f;

    [Header("返回动画设置")]
    [Tooltip("返回动画持续时间")]
    public float returnDuration = 0.4f;

    [Tooltip("是否开启浮动动画（效果不好暂时不用）")]
    public bool isOpenFloatEffect;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
        originalScale = rect.localScale;

        imgCard = this.GetComponent<Image>();
        if (imgCard == null)
            Debug.LogError("没有找到Image组件");

        myCard = this.GetComponent<BaseCard>();
        if (myCard == null)
            Debug.LogError("没有找到BaseCard组件");
    }

    private void OnEnable()
    {
        originalPos = rect.anchoredPosition;
        originalScale = rect.localScale;

        //注册监听事件
        //EventCenter.Instance.AddEventListener(E_EventType)
    }

    private void OnDisable()
    {
        // 组件禁用时确保恢复到原始状态
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);
        rect.anchoredPosition = originalPos;
        rect.localScale = originalScale;
    }

    private void Update()
    {
        // 只有在左键选中状态下，且不是返回动画中，且右键按下时才执行
        if (isLeftMouseButtonClicking && Input.GetMouseButtonDown(1) && !isReturning)
        {
            ForceUnlockAndReturn();
        }
    }

    /// <summary>
    /// 强制取消锁定并返回原位（立即执行返回动画）
    /// </summary>
    private void ForceUnlockAndReturn()
    {
        if (!isLocked || isReturning) return; // 如果已经在返回中，不重复执行

        isReturning = true; // 标记开始返回动画
        isLocked = false;
        isSelected = false;
        isPointerOver = false;
        isLeftMouseButtonClicking = false;
        isRightMouseButtonClicking = false;
        imgCard.color = Color.white;

        // 停止所有正在运行的协程
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        // 直接启动返回动画，无论鼠标在哪里
        returnCoroutine = StartCoroutine(SmoothReturn());

        EventCenter.Instance.EventTrigger<bool>(E_EventType.OnCardClick1_Bool, isSelected);
        EventCenter.Instance.EventTrigger<bool>(E_EventType.OnCardClick0_Bool, isSelected);
        EventCenter.Instance.EventTrigger<BaseCard>(E_EventType.CancelSelected, myCard);
        EventCenter.Instance.EventTrigger<BaseCard>(E_EventType.OnCardClick1_BaseCard, myCard);
        EventCenter.Instance.EventTrigger(E_EventType.OnCardClick1);

        Debug.Log("<color=yellow>全局右键点击：锁定已取消，返回原位</color>");
    }

    // 鼠标进入按钮时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerOver = true;

        // 如果处于返回动画中，不播放动画
        if (isReturning)
        {
            Debug.Log("<color=cyan>处于返回动画中，鼠标进入不播放动画</color>");
            return;
        }

        // 如果处于锁定状态，不播放动画（保持原样）
        if (isLocked)
        {
            Debug.Log("<color=cyan>处于锁定状态，鼠标进入不播放动画</color>");
            return;
        }

        // 停止所有正在运行的协程
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        animCoroutine = StartCoroutine(PlayBounceAndFloat());
    }

    // 鼠标离开按钮时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;

        // 如果处于返回动画中，不做任何操作
        if (isReturning)
        {
            Debug.Log("<color=cyan>处于返回动画中，鼠标离开不操作</color>");
            return;
        }

        // 如果处于锁定状态，鼠标离开时不做任何操作
        if (isLocked)
        {
            Debug.Log("<color=cyan>处于锁定状态，鼠标离开不恢复</color>");
            return;
        }

        // 非锁定状态，正常执行返回动画
        // 停止浮动动画
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }

        // 启动平滑返回动画
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(SmoothReturn());
    }

    IEnumerator PlayBounceAndFloat()
    {
        float bounceDuration = 0.6f;
        float time = 0;

        // 弹开动画
        while (time < bounceDuration)
        {
            float normalizedTime = time / bounceDuration;
            float t = bounceCurve.Evaluate(normalizedTime);

            rect.anchoredPosition = originalPos + new Vector2(bounceXOffset * t, bounceYOffset * t);
            rect.localScale = originalScale * (1 + bounceScaleIncrement * t);

            time += Time.deltaTime;
            yield return null;
        }

        // 确保弹性动画精确到达最终位置
        Vector2 bounceEndPos = originalPos + new Vector2(bounceXOffset, bounceYOffset);
        Vector3 bounceEndScale = originalScale * (1 + bounceScaleIncrement);

        rect.anchoredPosition = bounceEndPos;
        rect.localScale = bounceEndScale;

        if (isOpenFloatEffect)
        {
            // 漂浮动画
            float elapsedTime = 0f;
            while (true)
            {
                // 使用正弦波在Y轴上做浮动，X轴保持弹性后的位置不变
                float floatOffset = Mathf.Sin(elapsedTime * floatSpeed) * floatVerticalAmplitude;
                rect.anchoredPosition = new Vector2(bounceEndPos.x, bounceEndPos.y + floatOffset);

                elapsedTime += Time.deltaTime;

                // 检查鼠标是否还在UI上，如果离开了就退出浮动动画
                if (!isPointerOver || isLocked || isReturning)
                {
                    // 退出浮动动画，开始返回
                    StartCoroutine(SmoothReturn());
                    yield break;
                }

                yield return null;
            }
        }
    }

    IEnumerator SmoothReturn()
    {
        isReturning = true; // 标记开始返回动画

        // 记录开始位置和缩放（当前值）
        Vector2 startPos = rect.anchoredPosition;
        Vector3 startScale = rect.localScale;

        float time = 0;

        while (time < returnDuration)
        {
            float normalizedTime = time / returnDuration;
            float t = returnCurve.Evaluate(normalizedTime);

            rect.anchoredPosition = Vector2.Lerp(startPos, originalPos, t);
            rect.localScale = Vector3.Lerp(startScale, originalScale, t);

            time += Time.deltaTime;
            yield return null;
        }

        // 确保精确回到原位
        rect.anchoredPosition = originalPos;
        rect.localScale = originalScale;

        returnCoroutine = null;
        isReturning = false; // 返回动画结束

        // 等待一帧，确保鼠标状态更新
        yield return null;

        // 返回动画结束后，检查鼠标是否还在UI上
        if (!isLocked && IsMouseOverUI())
        {
            Debug.Log("<color=green>返回动画结束，鼠标仍在UI上，重新触发悬停效果</color>");
            isPointerOver = true;
            animCoroutine = StartCoroutine(PlayBounceAndFloat());
        }
    }

    // 辅助方法：检查鼠标是否在当前UI上
    private bool IsMouseOverUI()
    {
        if (EventSystem.current == null)
            return false;

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (RaycastResult result in results)
        {
            if (result.gameObject == this.gameObject)
                return true;
        }

        return false;
    }

    // 公共方法：手动取消锁定（可用于其他脚本调用）
    public void CancelLock()
    {
        if (isLocked && !isReturning)
        {
            ForceUnlockAndReturn();
        }
    }

    // 公共方法：检查是否处于锁定状态
    public bool IsLocked()
    {
        return isLocked;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // 如果在返回动画中，禁止任何点击
        if (isReturning)
        {
            Debug.Log("<color=red>返回动画中，禁止点击</color>");
            return;
        }

        // 只有E_LevelState.PlayerTurn_CardOperate的状态鼠标点击UI才会响应
        if (LevelStepMgr.Instance.machine.NowStateType == E_LevelState.PlayerTurn_CardOperate)
        {
            Debug.Log("<color=lime>点击事件触发</color>");

            if (eventData.pointerId == -1)//鼠标左键点击
            {
                // 如果是右键选中状态下，禁止左键点击
                if (isRightMouseButtonClicking)
                {
                    Debug.Log("<color=orange>右键选中状态下，禁止左键点击</color>");
                    return;
                }

                #region 高亮相关
                if (!isLocked && !isLeftMouseButtonClicking)
                {
                    isLocked = true;
                    isSelected = true;
                    isLeftMouseButtonClicking = true;
                    isRightMouseButtonClicking = false;
                    imgCard.color = Color.red;
                    Debug.Log("<color=red>左键选中，UI已锁定</color>");
                    EventCenter.Instance.EventTrigger<BaseCard>(E_EventType.OnCardClick0_BaseCard, myCard);
                    EventCenter.Instance.EventTrigger<bool>(E_EventType.OnCardClick0_Bool, isSelected);
                }
                #endregion

                #region 绘线相关            
                if (isLeftMouseButtonClicking)
                {
                    // 1. 获取卡牌的RectTransform
                    RectTransform cardRect = imgCard.rectTransform;
                    if (cardRect.pivot != new Vector2(0.5f, 0.5f))
                    {
                        Debug.LogWarning("卡牌Pivot不是(0.5,0.5)，强制修正为中心！");
                        cardRect.pivot = new Vector2(0.5f, 0.5f);
                    }

                    Vector2 localCenter = Vector2.zero;
                    Vector3 uiWorldCenter = cardRect.TransformPoint(localCenter);
                    Camera uiCamera = eventData.pressEventCamera;
                    Vector2 cardCenterScreen = RectTransformUtility.WorldToScreenPoint(uiCamera, uiWorldCenter);

                    float zDistance = Camera.main.orthographic ? 0 : Camera.main.nearClipPlane + 0.1f;
                    Vector3 startPos = Camera.main.ScreenToWorldPoint(new Vector3(
                        cardCenterScreen.x,
                        cardCenterScreen.y,
                        zDistance
                    ));
                    startPos.z = 0;

                    EventCenter.Instance.EventTrigger<Vector3>(E_EventType.OnCardClick0_Vector3, startPos);
                }
                #endregion
            }
            else if (eventData.pointerId == -2)//鼠标右键点击
            {
                // 如果是左键选中状态下，禁止右键点击
                if (isLeftMouseButtonClicking)
                {
                    Debug.Log("<color=orange>左键选中状态下，禁止右键点击</color>");
                    return;
                }

                #region 高亮相关
                Debug.Log("点击右键" + "isLocked=" + isLocked + " isLeftMouseButtonClicking=" + isLeftMouseButtonClicking);

                // 情况一：第一次右键选中
                if ((!isLocked) && (!isLeftMouseButtonClicking))
                {
                    isLocked = true;
                    isSelected = true;
                    isRightMouseButtonClicking = true;
                    isLeftMouseButtonClicking = false;
                    imgCard.color = Color.yellow;
                    Debug.Log("<color=yellow>右键选中，UI已锁定</color>");
                    EventCenter.Instance.EventTrigger<BaseCard>(E_EventType.OnCardClick1_BaseCard, myCard);
                    EventCenter.Instance.EventTrigger<bool>(E_EventType.OnCardClick1_Bool, isSelected);
                }
                // 情况二：第二次右键取消选中
                else
                {
                    ForceUnlockAndReturn();
                }
                #endregion
            }
        }
    }
}





