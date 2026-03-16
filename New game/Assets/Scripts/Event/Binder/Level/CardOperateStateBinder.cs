// 路径：Scripts/Binder/GlobalEventBinder.cs
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 游戏关卡内，全局事件绑定器：管理绘线、出牌状态等全局模块的事件
/// 挂载在GameManager上，全局唯一
/// </summary>

public class CardOperateStateBinder : BaseLevelStateBinder
{
    //作为成员变量优化性能
    CardOperateState cachedState;
    /// <summary>
    /// 注册全局模块需要监听的事件
    /// </summary>
    protected override void RegisterOperateEvents()
    {
        // 出牌状态监听“卡牌左键选中事件”
        TypeSafeEventCenter.Instance.Register<CardLeftSelectEvent>(LevelStepMgr.Instance, OnCardLeftSelect);

        // 出牌状态监听“卡牌右键选中事件”
        TypeSafeEventCenter.Instance.Register<CardRightSelectEvent>(LevelStepMgr.Instance, OnCardRightSelect);

        // 出牌状态监听“卡牌取消左键选中事件”
        TypeSafeEventCenter.Instance.Register<CardCancelLeftSelectEvent>(LevelStepMgr.Instance, OnCardCancelLeftSelect);

        // 出牌状态监听“卡牌取消右键选中事件”
        TypeSafeEventCenter.Instance.Register<CardCancelRightSelectEvent>(LevelStepMgr.Instance, OnCardCancelRightSelect);

        // 出牌状态监听“卡牌在左键选中卡牌后，再用左键点其他卡牌事件”
        TypeSafeEventCenter.Instance.Register<CardCancelOhterLeftSelectEvent>(LevelStepMgr.Instance, OnCardCancelOtherLeftSelect);

        // 出牌状态监听“卡牌在右键选中卡牌后，再用右键点其他卡牌事件”
        TypeSafeEventCenter.Instance.Register<CardCancelOhterRightSelectEvent>(LevelStepMgr.Instance, OnCardCancelOtherRightSelect);

        // 出牌状态监听“卡牌合成成功事件”
        TypeSafeEventCenter.Instance.Register<CardCompositeSuccessEvent>(LevelStepMgr.Instance, OnCardCompositeSuccess);

        //左键单击后打出卡牌状态监听“格子是否可以显示高亮”事件
        //TypeSafeEventCenter.Instance.Register<CellUpdateAllowedHighLightEvent>(LevelStepMgr.Instance, OnCellUpdateAllowedHighLight);

        //// 出牌状态监听“卡牌打出成功事件” 该事件用事件中心反而复杂，暂时搁置
        //TypeSafeEventCenter.Instance.Register<CardReleaseSuccessEvent>(LevelStepMgr.Instance, OnCardReleaseSuccess);
    }

    #region 事件回调函数
    /// <summary>
    /// 出牌状态：响应卡牌左键选中
    /// </summary>
    private void OnCardLeftSelect(CardLeftSelectEvent evt)
    {
        if (!TryGetCurrentState(out CardOperateState state)) return;

        state.nowSelectedCard = evt.SourceCard;
        state.nowSelectedCard.isSelected = true;
        state.nowSelectedCard.isLeftMouseButtonCliking = true;
        state.nowSelectedCard.isRightMouseButtonCliking = false;

        //允许格子高亮
        state.isAllowedCellHighLight = true;

        Debug.Log($"[出牌状态] 选中卡牌{evt.SourceCard.cardID}");
    }

    /// <summary>
    /// 出牌状态：响应卡牌右键选中
    /// </summary>
    private void OnCardRightSelect(CardRightSelectEvent evt)
    {
        if (!TryGetCurrentState(out CardOperateState state)) return;

        Debug.Log("进入右键选中状态,当前的evt" + evt.SourceCard.cardID);

        state.AddCardInCompositeList(evt.SourceCard);
        state.nowSelectedCard = evt.SourceCard;
        state.nowSelectedCard.isSelected = true;
        state.nowSelectedCard.isLeftMouseButtonCliking = false;
        state.nowSelectedCard.isRightMouseButtonCliking = true;

        //取消格子高亮
        state.isAllowedCellHighLight = false;

        Debug.Log($"[出牌状态] 右键选中卡牌{evt.SourceCard.cardID}，合成列表数量={state.CardCompositeList.Count}");
    }

    /// <summary>
    /// 出牌状态：响应卡牌左键取消选中(打出卡牌)
    /// </summary>

