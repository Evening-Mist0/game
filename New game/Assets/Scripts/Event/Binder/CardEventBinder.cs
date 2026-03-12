// 路径：Scripts/Binder/CardEventBinder.cs
using UnityEngine;

/// <summary>
/// 卡牌事件绑定器：每个卡牌实例独立，管理自身事件注册/触发
/// 挂载在卡牌预制体上，与BaseCard/PaperBounceControl同节点
/// </summary>
[RequireComponent(typeof(BaseCard), typeof(CardEffectControl))]
public class CardEventBinder : MonoBehaviour
{
    // 当前卡牌实例的核心组件
    private BaseCard _baseCard;

    private void Awake()
    {
        // 获取当前卡牌的组件（多卡牌不冲突）
        _baseCard = GetComponent<BaseCard>();
        //_bounceControl = GetComponent<CardEffectControl>();

        // 注册当前卡牌的事件监听
        RegisterCardEvents();
    }

    /// <summary>
    /// 注册当前卡牌需要监听的事件
    /// </summary>
    private void RegisterCardEvents()
    {
        
    }

    #region 对外提供的事件触发API（供PaperBounceControl调用）
    

    /// <summary>
    /// 触发当前卡牌的左键绘线事件
    /// </summary>
    public void TriggerLeftDrawLine(Vector3 startPos)
    {
        TypeSafeEventCenter.Instance.Trigger(new CardLeftDrawLineEvent(_baseCard, startPos));
    }

    /// <summary>
    /// 触发当前卡牌的左键选中事件
    /// </summary>
    public void TriggerLeftSelect(bool isLeftBtnSelected)
    {
        TypeSafeEventCenter.Instance.Trigger(new CardLeftSelectEvent(_baseCard, isLeftBtnSelected));
    }


    /// <summary>
    /// 触发当前卡牌的左键取消选中事件
    /// </summary>
    public void TriggerCancelLeftSelect()
    {
        TypeSafeEventCenter.Instance.Trigger(new CardCancelLeftSelectEvent(_baseCard));
    }

    /// <summary>
    /// 触发当前卡牌的左键取消其他左键选中卡牌事件
    /// </summary>
    /// <param name="card">要取消选中的牌</param>
    public void TriggerCancelOtherLeftSelect(BaseCard card)
    {
        TypeSafeEventCenter.Instance.Trigger(new CardCancelOhterLeftSelectEvent(card));
    }

    /// <summary>
    /// 触发当前卡牌的右键选中事件
    /// </summary>
    public void TriggerRightSelect(bool isRightBtnSelected)
    {
        TypeSafeEventCenter.Instance.Trigger(new CardRightSelectEvent(_baseCard, isRightBtnSelected));
    }

    /// <summary>
    /// 触发当前卡牌的右键取消选中事件
    /// </summary>
    public void TriggerCancelRightSelect()
    {
        TypeSafeEventCenter.Instance.Trigger(new CardCancelRightSelectEvent(_baseCard));
    }

    /// <summary>
    /// 触发当前卡牌的左键取消其他左键选中卡牌事件
    /// </summary>
    /// <param name="card">要取消选中的牌</param>
    public void TriggerCancelOtherRightSelect(BaseCard card)
    {
        TypeSafeEventCenter.Instance.Trigger(new CardCancelOhterLeftSelectEvent(card));
    }
    #endregion

    ///// <summary>
    ///// 触发当前卡牌的状态变更事件
    ///// </summary>
    //public void TriggerStateChange(bool isSelected)
    //{
    //    TypeSafeEventCenter.Instance.Trigger(new CardSelectStateChangeEvent(_baseCard, isSelected));
    //}

    //#region 事件回调（只处理当前卡牌的事件）
    ///// <summary>
    ///// 卡牌状态变更回调（多卡牌核心：只响应自身事件）
    ///// </summary>
    //private void OnCardStateChange(CardSelectStateChangeEvent evt)
    //{
    //    // 关键过滤：只处理当前卡牌的事件
    //    if (evt.SourceCard != _baseCard) return;

    //    Debug.LogWarning("确认为当前卡牌，进入状态更新");
    //    // 更新当前卡牌的状态
    //    _baseCard.isSelected = evt.IsSelected;
    //    _baseCard.isLeftMouseButtonCliking = evt.IsSelected;
    //    if(evt.IsSelected)
    //    _baseCard.isRightMouseButtonCliking = !_baseCard.isLeftMouseButtonCliking;
    //    else
    //    _baseCard.isRightMouseButtonCliking = _baseCard.isSelected;
    //    _baseCard.selectedType = evt.IsSelected ? E_SelectedType.Fight : E_SelectedType.Idle;
    //    Debug.Log($"[卡牌{_baseCard.cardID}] 状态更新：是否选中={evt.IsSelected}");
    //}
    //#endregion
}