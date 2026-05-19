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
    public float dragThreshold = 10f;

    [Header("=== 弹跳动画参数 ===")]
    public float bounceXOffset = 0;
    public float bounceYOffset = 50f;
    public float bounceDuration = 0.3f;
    public float bounceScaleIncrement = 0.3f;

    [Header("=== 漂浮动画参数 ===")]
    public bool isOpenFloatEffect = true;
    public float floatVerticalAmplitude = 2f;
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
    public Color rightSelectedColor = new Color(1f, 1f, 0.5f);
    [Header("左键选中样式")]
    private Color leftSelectedColor = new Color(0.92f, 0.45f, 0.42f);

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
    private GridHorizontalLayoutCallback gridCallBack;

    private bool isLocked = false;
    private bool isPointerOver = false;
    private bool isLeftMouseButtonClicking;
    private bool isRightMouseButtonClicking;
    private bool isReturning = false;
    private bool isLayoutInitialized = false;
    private bool isDragging = false;
    private bool isSelected;

    private bool isPotentialDrag;

    public CellEffectControl targetCell;
    private CardShowBubble cardShowBubble;
    private CardHighlight cardHighlight;


    public TMP_Text textDesEffection;
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


        cardShowBubble = GetComponent<CardShowBubble>();
        if (cardShowBubble == null)
            Debug.LogWarning($"[{gameObject.name}]未找到CardShowBubble组件");

        cardHighlight = GetComponent<CardHighlight>();
        if (cardHighlight == null)
            Debug.LogWarning($"[{gameObject.name}]未找到CardShowBubble组件");

        StartCoroutine(InitOriginalPosAfterLayout());

        //注册笔峰带来的伤害更替事件
        EventCenter.Instance.AddEventListener(E_EventType.Treasure_PenEdgeUpdateAtk, ResetAtkOnPenEdgeHave);
    }

    void Start()
    {
        //更新描述
        UpdateCardDes();

        gridCallBack = UIMgr.Instance.GetPanel<CardPlayingPanel>().mainCallBack;
        if (gridCallBack != null)
            gridCallBack.OnHorizontalLayoutUpdated += RefreshOriginalPos;
    }

    private void OnDestroy()
    {
        //注册笔峰带来的伤害更替事件
        EventCenter.Instance.RemoveEventListener(E_EventType.Treasure_PenEdgeUpdateAtk, ResetAtkOnPenEdgeHave);
        
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
            AudioMgr.Instance.PlaySFX("选牌音效");
            isPointerOver = true;
            if (animCoroutine != null) StopCoroutine(animCoroutine);
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
                returnCoroutine = null;
            }
            animCoroutine = StartCoroutine(PlayBounceAndFloat());
            cardHighlight.SetTop();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isLayoutInitialized || isReturning || isDragging || isLocked) return;

        cardShowBubble.HideBubble();


        isPointerOver = false;
        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(SmoothReturn());
        cardHighlight.ResetTop();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (LevelStepMgr.Instance?.machine?.NowStateType != E_LevelState.PlayerTurn_CardOperate) return;
        if (myCard.cardType == E_CardType.Radical) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;

        // 清除所有右键选中的卡牌状态
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

        GamePlayer.CurrentLeftDraggingCard = myCard;
        isLeftMouseButtonClicking = true;
        isPotentialDrag = false;
        Debug.Log($"[OnPointerDown] 左键按下，全局拖拽卡牌设置为 {myCard.cardID}");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;

        if (!isDragging)
        {
            if (GamePlayer.CurrentLeftDraggingCard == myCard)
                GamePlayer.CurrentLeftDraggingCard = null;
            isLeftMouseButtonClicking = false;
            isLocked = false;
            Debug.Log($"[OnPointerUp] 短按，清除全局拖拽卡牌");
            CheckAndPlayHoverIfNeeded();
        }
        else
        {
            Debug.Log($"[OnPointerUp] 拖拽中，不清除全局记录，等待 OnEndDrag");
        }
        //清空玩家手上记录的卡牌
        GamePlayer.Instance.nowSelectedCard = null;
        //回归原始层级
        cardHighlight.ResetTop();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        cardShowBubble.HideBubble();

        if (LevelStepMgr.Instance?.machine?.NowStateType != E_LevelState.PlayerTurn_CardOperate) return;
        if (myCard.cardType == E_CardType.Radical) return;
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!isLayoutInitialized || isReturning) return;

        if (Vector2.Distance(eventData.pressPosition, eventData.position) < dragThreshold)
        {
            return;
        }
        isPotentialDrag = true;

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
        imgCard.color = leftSelectedColor;
        targetCell = null;

        if (isRightMouseButtonClicking)
        {
            isRightMouseButtonClicking = false;
            if (imgCard != null) imgCard.color = normalColor;
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
        imgCard.color = normalColor;

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
            isLayoutInitialized = true;
            Debug.Log($"[卡牌{gameObject.name}] 刷新原始位置: {originalAnchoredPos}");

            // 布局更新后，若鼠标已在卡牌上，手动触发悬停动画（解决合成后无动画问题）
            CheckAndPlayHoverIfNeeded();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (LevelStepMgr.Instance?.machine?.NowStateType != E_LevelState.PlayerTurn_CardOperate) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // 左键点击：解除右键选中状态，同时清理任何锁定（增强版）
            AudioMgr.Instance.PlaySFX("选牌音效");

            if (isRightMouseButtonClicking || isLocked || isReturning)
            {
                Debug.Log("<color=cyan>左键点击解除锁定/右键选中状态</color>");
                ForceUnlockAndReturn();
                _cardEventTrigger?.TriggerCancelRightSelect();
                cardHighlight.SetTop();
                return;
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            AudioMgr.Instance.PlaySFX("选牌音效");

            // 如果卡牌正在返回动画中，强制中断并复位，然后继续处理右键
            if (isReturning)
            {
                Debug.Log("<color=orange>卡牌正在返回动画中，强制中断回归</color>");
                if (returnCoroutine != null) StopCoroutine(returnCoroutine);
                returnCoroutine = null;
                rect.anchoredPosition = originalAnchoredPos;
                rect.localScale = originalScale;
                isReturning = false;
                isLocked = false;
                isPointerOver = false;
                if (imgCard != null) imgCard.color = normalColor;
            }

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

            if (!isLocked && !isLeftMouseButtonClicking)
            {
                isLocked = true;
                isSelected = true;
                isRightMouseButtonClicking = true;
                isLeftMouseButtonClicking = false;
                if (imgCard != null) imgCard.color = rightSelectedColor;
                Debug.Log("<color=yellow>右键选中卡牌，播放弹起停留动画</color>");
                //设置为高层层级
                cardHighlight.SetTop();
                if (animCoroutine != null) StopCoroutine(animCoroutine);
                if (returnCoroutine != null) StopCoroutine(returnCoroutine);
                animCoroutine = StartCoroutine(PlayBounceAndStay());
                _cardEventTrigger?.TriggerRightSelect(true);
            }
            else
            {
                ForceUnlockAndReturn();
                _cardEventTrigger?.TriggerCancelRightSelect();
                CheckAndPlayHoverIfNeeded();
            }
        }
    }

    IEnumerator PlayBounceAndStay()
    {
        float time = 0;
        Vector2 startPos = rect.anchoredPosition;
        Vector3 startScale = rect.localScale;
        Vector2 targetPos = originalAnchoredPos + new Vector2(bounceXOffset, bounceYOffset);
        Vector3 targetScale = originalScale * (1 + bounceScaleIncrement);

        while (time < bounceDuration)
        {
            float t = bounceCurve.Evaluate(time / bounceDuration);
            rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            rect.localScale = Vector3.Lerp(startScale, targetScale, t);
            time += Time.deltaTime;
            yield return null;
        }

        rect.anchoredPosition = targetPos;
        rect.localScale = targetScale;
    }

    IEnumerator PlayBounceAndFloat()
    {
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

        //如果有一张卡牌是右键选中状态，展示预合成
        Debug.Log("检测到玩家选中卡牌数量"+ GamePlayer.Instance.CardCompositeSelectedCount);
        if (GamePlayer.Instance != null)
        {
            // 再判空 选中的卡牌
            if (GamePlayer.Instance.nowSelectedCard != null && myCard != null)
            {
                // 最后才是你原本的逻辑
                if (GamePlayer.Instance.CardCompositeSelectedCount == 1)
                {
                    Debug.Log("检测到玩家选中一张合成卡牌，展示预合成卡牌");

                    if (cardShowBubble != null)
                        cardShowBubble.ShowPrevCompositeBubble(
                            GamePlayer.Instance.nowSelectedCard.cardID,
                            myCard.cardID
                        );
                }
            }
        }
        if (isOpenFloatEffect)
        {
            float elapsedTime = 0f;
            while (true)
            {
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
        cardHighlight.ResetTop();

        // 放宽条件：只要布局已初始化，且有任何一种锁定/拖拽/标记/回归状态，就强制复位
        if (!isLayoutInitialized) return;
        if (!isLocked && !isDragging && !isLeftMouseButtonClicking && !isRightMouseButtonClicking && !isReturning)
            return;

        //关闭气泡描述
        cardShowBubble.HideBubble();

        // 停止所有协程
        if (animCoroutine != null) StopCoroutine(animCoroutine);
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);
        animCoroutine = null;
        returnCoroutine = null;

        // 重置所有状态标志
        isReturning = false;
        isLocked = false;
        isDragging = false;
        isSelected = false;
        isPointerOver = false;
        isLeftMouseButtonClicking = false;
        isRightMouseButtonClicking = false;
        if (GamePlayer.CurrentLeftDraggingCard == myCard)
            GamePlayer.CurrentLeftDraggingCard = null;
        if (imgCard != null) imgCard.color = normalColor;

        if (DrawLineMgr.Instance != null)
            DrawLineMgr.Instance.ExitDrawing();

        GetComponent<Image>().raycastTarget = true;

        // 瞬间复位位置和缩放
        rect.anchoredPosition = originalAnchoredPos;
        rect.localScale = originalScale;

        Debug.Log("<color=yellow>强制解锁并返回原始位置（增强版）</color>");
    }

    private void CheckAndPlayHoverIfNeeded()
    {
        if (!isLayoutInitialized || isReturning || isLocked || isDragging) return;
        if (IsMouseOverUI())
        {
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
        if (GetComponent<Image>() != null)
            GetComponent<Image>().raycastTarget = true;
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

    /// <summary>
    /// 更新效果描述
    /// </summary>
    /// <param name="atk"></param>
    public void UpdateDesEffection(int atk)
    {
        if (textDesEffection == null) return;

        string strAtk = atk <= 0 ? "" : atk.ToString(); // 如果攻击力小于0，说明不更新攻击力
        string newStr = string.Format(myCard.desEffection, strAtk);
        textDesEffection.text = newStr;
        Debug.Log($"更新卡牌{myCard.cardID}攻击力描述为" + strAtk + "全部描述为" + myCard.desEffection);

    }

    public void ResetDesEffection()
    {
        if (textDesEffection == null) return;
        string newStr = string.Format(myCard.desEffection, myCard.cardData.baseAtk);
        textDesEffection.text = myCard.cardData.desEffection;
    }

    public void ResetAtkOnPenEdgeHave()
    {
        if (textDesEffection == null) return;

       
        MonoMgr.Instance.StartCoroutine(ResetAtkDesOnPenEdgeHave());
    }

    private IEnumerator ResetAtkDesOnPenEdgeHave()
    {
        yield return null;

        //myCard.desViewAtk = myCard.cardData.baseAtk;
        //计算额外伤害
        int atk = Dealer.Instance.nowCards.Count / 3;
        if (atk > 3)
            atk = 3;

        //计算总伤害
         myCard.desViewAtk += atk;

        Debug.Log($"[笔峰]{myCard.cardID}更新卡牌的攻击力为" + myCard.desViewAtk + "原始攻击力为" + myCard.currentAtk);

        UpdateDesEffection(myCard.desViewAtk);
        myCard.desViewAtk -= atk;


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

    /// <summary>
    /// 更新描述
    /// </summary>
    private void UpdateCardDes()
    {
        //更新描述
        UpdateDesRange(myCard.currentRecRangeWide, myCard.currentRecRangeHigh);
        UpdateDesEffection(myCard.currentAtk);
        Debug.Log("CardEffectUIControl更新卡牌描述" + myCard.desEffection + myCard.desRange);

    }
}