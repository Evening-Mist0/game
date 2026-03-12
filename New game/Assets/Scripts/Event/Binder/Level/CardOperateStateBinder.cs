// 路径：Scripts/Binder/GlobalEventBinder.cs
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 游戏关卡内，全局事件绑定器：管理绘线、出牌状态等全局模块的事件
/// 挂载在GameManager上，全局唯一
/// </summary>

public class CardOperateStateBinder : BaseLevelStateBinder
{
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
    }

    #region 事件回调

    /// <summary>
    /// 出牌状态：响应卡牌左键选中
    /// </summary>
    private void OnCardLeftSelect(CardLeftSelectEvent evt)
    {

        if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
        {
            Debug.LogError("无法里氏替换为CardOperateState");
            return;
        }

        CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
        state.nowSelectedCard = evt.SourceCard;
        state.nowSelectedCard.isSelected = true;
        state.nowSelectedCard.isLeftMouseButtonCliking = true;
        state.nowSelectedCard.isRightMouseButtonCliking = false;
        
        Debug.Log($"[出牌状态] 选中卡牌{evt.SourceCard.cardID}");
    }

    /// <summary>
    /// 出牌状态：响应卡牌右键选中
    /// </summary>
    private void OnCardRightSelect(CardRightSelectEvent evt)
    {
        if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
        {
            Debug.LogError("无法里氏替换为CardOperateState");
            return;
        }


        Debug.Log("进入右键选中状态,当前的evt" + evt.SourceCard.cardID);
       
        CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;

        state.AddCardInCompositeList(evt.SourceCard);
        state.nowSelectedCard = evt.SourceCard;
        state.nowSelectedCard.isSelected = true;
        state.nowSelectedCard.isLeftMouseButtonCliking = false;
        state.nowSelectedCard.isRightMouseButtonCliking = true;
        Debug.Log($"[出牌状态] 右键选中卡牌{evt.SourceCard.cardID}，合成列表数量={state.CardCompositeList.Count}");
    }


    /// <summary>
    /// 出牌状态：响应卡牌左键取消选中(打出卡牌)
    /// </summary>
    private void OnCardCancelLeftSelect(CardCancelLeftSelectEvent evt)
    {
        if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
        {
            Debug.LogError("无法里氏替换为CardOperateState");
            return;
        }

        CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
        
        evt.SourceCard.isSelected = false;
        evt.SourceCard.isLeftMouseButtonCliking = false;
        evt.SourceCard.isRightMouseButtonCliking = false;
        Debug.Log($"[出牌状态] 打出卡牌或取消右键选牌{evt.SourceCard.cardID}");
    }

    /// <param name="evt">要取消选中的卡牌</param>
    private void OnCardCancelOtherLeftSelect(CardCancelOhterLeftSelectEvent evt)
    {
        if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
        {
            Debug.LogError("无法里氏替换为CardOperateState");
            return;
        }

        if(evt.SourceCard != null)
        {
            CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
            evt.SourceCard.cardEffectControl.ForceUnlockAndReturn();
            evt.SourceCard.isSelected = false;
            evt.SourceCard.isLeftMouseButtonCliking = false;
            evt.SourceCard.isRightMouseButtonCliking = false;
        }
       
    }

    /// <param name="evt">要取消选中的卡牌</param>
    private void OnCardCancelOtherRightSelect(CardCancelOhterRightSelectEvent evt)
    {
        if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
        {
            Debug.LogError("无法里氏替换为CardOperateState");
            return;
        }

        if (evt.SourceCard != null)
        {
            CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
            evt.SourceCard.cardEffectControl.ForceUnlockAndReturn();
            evt.SourceCard.isSelected = false;
            evt.SourceCard.isLeftMouseButtonCliking = false;
            evt.SourceCard.isRightMouseButtonCliking = false;
        }
    }

    /// <summary>
    /// 出牌状态：响应卡牌右键取消选中
    /// </summary>
    private void OnCardCancelRightSelect(CardCancelRightSelectEvent evt)
    {
        if (!(LevelStepMgr.Instance.machine.nowState is CardOperateState))
        {
            Debug.LogError("无法里氏替换为CardOperateState");
            return;
        }

        CardOperateState state = LevelStepMgr.Instance.machine.nowState as CardOperateState;
        state.RemoveCardInCompositeList(evt.SourceCard);
        evt.SourceCard.isSelected = false;
        evt.SourceCard.isLeftMouseButtonCliking = false;
        evt.SourceCard.isRightMouseButtonCliking = false;
        Debug.Log($"[出牌状态] 取消选中卡牌{evt.SourceCard.cardID}，合成列表数量={state.CardCompositeList.Count}");
    }

    private void OnCardCompositeSuccess(CardCompositeSuccessEvent evt)
    {
        Debug.Log("触发合成成功音效");
        Debug.Log("触发合成成功动画");
    }

    protected override void Init()
    {
        base.Init();
    }



    #endregion
}