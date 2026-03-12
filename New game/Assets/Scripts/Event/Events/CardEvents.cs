// 路径：Scripts/Event/CardEvents.cs
using UnityEngine;

/// <summary>
/// 卡牌事件基类，获取卡牌变量（BaseCard）
/// </summary>
public class CardEventBase : GameEventBase
{
    /// <summary>
    /// 触发事件的卡牌实例（多卡牌场景核心标识）
    /// </summary>
    public BaseCard SourceCard { get; protected set; }

    public CardEventBase(BaseCard sourceCard)
    {
        SourceCard = sourceCard;
    }
}


#region 卡牌交互事件

/// <summary>
/// 卡牌左键绘线事件（传递绘线起始位置+触发卡牌）
/// </summary>
public class CardLeftDrawLineEvent : CardEventBase
{
    public Vector3 DrawStartPos { get; }

    public CardLeftDrawLineEvent(BaseCard sourceCard, Vector3 drawStartPos) : base(sourceCard)
    {
        DrawStartPos = drawStartPos;
    }
}

/// <summary>
/// 左键取消 当前左键选中卡牌 事件（传递当前被左键选中卡牌）
/// </summary>
public class CardCancelOhterLeftSelectEvent : CardEventBase
{
    public CardCancelOhterLeftSelectEvent(BaseCard sourceCard) : base(sourceCard) { }
}

/// <summary>
/// 右键取消 当前右键选中卡牌 事件（传递当前被右键选中卡牌）
/// </summary>
public class CardCancelOhterRightSelectEvent : CardEventBase
{
    public CardCancelOhterRightSelectEvent(BaseCard sourceCard) : base(sourceCard) { }
}




/// <summary>
/// 卡牌左键选中状态变更事件（传递是否选中+目标卡牌）
/// </summary>
public class CardLeftSelectEvent : CardEventBase
{
    public bool IsLeftBtnSelected { get; }

    public CardLeftSelectEvent(BaseCard sourceCard, bool isLeftBtnSelected) : base(sourceCard)
    {
        IsLeftBtnSelected = isLeftBtnSelected;
    }
}

/// <summary>
/// 卡牌取消左键点击选中事件（传递是否选中+目标卡牌）
/// </summary>
public class CardCancelLeftSelectEvent : CardEventBase
{
    public CardCancelLeftSelectEvent(BaseCard sourceCard) : base(sourceCard) { }
}

/// <summary>
/// 卡牌右键选中事件（传递是否选中+目标卡牌）
/// </summary>
public class CardRightSelectEvent : CardEventBase
{
    public bool IsRightBtnSelected { get; }

    public CardRightSelectEvent(BaseCard sourceCard, bool isRightBtnSelected) : base(sourceCard) 
    {
        IsRightBtnSelected = isRightBtnSelected;
    }
}

/// <summary>
/// 卡牌取消右键点击选中事件（传递是否选中+目标卡牌）
/// </summary>
public class CardCancelRightSelectEvent : CardEventBase
{
    public CardCancelRightSelectEvent(BaseCard sourceCard) : base(sourceCard) { }
}


#endregion