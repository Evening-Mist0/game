// 路径：Scripts/Binder/CardEventBinder.cs
using UnityEngine;

/// <summary>
/// 卡牌事件绑定器：每个卡牌实例独立，管理自身事件注册/触发
/// 挂载在卡牌预制体上，与BaseCard/PaperBounceControl同节点
/// </summary>
public class CardEventTrigger : MonoBehaviour
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
        //位置变化更新
    }

    #region 对外提供的事件触发API（供CardEffectControl调用）
    

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
}