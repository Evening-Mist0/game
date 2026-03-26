using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

public class CardEffectUIControl : MonoBehaviour,IBeginDragHandler, IDragHandler, 
    IEndDragHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    [Header("拖拽配置")]
    [Tooltip("拖到哪里才算成功（比如出牌区）")]
    public RectTransform targetArea;
    [Tooltip("鼠标悬停时卡牌放大倍数")]
    public float hoverScale = 1.2f;
    [Tooltip("悬停时卡牌向上偏移距离")]
    public float hoverOffsetY = 20f;

    [Header("=== 弹跳动画参数 ===")]
    public float bounceXOffset = 0;        // 水平偏移
    public float bounceYOffset = 50f;        // 垂直偏移
    public float bounceScaleIncrement = 0.3f;// 缩放增量

    [Header("=== 漂浮动画参数 ===")]
    public bool isOpenFloatEffect = true;   // 开启漂浮
    public float floatVerticalAmplitude = 5f;// 漂浮振幅
    public float floatSpeed = 2f;            // 漂浮速度

    [Header("=== 返回动画参数 ===")]
    public float returnDuration = 0.4f;      // 返回时长

    // 弹跳动画曲线
    private AnimationCurve bounceCurve = new AnimationCurve(
        new Keyframe(0, 0, 0, 5),
        new Keyframe(0.6f, 1, 0, -3)
    );

    // 返回动画曲线
    private AnimationCurve returnCurve = new AnimationCurve(
        new Keyframe(0, 0, 2, 2),
        new Keyframe(1, 1, 0, 0)
    );

    [Header("右键选中样式")]
    public Color selectedColor = new Color(1f, 1f, 0.5f); // 选中高亮色
    public Color normalColor = Color.white;               // 正常颜色

    private RectTransform rect;
    private Vector2 originalAnchoredPos; // Grid布局下的原始锚点位置
    private Vector3 originalScale;
    private Coroutine animCoroutine;     // 弹跳/漂浮动画协程
    private Coroutine returnCoroutine;   // 返回动画协程

    // 卡牌事件触发器
    private CardEventTrigger _cardEventTrigger;
    public BaseCard myCard;
    private Image imgCard;
    private Camera uiCamera;//渲染该卡牌的UI相机
    private GridLayoutCallback gridCallBack;

    // 状态控制
    private bool isLocked = false;           // 是否被锁定（选中状态）
    private bool isPointerOver = false;      // 鼠标是否在UI上
    private bool isLeftMouseButtonClicking;  // 左键是否被选中
    private bool isRightMouseButtonClicking; // 右键是否被选中         
    private bool isReturning = false;         // 是否正在返回原位
    private bool isLayoutInitialized = false; // Grid布局是否初始化完成
    private bool isDragging = false;         // 是否正在拖拽
    private bool isSelected;                  // 是否被选中

    private CellEffectControl targetCell;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
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

        //实例化后检查鼠标是否已经在卡牌上
    }
    private IEnumerator InitOriginalPosAfterLayout()
    {
        yield return null; // 等待1帧，让GridLayoutGroup完成布局
        originalAnchoredPos = rect.anchoredPosition;
        isLayoutInitialized = true;
        Debug.Log($"[卡牌{gameObject.name}] 初始化Grid布局原始位置: {originalAnchoredPos}");
    }
    //鼠标悬停与离开
    //鼠标悬停
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 状态校验：未拖拽、未锁定、已初始化、未回弹
        if (!isDragging && !isLocked && isLayoutInitialized && !isReturning)
        {
            isPointerOver = true;
            if (animCoroutine != null) StopCoroutine(animCoroutine);
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
                returnCoroutine = null;
            }
            animCoroutine = StartCoroutine(PlayBounceAndFloat());
        }
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isLayoutInitialized || isReturning || isLocked) return;

        isPointerOver = false;
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(SmoothReturn());
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (LevelStepMgr.Instance?.machine?.NowStateType != E_LevelState.PlayerTurn_CardOperate) return;
        if (myCard.cardType == E_CardType.Radical)
            return;
        if (eventData.button != PointerEventData.InputButton.Left)
            return;
        if (!isLayoutInitialized || isLocked || isReturning)
            return;

        // 仅标记拖拽状态，卡牌不动
        isDragging = true;
        isSelected = true;
        imgCard.color = Color.red;
        targetCell = null;

        if (isRightMouseButtonClicking)
            ToggleRightSelect(false);

        // 关闭射线，不影响指示线检测
        GetComponent<Image>().raycastTarget = false;

        // 开启指示线（交给画线脚本自己更新）
        if (DrawLineMgr.Instance != null)
        {
            Vector3 cardWorldPos = rect.TransformPoint(rect.rect.center);
            DrawLineMgr.Instance.EnterDrawing(cardWorldPos);
        }
        //触发左键选中事件
        _cardEventTrigger?.TriggerLeftSelect(isSelected);
    }

    // 2. 拖拽中（每帧执行）
    public void OnDrag(PointerEventData eventData)
{
    if (!isDragging) return;

    // 仅检测鼠标指向的格子（唯一核心功能）
    targetCell = GetTargetCellUnderMouse(eventData);
}

    // 3. 结束拖拽（松手）
    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        GetComponent<Image>().raycastTarget = true;
        imgCard.color = Color.white;

        // 仅通知结束画线
        if (DrawLineMgr.Instance != null)
            DrawLineMgr.Instance.ExitDrawing();

        // 拖到格子 → 触发出牌
        if (targetCell != null)
        {
            OnDropSuccess();
            targetCell = null;
        }
        // 拖空 → 返回原位
        else
        {
            _cardEventTrigger?.TriggerCancelLeftSelect();
            StartCoroutine(SmoothReturn());
        }
    }
    //卡牌释放函数
    void OnDropSuccess()
    {
        // 直接调用GamePlayer里的ReleaseCard，和手动左键点击效果完全一致
        GamePlayer.Instance.ReleaseCard(myCard, targetCell.myCell);
    }

    //同步卡牌在 Grid 布局中的原始位置
    public void RefreshOriginalPos()
    {
        if (rect != null && !isDragging && !isReturning)
        {
            originalAnchoredPos = rect.anchoredPosition;
        }
    }
    //鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        if (LevelStepMgr.Instance?.machine?.NowStateType != E_LevelState.PlayerTurn_CardOperate) return;
        // 右键点击：仅选中/取消选中，不触发拖拽
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            ToggleRightSelect();
        }
    }
    // 右键选中/取消选中切换
    private void ToggleRightSelect(bool? forceState = null)
    {
        // 强制设置状态（比如拖拽时强制取消）
        if (forceState.HasValue)
        {
            isRightMouseButtonClicking = forceState.Value;
        }
        else
        {
            // 切换状态
            isRightMouseButtonClicking = !isRightMouseButtonClicking;
        }

        // 视觉反馈：改变卡牌颜色（也可以换成高亮边框、缩放等）
        if (imgCard != null)
        {
            imgCard.color = isRightMouseButtonClicking ? selectedColor : normalColor;
        }

        // 可选：触发选中/取消选中事件
        Debug.Log($"卡牌{gameObject.name} 右键选中状态：{isRightMouseButtonClicking}");
        if (myCard != null)
        {
            _cardEventTrigger.TriggerRightSelect(true);
        }
    }
    /// <summary>
    /// 弹跳动画
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayBounceAndFloat()
    {
        float bounceDuration = 0.6f;
        float time = 0;

        // 1. 弹跳阶段
        while (time < bounceDuration)
        {
            float normalizedTime = time / bounceDuration;
            float t = bounceCurve.Evaluate(normalizedTime);

            rect.anchoredPosition = originalAnchoredPos + new Vector2(bounceXOffset * t, bounceYOffset * t);
            rect.localScale = originalScale * (1 + bounceScaleIncrement * t);

            time += Time.deltaTime;
            yield return null;
        }

        // 弹跳结束固定位置
        Vector2 bounceEndPos = originalAnchoredPos + new Vector2(bounceXOffset, bounceYOffset);
        Vector3 bounceEndScale = originalScale * (1 + bounceScaleIncrement);
        rect.anchoredPosition = bounceEndPos;
        rect.localScale = bounceEndScale;

        // 2. 漂浮阶段（开启时生效）
        if (isOpenFloatEffect)
        {
            float elapsedTime = 0f;
            while (true)
            {
                float floatOffset = Mathf.Sin(elapsedTime * floatSpeed) * floatVerticalAmplitude;
                rect.anchoredPosition = new Vector2(bounceEndPos.x, bounceEndPos.y + floatOffset);

                elapsedTime += Time.deltaTime;

                // 满足条件则返回原位
                if (!isPointerOver || isLocked || isReturning)
                {
                    StartCoroutine(SmoothReturn());
                    yield break;
                }
                yield return null;
            }
        }
    }

    /// <summary>
    /// 平滑返回原始位置协程
    /// </summary>
    IEnumerator SmoothReturn()
    {
        if (!isLayoutInitialized || isDragging) yield break;

        isReturning = true;
        Vector2 startPos = rect.anchoredPosition;
        Vector3 startScale = rect.localScale;
        float time = 0;

        // 插值返回
        while (time < returnDuration)
        {
            float normalizedTime = time / returnDuration;
            float t = returnCurve.Evaluate(normalizedTime);

            rect.anchoredPosition = Vector2.Lerp(startPos, originalAnchoredPos, t);
            rect.localScale = Vector3.Lerp(startScale, originalScale, t);

            time += Time.deltaTime;
            yield return null;
        }

        // 强制校准位置
        rect.anchoredPosition = originalAnchoredPos;
        rect.localScale = originalScale;

        returnCoroutine = null;
        isReturning = false;
    }
    private CellEffectControl GetTargetCellUnderMouse(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var res in results)
        {
            CellEffectControl cell = res.gameObject.GetComponent<CellEffectControl>();
            if (cell != null)
                return cell;
        }
        return null;
    }

    /// <summary>
    /// 下面不用看，我当时换脚本时，弹报错，我直接把报错相关的函数从你那复杂过来了
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
   
    public void PlayReleaseAnimation()
    {
        Debug.Log("播放卡牌释放动画");
    }
    public void ResetTransform()
    {
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        originalScale = rect.localScale;
        // 延迟1帧获取Grid布局后的初始位置（等GridLayoutGroup布局完成）
        StartCoroutine(InitOriginalPosAfterLayout());
    }
}