    private void OnCardCancelLeftSelect(CardCancelLeftSelectEvent evt)
    {
        if (!TryGetCurrentState(out CardOperateState state)) return;
        if (evt.SourceCard == null) return;

        evt.SourceCard.isSelected = false;
        evt.SourceCard.isLeftMouseButtonCliking = false;
        evt.SourceCard.isRightMouseButtonCliking = false;

        //取消格子高亮
        state.isAllowedCellHighLight = false;

        Debug.Log($"[出牌状态] 打出卡牌或取消右键选牌{evt.SourceCard.cardID}");
    }


    /// <summary>
    /// 左键取消当前选中的牌（用于已经选择了一张牌，选另外一张牌时，前一张牌应当撤回）
    /// </summary>
    /// <param name="evt"></param>
    private void OnCardCancelOtherLeftSelect(CardCancelOhterLeftSelectEvent evt)
    {
        if (!TryGetCurrentState(out CardOperateState state)) return;
        if (evt.SourceCard == null) return;

        evt.SourceCard.cardEffectControl.ForceUnlockAndReturn();
        evt.SourceCard.isSelected = false;
        evt.SourceCard.isLeftMouseButtonCliking = false;
        evt.SourceCard.isRightMouseButtonCliking = false;

        //取消格子高亮
        state.isAllowedCellHighLight = false;
    }

    /// <summary>
    /// 右键取消当前选中的牌（用于已经选择了一张牌，选另外一张牌时，前一张牌应当撤回）
    /// </summary>
    /// <param name="evt">要命令其撤回的牌</param>

    private void OnCardCancelOtherRightSelect(CardCancelOhterRightSelectEvent evt)
    {
        if (!TryGetCurrentState(out CardOperateState state)) return;
        if (evt.SourceCard == null) return;

        evt.SourceCard.cardEffectControl.ForceUnlockAndReturn();
        evt.SourceCard.isSelected = false;
        evt.SourceCard.isLeftMouseButtonCliking = false;
        evt.SourceCard.isRightMouseButtonCliking = false;

        //允许格子高亮
        state.isAllowedCellHighLight = true;
    }

    /// <summary>
    /// 出牌状态：响应卡牌右键取消选中
    /// </summary>
    private void OnCardCancelRightSelect(CardCancelRightSelectEvent evt)
    {
        if (!TryGetCurrentState(out CardOperateState state)) return;
        if (evt.SourceCard == null) return;

        state.RemoveCardInCompositeList(evt.SourceCard);
        evt.SourceCard.isSelected = false;
        evt.SourceCard.isLeftMouseButtonCliking = false;
        evt.SourceCard.isRightMouseButtonCliking = false;

        //取消格子高亮
        state.isAllowedCellHighLight = false;

        Debug.Log($"[出牌状态] 取消选中卡牌{evt.SourceCard.cardID}，合成列表数量={state.CardCompositeList.Count}");
    }

    private void OnCardCompositeSuccess(CardCompositeSuccessEvent evt)
    {
        Debug.Log("触发合成成功音效");
        Debug.Log("触发合成成功动画");
    }

    #endregion

    //#region 事件回调

    ///// <summary>
    ///// 出牌状态：响应卡牌左键选中
    ///// </summary>
    //private void OnCardLeftSelect(CardLeftSelectEvent evt)
    //{

    //    if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
    //    {
    //        Debug.LogError("无法里氏替换为CardOperateState");
    //        return;
    //    }



    //    CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
    //    //调整卡牌状态
    //    state.nowSelectedCard = evt.SourceCard;
    //    state.nowSelectedCard.isSelected = true;
    //    state.nowSelectedCard.isLeftMouseButtonCliking = true;
    //    state.nowSelectedCard.isRightMouseButtonCliking = false;

    //    Debug.Log($"[出牌状态] 选中卡牌{evt.SourceCard.cardID}");
    //}

    ///// <summary>
    ///// 出牌状态：响应卡牌右键选中
    ///// </summary>
    //private void OnCardRightSelect(CardRightSelectEvent evt)
    //{
    //    if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
    //    {
    //        Debug.LogError("无法里氏替换为CardOperateState");
    //        return;
    //    }


    //    Debug.Log("进入右键选中状态,当前的evt" + evt.SourceCard.cardID);

    //    CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;

    //    state.AddCardInCompositeList(evt.SourceCard);
    //    state.nowSelectedCard = evt.SourceCard;
    //    state.nowSelectedCard.isSelected = true;
    //    state.nowSelectedCard.isLeftMouseButtonCliking = false;
    //    state.nowSelectedCard.isRightMouseButtonCliking = true;
    //    Debug.Log($"[出牌状态] 右键选中卡牌{evt.SourceCard.cardID}，合成列表数量={state.CardCompositeList.Count}");
    //}


