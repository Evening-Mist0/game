
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;

/// <summary>
/// 卡牌使用时产生的技能效果类型
/// </summary>
public enum E_AttackEffectType
{
    Fire,
    FirePlus,
    Water,
    Earth,
    Wood,
    Repel,
    TrueDamage,
    Heal,
    GetDef,
    Burn,
    Imprison,
    SpeedUp,
}

/// <summary>
/// 卡牌效果控制类，负责卡牌UI动画、选中状态、交互逻辑
/// </summary>
public class CardEffectControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform rect;
    public Vector2 originalAnchoredPos;
    private Vector3 originalScale;
    private Coroutine animCoroutine;
    private Coroutine returnCoroutine;
    private Animator animator;
    private GridLayoutCallback gridCallBack;

    private CardEventTrigger _cardEventTrigger;
    public BaseCard myCard;
    private Image imgCard;
    private Camera uiCamera;
    private CardHighlight cardHighlight;

    private bool isLocked = false;
    private bool isPointerOver = false;
    private bool isLeftMouseButtonClicking;
    private bool isRightMouseButtonClicking;
    private bool isSelected;
    private bool isReturning = false;
    private bool isLayoutInitialized = false;

    private AnimationCurve bounceCurve = new AnimationCurve(
        new Keyframe(0, 0, 0, 5),
        new Keyframe(0.6f, 1, 0, -3)
    );

    private AnimationCurve returnCurve = new AnimationCurve(
        new Keyframe(0, 0, 2, 2),
        new Keyframe(1, 1, 0, 0)
    );

    [Header("弹起动画设置")]
    public float bounceXOffset = 50f;
    public float bounceYOffset = 30f;
    public float bounceScaleIncrement = 0.1f;

    [Header("悬浮动画设置")]
    public float floatVerticalAmplitude = 5f;
    public float floatSpeed = 2f;

    [Header("回归设置")]
    public float returnDuration = 0.4f;

    public bool isOpenFloatEffect;

    void Awake()
    {
        uiCamera = UIMgr.Instance.UICamera;
        if (uiCamera == null)
            Debug.LogError("未获取到UI相机");
        rect = GetComponent<RectTransform>();
        originalScale = rect.localScale;

        imgCard = GetComponent<Image>();
        if (imgCard == null)
            Debug.LogError($"[{gameObject.name}]未找到Image组件");

        cardHighlight = GetComponent<CardHighlight>();
        if (cardHighlight == null)
            Debug.LogError($"[{gameObject.name}]未找到CardHighlight组件");

        myCard = GetComponent<BaseCard>();
        if (myCard == null)
            Debug.LogError($"[{gameObject.name}]未找到BaseCard组件");

        _cardEventTrigger = GetComponent<CardEventTrigger>();
        if (_cardEventTrigger == null)
            Debug.LogError($"[{gameObject.name}]未找到CardEventTrigger组件");

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError($"[{gameObject.name}]未找到Animator组件");

    }

    void Start()
    {
        // 获取 gridCallBack
        gridCallBack = UIMgr.Instance.GetPanel<CardPlayingPanel>().mainCallBack;
        if (gridCallBack != null)
        {
            Debug.Log($"[{gameObject.name}] Start中注册回调");
            gridCallBack.OnGridLayoutUpdated += RefreshOriginalPos;
        }
        StartCoroutine(CheckMouseOverAfterInstantiate());

        // 主动获取一次当前位置
        RefreshOriginalPos();
    }

    private IEnumerator CheckMouseOverAfterInstantiate()
    {
        yield return null;
        if (!isLayoutInitialized)
            yield return new WaitUntil(() => isLayoutInitialized);

        if (IsMouseOverUI() && !isLocked && !isReturning)
        {
            Debug.Log($"[{gameObject.name}] 实例化时鼠标已悬停，手动触发悬停动画");
            isPointerOver = true;
            if (animCoroutine != null)
                StopCoroutine(animCoroutine);
            if (returnCoroutine != null)
            {
                StopCoroutine(returnCoroutine);
                returnCoroutine = null;
            }
            animCoroutine = StartCoroutine(PlayBounceAndFloat());
        }
    }

    /// <summary>
    /// 重置卡牌变换（仅缩放，位置由GridLayout决定）
    /// </summary>
    public void ResetTransform()
    {
        rect.localPosition = Vector3.zero;
        rect.localScale = Vector3.one;
        originalScale = rect.localScale;
    }

    /// <summary>
    /// 记录当前网格布局的正确位置（供外部在强制布局后调用）
    /// </summary>
    //public void RecordOriginalPos()
    //{
    //    if (rect != null)
    //    {
    //        originalAnchoredPos = rect.anchoredPosition;
    //        isLayoutInitialized = true;
    //        Debug.Log($"[{gameObject.name}] 记录布局原始位置: {originalAnchoredPos}");
    //    }
    //}

    private void OnEnable()
    {
        // 重置所有交互状态
        isReturning = false;
        isLocked = false;
        isPointerOver = false;
        isLeftMouseButtonClicking = false;
        isRightMouseButtonClicking = false;
        isSelected = false;
        if (imgCard != null) imgCard.color = Color.white;

        // 使用当前RectTransform位置作为临时原始位置（允许立即交互）
        if (rect != null)
        {
            //originalAnchoredPos = rect.anchoredPosition;

            //originalScale = rect.localScale;
        }
        isLayoutInitialized = true; // 允许交互

        Debug.Log($"[卡牌激活]{gameObject.name}激活，当前缩放{rect.localScale}，位置{rect.localPosition}");
    }

    private void OnDisable()
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);

        if (isLayoutInitialized)
            rect.anchoredPosition = originalAnchoredPos;
        rect.localScale = originalScale;

        isReturning = false;
        isLocked = false;
        isPointerOver = false;
        isLeftMouseButtonClicking = false;
        isRightMouseButtonClicking = false;
        isSelected = false;
        if (imgCard != null) imgCard.color = Color.white;
    }

    private void Update()
    {
        if (!isLayoutInitialized) return;

        if (isLeftMouseButtonClicking && Input.GetMouseButtonDown(1) && !isReturning)
        {
            ForceUnlockAndReturn();
            _cardEventTrigger?.TriggerCancelRightSelect();
            _cardEventTrigger?.TriggerCancelLeftSelect();
        }

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
            Debug.Log($"[{gameObject.name}] 销毁，取消布局更新监听");
        }
    }

    public void ForceUnlockAndReturn()
    {
        if (!isLocked || isReturning || !isLayoutInitialized) return;

        ResetTop();
        isReturning = true;
        isLocked = false;
        isSelected = false;
        isPointerOver = false;
        isLeftMouseButtonClicking = false;
        isRightMouseButtonClicking = false;
        if (imgCard != null) imgCard.color = Color.white;

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

        returnCoroutine = StartCoroutine(SmoothReturn());
        Debug.Log("<color=yellow>强制解锁并回归原始位置</color>");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isLayoutInitialized) return;
        isPointerOver = true;
        SetTop();

        if (isReturning || isLocked)
        {
            Debug.Log(isReturning ? "<color=cyan>正在回归中，跳过悬停动画</color>" : "<color=cyan>锁定状态，跳过悬停动画</color>");
            return;
        }

        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        animCoroutine = StartCoroutine(PlayBounceAndFloat());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isLayoutInitialized) return;
        isPointerOver = false;

        if (isReturning || isLocked)
        {
            Debug.Log(isReturning ? "<color=cyan>正在回归中，忽略离开事件</color>" : "<color=cyan>锁定状态，忽略离开事件</color>");
            return;
        }

        ResetTop();

        if (animCoroutine != null)
        {
            StopCoroutine(animCoroutine);
            animCoroutine = null;
        }

        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(SmoothReturn());
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
                float floatOffset = Mathf.Sin(elapsedTime * floatSpeed) * floatVerticalAmplitude;
                rect.anchoredPosition = new Vector2(bounceEndPos.x, bounceEndPos.y + floatOffset);

                elapsedTime += Time.deltaTime;

                if (!isPointerOver || isLocked || isReturning)
                {
                    ResetTop();
                    StartCoroutine(SmoothReturn());
                    yield break;
                }

                yield return null;
            }
        }
    }

    IEnumerator SmoothReturn()
    {
        if (!isLayoutInitialized) yield break;

        isReturning = true;
        ResetTop();

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

        yield return null;

        if (!isLocked && IsMouseOverUI())
        {
            Debug.Log("<color=green>回归后鼠标仍悬停，重新播放悬停动画</color>");
            SetTop();
            isPointerOver = true;
            animCoroutine = StartCoroutine(PlayBounceAndFloat());
        }
    }

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

    public void CancelLock()
    {
        if (isLocked && !isReturning && isLayoutInitialized)
            ForceUnlockAndReturn();
    }

    public bool IsLocked() => isLocked;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isLayoutInitialized || isReturning)
        {
            Debug.Log("<color=red>卡牌未初始化/正在回归，禁止操作</color>");
            return;
        }

        if (LevelStepMgr.Instance?.machine?.NowStateType != E_LevelState.PlayerTurn_CardOperate) return;

        if (eventData.pointerId == -1) // 左键
        {
            if (myCard.cardType == E_CardType.Radical) return;
            if (isRightMouseButtonClicking)
            {
                Debug.Log("<color=orange>右键选中状态，禁止左键操作</color>");
                return;
            }

            if (!isLocked && !isLeftMouseButtonClicking)
            {
                CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
                _cardEventTrigger?.TriggerCancelOtherLeftSelect(state?.nowSelectedCard);

                isLocked = true;
                isSelected = true;
                isLeftMouseButtonClicking = true;
                isRightMouseButtonClicking = false;
                if (imgCard != null) imgCard.color = Color.red;
                Debug.Log("<color=red>左键选中卡牌</color>");
                _cardEventTrigger?.TriggerLeftSelect(isSelected);
            }

            if (isLeftMouseButtonClicking && imgCard != null)
            {
                RectTransform cardRect = imgCard.rectTransform;
                uiCamera = eventData.pressEventCamera;
                Vector3 startPos = cardRect.position;
                startPos.z = cardRect.position.z;
                _cardEventTrigger?.TriggerLeftDrawLine(startPos);
            }
        }
        else if (eventData.pointerId == -2) // 右键
        {
            if (isLeftMouseButtonClicking)
            {
                Debug.Log("<color=orange>左键选中状态，禁止右键操作</color>");
                return;
            }

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
                if (imgCard != null) imgCard.color = Color.yellow;
                Debug.Log("<color=yellow>右键选中卡牌</color>");
                _cardEventTrigger?.TriggerRightSelect(true);
            }
            else
            {
                ForceUnlockAndReturn();
                _cardEventTrigger?.TriggerCancelRightSelect();
            }
        }
    }

    public void PlayReleaseAnimation() => Debug.Log("播放卡牌释放动画");
    public void PlayGetAnimator() => Debug.Log("播放卡牌获取动画");

    public void RefreshOriginalPos()
    {
       
        if (rect != null)
        {
            originalAnchoredPos = rect.anchoredPosition;
            isLayoutInitialized = true; // 可在此处设置
            Debug.Log(this.gameObject.name + "更新卡牌ancho布局为" + rect.anchoredPosition);
        }
    }

    public void SetTop() => cardHighlight.SetTop(100);
    public void ResetTop() => cardHighlight.ResetTop();
}


