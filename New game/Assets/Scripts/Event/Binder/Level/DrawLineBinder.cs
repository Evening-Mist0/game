
// 路径：Scripts/Binder/GlobalEventBinder.cs
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 游戏关卡内，全局事件绑定器：管理绘线、出牌状态等全局模块的事件
/// 挂载在GameManager上，全局唯一
/// </summary>

public class DrawLineBinder : MonoBehaviour
{
    private void Awake()
    {
        // 注册DrawLine事件
        RegisterDrawLineEvents();
    }

    /// <summary>
    /// 注册全局模块需要监听的事件
    /// </summary>
    private void RegisterDrawLineEvents()
    {
        Debug.Log("注册绘画事件");
        // 绘线管理器监听“卡牌绘线事件”，绑定_drawLineMgr生命周期
        TypeSafeEventCenter.Instance.Register<CardLeftDrawLineEvent>(this, OnCardDrawLine);
        TypeSafeEventCenter.Instance.Register<CardCancelLeftSelectEvent>(this, CancelDrawLine);
        TypeSafeEventCenter.Instance.Register<CardCancelRightSelectEvent>(this, CancelDrawLine);
    }

    #region 画线事件回调
    /// <summary>
    /// 绘线管理器：响应卡牌绘线事件
    /// </summary>
    private void OnCardDrawLine(CardLeftDrawLineEvent evt)
    {
        Debug.Log($"[绘线管理器] 卡牌{evt.SourceCard.cardID}触发绘线，起始位置={evt.DrawStartPos}");
        DrawLineMgr.Instance.EnterDrawing(evt.DrawStartPos);
    }

    private void CancelDrawLine(CardCancelLeftSelectEvent evt)
    {
        DrawLineMgr.Instance.ExitDrawing();
    }

    private void CancelDrawLine(CardCancelRightSelectEvent evt)
    {
        DrawLineMgr.Instance.ExitDrawing();
    }

    #endregion
}