    ///// <summary>
    ///// 出牌状态：响应卡牌左键取消选中(打出卡牌)
    ///// </summary>
    //private void OnCardCancelLeftSelect(CardCancelLeftSelectEvent evt)
    //{
    //    if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
    //    {
    //        Debug.LogError("无法里氏替换为CardOperateState");
    //        return;
    //    }

    //    CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;

    //    if (evt.SourceCard != null)
    //    {
    //        evt.SourceCard.isSelected = false;
    //        evt.SourceCard.isLeftMouseButtonCliking = false;
    //        evt.SourceCard.isRightMouseButtonCliking = false;
    //        Debug.Log($"[出牌状态] 打出卡牌或取消右键选牌{evt.SourceCard.cardID}");
    //    }

    //}

    ///// <summary>
    ///// 左键取消当前选中的牌（用于已经选择了一张牌，选另外一张牌时，前一张牌应当撤回）
    ///// </summary>
    ///// <param name="evt">要命令其撤回的牌</param>
    //private void OnCardCancelOtherLeftSelect(CardCancelOhterLeftSelectEvent evt)
    //{
    //    if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
    //    {
    //        Debug.LogError("无法里氏替换为CardOperateState");
    //        return;
    //    }


    //    //CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
    //    //state.cellIsAllowedEnterHighLight = true;

    //    if (evt.SourceCard != null)
    //    {
    //        evt.SourceCard.cardEffectControl.ForceUnlockAndReturn();
    //        evt.SourceCard.isSelected = false;
    //        evt.SourceCard.isLeftMouseButtonCliking = false;
    //        evt.SourceCard.isRightMouseButtonCliking = false;
    //    }

    //}


    ///// <summary>
    ///// 右键取消当前选中的牌（用于已经选择了一张牌，选另外一张牌时，前一张牌应当撤回）
    ///// </summary>
    ///// <param name="evt">要命令其撤回的牌</param>
    //private void OnCardCancelOtherRightSelect(CardCancelOhterRightSelectEvent evt)
    //{
    //    if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
    //    {
    //        Debug.LogError("无法里氏替换为CardOperateState");
    //        return;
    //    }

    //    if (evt.SourceCard != null)
    //    {
    //        CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
    //        evt.SourceCard.cardEffectControl.ForceUnlockAndReturn();
    //        evt.SourceCard.isSelected = false;
    //        evt.SourceCard.isLeftMouseButtonCliking = false;
    //        evt.SourceCard.isRightMouseButtonCliking = false;
    //    }
    //}

    ///// <summary>
    ///// 出牌状态：响应卡牌右键取消选中
    ///// </summary>
    //private void OnCardCancelRightSelect(CardCancelRightSelectEvent evt)
    //{
    //    if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
    //    {
    //        Debug.LogError("无法里氏替换为CardOperateState");
    //        return;
    //    }

    //    if (evt.SourceCard != null)
    //    {
    //        CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
    //        state.RemoveCardInCompositeList(evt.SourceCard);
    //        evt.SourceCard.isSelected = false;
    //        evt.SourceCard.isLeftMouseButtonCliking = false;
    //        evt.SourceCard.isRightMouseButtonCliking = false;
    //        Debug.Log($"[出牌状态] 取消选中卡牌{evt.SourceCard.cardID}，合成列表数量={state.CardCompositeList.Count}");
    //    }
    //}

    //private void OnCardCompositeSuccess(CardCompositeSuccessEvent evt)
    //{
    //    Debug.Log("触发合成成功音效");
    //    Debug.Log("触发合成成功动画");
    //}

    //private void OnCellUpdateAllowedHighLight(CellUpdateAllowedHighLightEvent evt)
    //{
    //    CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;

    //}

    //private void OnCardReleaseSuccess(CardReleaseSuccessEvent evt)
    //{
    //    Debug.Log("卡牌成功打出，进行范围辐射检测");
    //}

    //#endregion

    protected override void Init()
    {
        base.Init();
    }

    ///// <summary>
    ///// 获取成员变量state的当前状态，如果是CardOperateState
    ///// </summary>
    ///// <param name="state"></param>
    ///// <returns></returns>
    //private bool TryGetCurrentState(out CardOperateState state)
    //{
    //    state = null;

    //    // 全局单例判空
    //    if (LevelStepMgr.Instance == null || LevelStepMgr.Instance.machine == null)
    //        return false;

    //    var nowState = LevelStepMgr.Instance.machine.nowState;

    //    // 不是当前状态 → 直接返回
    //    if (!(nowState is CardOperateState))
    //        return false;

    //    // 是 → 赋值缓存
    //    cachedState = nowState as CardOperateState;
    //    state = cachedState;
    //    return true;
    //}
}