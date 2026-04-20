using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardEffectUIControl : MonoBehaviour, IBeginDragHandler, IDragHandler,
    IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("拖拽配置")]
    public RectTransform targetArea;
    public float hoverScale = 1.2f;
    public float hoverOffsetY = 20f;

    [Header("=== 弹跳动画参数 ===")]
    public float bounceXOffset = 0;
    public float bounceYOffset = 50f;
    public float bounceScaleIncrement = 0.3f;

    [Header("=== 漂浮动画参数 ===")]
    public bool isOpenFloatEffect = true;
    public float floatVerticalAmplitude = 5f;
    public float floatSpeed = 2f;

    [Header("=== 返回动画参数 ===")]
    public float returnDuration = 0.4f;

    private AnimationCurve bounceCurve = new AnimationCurve(
        new Keyframe(0, 0, 0, 5),
        new Keyframe(0.6f, 1, 0, -3)
    );

    private AnimationCurve returnCurve = new AnimationCurve(
        new Keyframe(0, 0, 2, 2),
        new Keyframe(1, 1, 0, 0)
    );

    [Header("右键选中样式")]
    public Color selectedColor = new Color(1f, 1f, 0.5f);
    public Color normalColor = Color.white;

    private RectTransform rect;
    public Vector2 originalAnchoredPos;
    private Vector3 originalScale;
    private Coroutine animCoroutine;
    private Coroutine returnCoroutine;

    private CardEventTrigger _cardEventTrigger;
    public BaseCard myCard;
    private Image imgCard;
    private Camera uiCamera;
    private GridLayoutCallback gridCallBack;

    private bool isLocked = false;
    private bool isPointerOver = false;
    private bool isLeftMouseButtonClicking;
    private bool isRightMouseButtonClicking;
    private bool isReturning = false;
    private bool isLayoutInitialized = false;
    private bool isDragging = false;
    private bool isSelected;

    public CellEffectControl targetCell;

    public TMP_Text textDesAtk;
    public TMP_Text textDesRange;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        uiCamera = UIMgr.Instance.UICamera;
        if (uiCamera == null)
            Debug.LogError("没有获取到UI相机");
        originalScale = rect.localScale;

        imgCard = GetComponent<Image>();
        if (imgCard == null)
            Debug.LogError($"[卡牌{gameObject.name}]未找到Image组件");

        myCard = GetComponent<BaseCard>();
        if (myCard == null)
            Debug.LogError($"[卡牌{gameObject.name}]未找到BaseCard组件");

        _cardEventTrigger = GetComponent<CardEventTrigger>();
        if (_cardEventTrigger == null)
            Debug.LogError($"[卡牌{gameObject.name}] 未找到CardEventTrigger组件");

        StartCoroutine(InitOriginalPosAfterLayout());
    }

    void Start()
    {
        gridCallBack = UIMgr.Instance.GetPanel<CardPlayingPanel>().mainCallBack;
        if (gridCallBack != null)
            gridCallBack.OnGridLayoutUpdated += RefreshOriginalPos;
    }

    private IEnumerator InitOriginalPosAfterLayout()
    {
        yield return null;
        originalAnchoredPos = rect.anchoredPosition;
        isLayoutInitialized = true;
        Debug.Log($"[卡牌{gameObject.name}] 初始化Grid布局原始位置: {originalAnchoredPos}");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
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

    public void OnPointerExit(PointerEventData eventData)
    {
        // 拖拽、锁定（包括右键选中）时不处理离开
        if (!isLayoutInitialized || isReturning || isDragging || isLocked) return;

        isPointerOver = false;
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(SmoothReturn());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (LevelStepMgr.Instance?.machine?.NowStateType != E_LevelState.PlayerTurn_CardOperate) return;
        if (myCard.cardType == E_CardType.Radical) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // 左键按下时，先清除所有右键选中的卡牌状态
        if (GamePlayer.Instance != null && GamePlayer.Instance.CardCompositeList.Count > 0)
        {
            List<BaseCard> rightSelectedCards = new List<BaseCard>(GamePlayer.Instance.CardCompositeList);
            foreach (var card in rightSelectedCards)
            {
                if (card != null && card.cardEffectControl != null)
                {
                    Debug.Log($"[左键按下] 清除右键选中卡牌: {card.cardID}");
                    card.cardEffectControl.ForceUnlockAndReturn();
                }
            }
            GamePlayer.Instance.RemoveAllCardInCompositeList();
        }

        // 记录当前正在被左键按下的卡牌（全局互斥）
        GamePlayer.CurrentLeftDraggingCard = myCard;
        isLeftMouseButtonClicking = true;
        Debug.Log($"[OnPointerDown] 左键按下，全局拖拽卡牌设置为 {myCard.cardID}");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!isDragging)
        {
            // 短按：清除全局标记，但不解锁（保持选中状态）
            if (GamePlayer.CurrentLeftDraggingCard == myCard)
                GamePlayer.CurrentLeftDraggingCard = null;
            isLeftMouseButtonClicking = false;
            Debug.Log($"[OnPointerUp] 短按，清除全局拖拽卡牌");
            // 短按后可能鼠标仍在卡牌上，主动触发悬停动画
            CheckAndPlayHoverIfNeeded();
        }
        else
        {
            Debug.Log($"[OnPointerUp] 拖拽中，不清除全局记录，等待 OnEndDrag");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (LevelStepMgr.Instance?.machine?.NowStateType != E_LevelState.PlayerTurn_CardOperate) return;
        if (myCard.cardType == E_CardType.Radical) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!isLayoutInitialized || isReturning) return;

        // 清除所有右键选中的卡牌状态（确保拖拽开始前右键选中被取消）
        if (GamePlayer.Instance != null && GamePlayer.Instance.CardCompositeList.Count > 0)
        {
            List<BaseCard> rightSelectedCards = new List<BaseCard>(GamePlayer.Instance.CardCompositeList);
            foreach (var card in rightSelectedCards)
            {
                if (card != null && card.cardEffectControl != null)
                {
                    Debug.Log($"[OnBeginDrag] 清除右键选中卡牌: {card.cardID}");
                    card.cardEffectControl.ForceUnlockAndReturn();
                }
            }
            GamePlayer.Instance.RemoveAllCardInCompositeList();
        }

        isLocked = true;
        isDragging = true;
        isSelected = true;
        imgCard.color = Color.red;
        targetCell = null;

        if (isRightMouseButtonClicking)
        {
            isRightMouseButtonClicking = false;
            if (imgCard != null) imgCard.color = Color.white;
            _cardEventTrigger?.TriggerCancelRightSelect();
        }

        GetComponent<Image>().raycastTarget = false;

        if (DrawLineMgr.Instance != null)
        {
            Vector3 cardWorldPos = rect.TransformPoint(rect.rect.center);
            DrawLineMgr.Instance.EnterDrawing(cardWorldPos);
        }
        _cardEventTrigger?.TriggerLeftSelect(isSelected);
        Debug.Log($"[OnBeginDrag] 开始拖拽，isLocked={isLocked}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;
        targetCell = GetTargetCellUnderMouse(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        isDragging = false;
        isLeftMouseButtonClicking = false;
        isLocked = false;
        if (GamePlayer.CurrentLeftDraggingCard == myCard)
            GamePlayer.CurrentLeftDraggingCard = null;

        GetComponent<Image>().raycastTarget = true;
        imgCard.color = Color.white;

        if (DrawLineMgr.Instance != null)
            DrawLineMgr.Instance.ExitDrawing();

        if (targetCell != null)
        {
            OnDropSuccess();
            targetCell = null;
        }
        else
        {
            _cardEventTrigger?.TriggerCancelLeftSelect();
            StartCoroutine(SmoothReturn());
        }
        Debug.Log($"[OnEndDrag] 结束拖拽");
        // 拖拽结束后，检查鼠标是否在卡牌上，触发悬停动画
        CheckAndPlayHoverIfNeeded();
    }

    void OnDropSuccess()
    {
        GamePlayer.Instance.ReleaseCard(myCard, targetCell.myCell);
    }

    public void RefreshOriginalPos()
    {
        if (rect != null && !isDragging && !isReturning)
        {
            originalAnchoredPos = rect.anchoredPosition;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (LevelStepMgr.Instance?.machine?.NowStateType != E_LevelState.PlayerTurn_CardOperate) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 左键无额外逻辑
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            // 检查是否有其他卡牌正在被左键拖拽/按下
            BaseCard draggingCard = GamePlayer.CurrentLeftDraggingCard;
            if (draggingCard != null && draggingCard != myCard)
            {
                Debug.Log($"<color=orange>其他卡牌 [{draggingCard.cardID}] 正在被左键拖拽，先解除其状态</color>");
                if (draggingCard.cardEffectControl != null)
                {
                    draggingCard.cardEffectControl.ForceUnlockAndReturn();
                }
                GamePlayer.CurrentLeftDraggingCard = null;
            }

            // 如果当前卡牌自己正在左键拖拽，先解除自己
            if (isLeftMouseButtonClicking || isDragging)
            {
                Debug.Log("<color=orange>当前卡牌正在左键拖拽，先解除自身状态</color>");
                ForceUnlockAndReturn();
            }

            Debug.Log($"<color=orange>鼠标右键点击，左键选中状态为{isLeftMouseButtonClicking}</color>");

            if (myCard.cardType == E_CardType.Radical)
            {
                if (myCard is BaseRadicalCard radicalCard && radicalCard.myCardCount <= 0)
                    return;
            }

            // 进入右键选中/合成逻辑
            if (!isLocked && !isLeftMouseButtonClicking)
            {
                // 右键选中：播放弹跳动画并停留，不回归
                isLocked = true;
                isSelected = true;
                isRightMouseButtonClicking = true;
                isLeftMouseButtonClicking = false;
                if (imgCard != null) imgCard.color = Color.yellow;
                Debug.Log("<color=yellow>右键选中卡牌，播放弹起停留动画</color>");

                // 停止当前动画
                if (animCoroutine != null) StopCoroutine(animCoroutine);
                if (returnCoroutine != null) StopCoroutine(returnCoroutine);
                // 播放弹起停留动画（已注释）
                //animCoroutine = StartCoroutine(PlayBounceAndStay());
                _cardEventTrigger?.TriggerRightSelect(true);
            }
            else
            {
                // 已选中状态（再次右键） → 取消选中并回归
                ForceUnlockAndReturn();
                _cardEventTrigger?.TriggerCancelRightSelect();
                // 取消选中后，鼠标可能仍在卡牌上，触发悬停动画
                CheckAndPlayHoverIfNeeded();
            }
        }
    }

    IEnumerator PlayBounceAndFloat()
    {
        float bounceDuration = 0.6f;
        float time = 0;

        while (time < bounceDuration)
        {
            float normalizedTime = time / bounceDuration;
            float t = bounceCurve.Evaluate(normalizedTime);
            rect.anchoredPosition = originalAnchoredPos + new Vector2(bounceXOffset * t, bounceYOffset * t);
            rect.localScale = originalScale * (1 + bounceScaleIncrement * t);
            time += Time.deltaTime;
            yield return null;
        }

        Vector2 bounceEndPos = originalAnchoredPos + new Vector2(bounceXOffset, bounceYOffset);
        Vector3 bounceEndScale = originalScale * (1 + bounceScaleIncrement);
        rect.anchoredPosition = bounceEndPos;
        rect.localScale = bounceEndScale;

        if (isOpenFloatEffect)
        {
            float elapsedTime = 0f;
            while (true)
            {
                // 如果被锁定且是右键选中状态，则退出漂浮，保持当前位置（不回归）
                if (isLocked && isRightMouseButtonClicking)
                {
                    yield break;
                }
                float floatOffset = Mathf.Sin(elapsedTime * floatSpeed) * floatVerticalAmplitude;
                rect.anchoredPosition = new Vector2(bounceEndPos.x, bounceEndPos.y + floatOffset);
                elapsedTime += Time.deltaTime;
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
        if (!isLayoutInitialized || isDragging) yield break;

        isReturning = true;
        Vector2 startPos = rect.anchoredPosition;
        Vector3 startScale = rect.localScale;
        float time = 0;

        while (time < returnDuration)
        {
            float normalizedTime = time / returnDuration;
            float t = returnCurve.Evaluate(normalizedTime);
            rect.anchoredPosition = Vector2.Lerp(startPos, originalAnchoredPos, t);
            rect.localScale = Vector3.Lerp(startScale, originalScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = originalAnchoredPos;
        rect.localScale = originalScale;
        returnCoroutine = null;
        isReturning = false;

        // 回归完成后，检查鼠标是否在卡牌上，触发悬停动画
        CheckAndPlayHoverIfNeeded();
    }

    private CellEffectControl GetTargetCellUnderMouse(PointerEventData eventData)
    {
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var res in results)
        {
            CellEffectControl cell = res.gameObject.GetComponent<CellEffectControl>();
            if (cell != null) return cell;
        }
        return null;
    }

    public void ForceUnlockAndReturn()
    {
        // 允许拖拽中或锁定时强制回归
        if ((!isLocked && !isDragging) || isReturning || !isLayoutInitialized) return;

        isReturning = true;
        isLocked = false;
        isDragging = false;
        isSelected = false;
        isPointerOver = false;
        isLeftMouseButtonClicking = false;
        isRightMouseButtonClicking = false;
        if (GamePlayer.CurrentLeftDraggingCard == myCard)
            GamePlayer.CurrentLeftDraggingCard = null;
        if (imgCard != null) imgCard.color = Color.white;

        if (DrawLineMgr.Instance != null)
            DrawLineMgr.Instance.ExitDrawing();

        GetComponent<Image>().raycastTarget = true;

        if (animCoroutine != null) StopCoroutine(animCoroutine);
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);

        returnCoroutine = StartCoroutine(SmoothReturn());
        Debug.Log("<color=yellow>强制解锁并返回原始位置</color>");
    }

    /// <summary>
    /// 检查鼠标是否在当前卡牌上，如果是则手动触发悬停动画
    /// 解决快速移动鼠标导致 OnPointerEnter 未触发的问题
    /// </summary>
    private void CheckAndPlayHoverIfNeeded()
    {
        if (!isLayoutInitialized || isReturning || isLocked || isDragging) return;
        if (IsMouseOverUI())
        {
            // 鼠标确实在卡牌上，触发悬停动画
            isPointerOver = true;
            if (animCoroutine != null) StopCoroutine(animCoroutine);
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
                returnCoroutine = null;
            }
            animCoroutine = StartCoroutine(PlayBounceAndFloat());
            Debug.Log($"[CheckAndPlayHover] 手动触发悬停动画: {myCard.cardID}");
        }
    }

    private bool IsMouseOverUI()
    {
        if (EventSystem.current == null) return false;
        PointerEventData pointerData = new PointerEventData(EventSystem.current) { position = Input.mousePosition };
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        foreach (var result in results)
        {
            if (result.gameObject == this.gameObject)
                return true;
        }
        return false;
    }

    private void OnDisable()
    {
        if (GamePlayer.CurrentLeftDraggingCard == myCard)
            GamePlayer.CurrentLeftDraggingCard = null;
    }

    public void PlayReleaseAnimation() => Debug.Log("播放卡牌释放动画");
    public void ResetTransform()
    {
        Vector3 localPos = rect.localPosition;
        localPos.z = 0;
        rect.localPosition = localPos;
        rect.localScale = Vector3.one;
        originalScale = rect.localScale;
        StartCoroutine(InitOriginalPosAfterLayout());
    }

    public void UpdateDesAtk(int atk)
    {
        if (textDesAtk == null) return;
        string newStr = string.Format(myCard.desEffection, atk);
        textDesAtk.text = newStr;
    }

    public void ResetDesAtk()
    {
        if (textDesAtk == null) return;
        string newStr = string.Format(myCard.desEffection, myCard.cardData.desEffection);
        textDesAtk.text = newStr;
    }

    public void ResetAtkOnPenEdgeHave(int currentCardCounts)
    {
        if (textDesAtk == null) return;
        UpdateDesAtk(myCard.currentAtk + currentCardCounts / 2);
    }

    public void UpdateDesRange(int wide, int high)
    {
        if (textDesRange == null) return;
        string newStr = string.Format(myCard.desRange, wide, high);
        textDesRange.text = newStr;
    }

    public void ResetDesRange()
    {
        if (textDesRange == null) return;
        string newStr = string.Format(myCard.desRange, myCard.cardData.baseRecRangeWide, myCard.cardData.baseRecRangeHigh);
        textDesRange.text = newStr;
    }
}