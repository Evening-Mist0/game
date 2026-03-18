
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 卡牌交互控制：处理卡牌UI的悬停、点击、动画效果
/// </summary>
public class CardEffectControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform rect;
    private Vector2 originalAnchoredPos; // Grid布局下的原始锚点位置
    private Vector3 originalScale;
    private Coroutine animCoroutine;
    private Coroutine returnCoroutine;
    private Animator animator;
    private GridLayoutCallback gridCallBack;

    // 卡牌事件触发器
    private CardEventTrigger _cardEventTrigger;
    public BaseCard myCard;
    private Image imgCard;
    private Camera uiCamera;//渲染该卡牌的UI相机

    // 状态控制
    private bool isLocked = false;           // 是否被锁定（选中状态）
    private bool isPointerOver = false;      // 鼠标是否在UI上
    private bool isLeftMouseButtonClicking;  // 左键是否被选中
    private bool isRightMouseButtonClicking; // 右键是否被选中
    private bool isSelected;                  // 是否被选中
    private bool isReturning = false;         // 是否正在返回原位
    private bool isLayoutInitialized = false; // Grid布局是否初始化完成


    // 弹跳动画曲线
    private AnimationCurve bounceCurve = new AnimationCurve(
        new Keyframe(0, 0, 0, 5),    // 0秒：初始状态，斜率5
        new Keyframe(0.6f, 1, 0, -3) // 0.6秒：目标状态，斜率-3，回弹
    );

    // 返回动画曲线：平滑减速
    private AnimationCurve returnCurve = new AnimationCurve(
        new Keyframe(0, 0, 2, 2),    // 初始：快速移动
        new Keyframe(1, 1, 0, 0)      // 结束：平滑停止
    );

    [Header("弹跳参数")]
    [Tooltip("弹跳水平偏移量")]
    public float bounceXOffset = 50f;
    [Tooltip("弹跳垂直偏移量")]
    public float bounceYOffset = 30f;
    [Tooltip("弹跳时卡牌缩放增量")]
    public float bounceScaleIncrement = 0.1f;

    [Header("漂浮参数")]
    [Tooltip("漂浮垂直振幅")]
    public float floatVerticalAmplitude = 5f;
    [Tooltip("漂浮速度")]
    public float floatSpeed = 2f;

    [Header("返回参数")]
    [Tooltip("返回原始位置的时长")]
    public float returnDuration = 0.4f;

    [Tooltip("是否开启漂浮效果（悬停时）")]
    public bool isOpenFloatEffect;

    void Awake()
    {
        uiCamera = UIMgr.Instance.UICamera;
            if (uiCamera == null)
            Debug.LogError("没有获取到UI相机");
        rect = GetComponent<RectTransform>();
        originalScale = rect.localScale;

        imgCard = this.GetComponent<Image>();
        if (imgCard == null)
            Debug.LogError($"[卡牌{gameObject.name}]未找到Image组件");

        myCard = this.GetComponent<BaseCard>();
        if (myCard == null)
            Debug.LogError($"[卡牌{gameObject.name}]未找到BaseCard组件");

        _cardEventTrigger = GetComponent<CardEventTrigger>();
        if (_cardEventTrigger == null)
        {
            Debug.LogError($"[卡牌{gameObject.name}] 未找到CardEventBinder组件");
        }

        animator = this.GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError($"[卡牌{gameObject.name}]未找到Animator组件");
        }

       

        // 延迟1帧获取Grid布局后的初始位置（等GridLayoutGroup布局完成）
        StartCoroutine(InitOriginalPosAfterLayout());
    }

 


    void Start()
    {
        gridCallBack = UIMgr.Instance.GetPanel<CardPlayingPanel>().mainCallBack;
        if (gridCallBack == null)
        {
            Debug.Log($"[卡牌{gameObject.name}]未找到CardPlayingPanel的GridLayoutCallback组件");
        }
        else
            gridCallBack.OnGridLayoutUpdated += RefreshOriginalPos;
    }

    /// <summary>
    /// 延迟获取Grid布局后的原始位置
    /// </summary>
    private IEnumerator InitOriginalPosAfterLayout()
    {
        yield return null; // 等待1帧，让GridLayoutGroup完成布局
        originalAnchoredPos = rect.anchoredPosition;
        isLayoutInitialized = true;
        Debug.Log($"[卡牌{gameObject.name}] 初始化Grid布局原始位置: {originalAnchoredPos}");
    }

    private void OnEnable()
    {
        // 启用时重新获取原始位置（防止Grid布局刷新）
        if (isLayoutInitialized)
        {
            originalAnchoredPos = rect.anchoredPosition;
        }
        originalScale = rect.localScale;
    }

    private void OnDisable()
    {
        // 禁用时恢复原始状态
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);

        if (isLayoutInitialized)
        {
            rect.anchoredPosition = originalAnchoredPos;
        }
        rect.localScale = originalScale;

        // 重置状态
        isReturning = false;
        isLocked = false;
        isPointerOver = false;
        isLeftMouseButtonClicking = false;
        isRightMouseButtonClicking = false;
        isSelected = false;
        if (imgCard != null)
        {
            imgCard.color = Color.white;
        }
    }

    private void Update()
    {
        // 未初始化完成时不处理交互
        if (!isLayoutInitialized) return;

        // 左键选中时，右键点击取消
        if (isLeftMouseButtonClicking && Input.GetMouseButtonDown(1) && !isReturning)
        {
            ForceUnlockAndReturn();
            _cardEventTrigger?.TriggerCancelRightSelect();
            _cardEventTrigger?.TriggerCancelLeftSelect();
        }

        // 右键选中时，左键点击取消
        if (isRightMouseButtonClicking && Input.GetMouseButtonDown(0) && !isReturning)
        {
            ForceUnlockAndReturn();
            _cardEventTrigger?.TriggerCancelRightSelect();
            _cardEventTrigger?.TriggerCancelLeftSelect();
        }
    }

    private void OnDestroy()
    {
        if (gridCallBack != null)
        {
            gridCallBack.OnGridLayoutUpdated -= RefreshOriginalPos;
            Debug.Log($"[卡牌{gameObject.name}] 销毁，注销布局更新订阅");
        }
    }

    /// <summary>
    /// 强制解锁并返回原始位置（中断所有动画）
    /// </summary>
    public void ForceUnlockAndReturn()
    {
        if (!isLocked || isReturning || !isLayoutInitialized) return; // 未锁定/返回中/未初始化，不执行

        isReturning = true;
        isLocked = false;
        isSelected = false;
        isPointerOver = false;
        isLeftMouseButtonClicking = false;
        isRightMouseButtonClicking = false;
        if (imgCard != null)
        {
            imgCard.color = Color.white;
        }

        // 停止所有动画协程
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

        // 执行平滑返回
        returnCoroutine = StartCoroutine(SmoothReturn());
        Debug.Log("<color=yellow>强制解锁并返回原始位置</color>");
    }

    // 鼠标进入卡牌时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isLayoutInitialized) return; // 未初始化不处理
        isPointerOver = true;

        // 返回中/锁定状态，不执行动画
        if (isReturning || isLocked)
        {
            Debug.Log(isReturning ? "<color=cyan>正在返回中，忽略悬停动画</color>" : "<color=cyan>卡牌锁定状态，忽略悬停动画</color>");
            return;
        }

        // 停止现有动画
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        animCoroutine = StartCoroutine(PlayBounceAndFloat());
    }

    // 鼠标离开卡牌时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isLayoutInitialized) return; // 未初始化不处理
        isPointerOver = false;

        // 返回中/锁定状态，不执行返回
        if (isReturning || isLocked)
        {
            Debug.Log(isReturning ? "<color=cyan>正在返回中，忽略离开事件</color>" : "<color=cyan>卡牌锁定状态，忽略离开事件</color>");
            return;
        }

        // 停止弹跳/漂浮动画
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }

        // 执行平滑返回
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(SmoothReturn());
    }

    IEnumerator PlayBounceAndFloat()
    {
        float bounceDuration = 0.6f;
        float time = 0;

        // 弹跳阶段
        while (time < bounceDuration)
        {
            float normalizedTime = time / bounceDuration;
            float t = bounceCurve.Evaluate(normalizedTime);

            // 基于Grid原始位置计算偏移
            rect.anchoredPosition = originalAnchoredPos + new Vector2(bounceXOffset * t, bounceYOffset * t);
            rect.localScale = originalScale * (1 + bounceScaleIncrement * t);

            time += Time.deltaTime;
            yield return null;
        }

        // 弹跳结束位置
        Vector2 bounceEndPos = originalAnchoredPos + new Vector2(bounceXOffset, bounceYOffset);
        Vector3 bounceEndScale = originalScale * (1 + bounceScaleIncrement);

        rect.anchoredPosition = bounceEndPos;
        rect.localScale = bounceEndScale;

        // 漂浮阶段（开启时）
        if (isOpenFloatEffect)
        {
            float elapsedTime = 0f;
            while (true)
            {
                // 仅在Y轴漂浮，X轴保持弹跳结束位置
                float floatOffset = Mathf.Sin(elapsedTime * floatSpeed) * floatVerticalAmplitude;
                rect.anchoredPosition = new Vector2(bounceEndPos.x, bounceEndPos.y + floatOffset);

                elapsedTime += Time.deltaTime;

                // 触发返回条件：鼠标离开/卡牌锁定/正在返回
                if (!isPointerOver || isLocked || isReturning)
                {
                    StartCoroutine(SmoothReturn());
                    yield break;
                }

                yield return null;
            }
        }
    }

    IEnumerator SmoothReturn()
    {
        if (!isLayoutInitialized) yield break; // 未初始化直接退出

        isReturning = true;

        // 记录当前位置和缩放（作为返回起点）
        Vector2 startPos = rect.anchoredPosition;
        Vector3 startScale = rect.localScale;

        float time = 0;

        // 平滑插值返回原始位置
        while (time < returnDuration)
        {
            float normalizedTime = time / returnDuration;
            float t = returnCurve.Evaluate(normalizedTime);

            rect.anchoredPosition = Vector2.Lerp(startPos, originalAnchoredPos, t);
            rect.localScale = Vector3.Lerp(startScale, originalScale, t);

            time += Time.deltaTime;
            yield return null;
        }

        // 强制恢复原始位置和缩放（避免插值误差）
        rect.anchoredPosition = originalAnchoredPos;
        rect.localScale = originalScale;

        returnCoroutine = null;
        isReturning = false;

        // 延迟1帧，检测鼠标是否仍在卡牌上（防止快速进出导致的动画闪烁）
        yield return null;

        // 未锁定且鼠标仍在卡牌上，重新触发悬停动画
        if (!isLocked && IsMouseOverUI())
        {
            Debug.Log("<color=green>返回完成后鼠标仍在卡牌上，重新播放悬停动画</color>");
            isPointerOver = true;
            animCoroutine = StartCoroutine(PlayBounceAndFloat());
        }
    }

    // 检测鼠标是否在当前卡牌UI上
    private bool IsMouseOverUI()
    {
        if (EventSystem.current == null || !isLayoutInitialized)
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

    /// <summary>
    /// 取消卡牌锁定状态并返回原始位置
    /// </summary>
    public void CancelLock()
    {
        if (isLocked && !isReturning && isLayoutInitialized)
        {
            ForceUnlockAndReturn();
        }
    }

    /// <summary>
    /// 获取卡牌是否处于锁定状态
    /// </summary>
    public bool IsLocked()
    {
        return isLocked;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isLayoutInitialized || isReturning)
        {
            Debug.Log("<color=red>布局未初始化/正在返回，禁止点击</color>");
            return;
        }

        // 仅在玩家卡牌操作阶段响应点击
        if (LevelStepMgr.Instance?.machine?.NowStateType != E_LevelState.PlayerTurn_CardOperate) return;

        if (eventData.pointerId == -1)// 左键点击
        {
            // 右键选中状态下，禁止左键操作
            if (isRightMouseButtonClicking)
            {
                Debug.Log("<color=orange>右键选中状态，禁止左键点击</color>");
                return;
            }

            // 左键选中逻辑
            if (!isLocked && !isLeftMouseButtonClicking)
            {
                // 取消其他卡牌的选中状态
                CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
                _cardEventTrigger?.TriggerCancelOtherLeftSelect(state?.nowSelectedCard);

                // 锁定卡牌并标记状态
                isLocked = true;
                isSelected = true;
                isLeftMouseButtonClicking = true;
                isRightMouseButtonClicking = false;
                if (imgCard != null)
                {
                    imgCard.color = Color.red;
                }
                Debug.Log("<color=red>左键选中卡牌</color>");
                _cardEventTrigger?.TriggerLeftSelect(isSelected);
            }

            

            if (isLeftMouseButtonClicking && imgCard != null)
            {
                RectTransform cardRect = imgCard.rectTransform;
                uiCamera = eventData.pressEventCamera;

                // 直接获取 UI 元素在 Canvas 平面上的世界坐标
                // 这是 Screen Space - Camera 模式唯一正确的方法
                Vector3 startPos = cardRect.position;
                startPos.z = cardRect.position.z; // 保留 Canvas 平面的 Z

                _cardEventTrigger?.TriggerLeftDrawLine(startPos);
            }
        }
        else if (eventData.pointerId == -2)// 右键点击
        {
            // 左键选中状态下，禁止右键操作
            if (isLeftMouseButtonClicking)
            {
                Debug.Log("<color=orange>左键选中状态，禁止右键点击</color>");
                return;
            }

            Debug.Log("右键点击卡牌 | isLocked=" + isLocked + " isLeftMouseButtonClicking=" + isLeftMouseButtonClicking);

            // 右键选中/取消逻辑
            if (!isLocked && !isLeftMouseButtonClicking)
            {
                isLocked = true;
                isSelected = true;
                isRightMouseButtonClicking = true;
                isLeftMouseButtonClicking = false;
                if (imgCard != null)
                {
                    imgCard.color = Color.yellow;
                }
                Debug.Log("<color=yellow>右键选中卡牌</color>");
                _cardEventTrigger?.TriggerRightSelect(true);
            }
            else
            {
                // 取消右键选中
                ForceUnlockAndReturn();
                _cardEventTrigger?.TriggerCancelRightSelect();
            }
        }
    }

    /// <summary>
    /// 播放释放卡牌动画
    /// </summary>
    public void PlayReleaseAnimation()
    {
        Debug.Log("播放卡牌释放动画");
    }

    /// <summary>
    /// 播放获取卡牌动画
    /// </summary>
    public void PlayGetAnimator()
    {
        Debug.Log("播放卡牌获取动画");
    }

    /// <summary>
    /// 手动刷新Grid布局后的原始位置（可选：当Grid布局动态变化时调用）
    /// </summary>
    public void RefreshOriginalPos()
    {
        if (rect != null)
        {
            originalAnchoredPos = rect.anchoredPosition;
            //Debug.Log($"[卡牌{gameObject.name}] 刷新原始位置: {originalAnchoredPos}");
        }
    }
}