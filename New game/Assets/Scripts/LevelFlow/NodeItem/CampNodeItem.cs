using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 休整营地节点
/// 核心逻辑：进入节点→打开营地面板→选择调息/悟道→完成节点
/// </summary>
public class CampNodeItem : BaseNodeItem
{
    public enum E_CampOption
    {
        TiaoXi,
        WuDao
    }

    private bool _isWaitingForOption = false;      // 是否正在等待营地选项
    private bool _isWaitingForBookSelect = false; // 是否正在等待典籍选择（悟道后）

    protected override void Awake()
    {
        base.Awake();
        EventCenter.Instance.AddEventListener<(E_CampOption option, string nodeId)>(E_EventType.Camp_OptionConfirm, OnCampOptionConfirm);
        // 监听典籍选择确认
        EventCenter.Instance.AddEventListener<E_BookType>(E_EventType.UI_BookSelectConfirm, OnBookSelected);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventCenter.Instance.RemoveEventListener<(E_CampOption option, string nodeId)>(E_EventType.Camp_OptionConfirm, OnCampOptionConfirm);
        EventCenter.Instance.RemoveEventListener<E_BookType>(E_EventType.UI_BookSelectConfirm, OnBookSelected);
    }

    protected override void OnNodeClick()
    {
        // 如果正在等待选项或典籍选择，禁止再次点击
        if (_isWaitingForOption || _isWaitingForBookSelect)
        {
            Debug.LogWarning("营地正在等待选择，不能重复进入");
            return;
        }
        base.OnNodeClick();

        // 显示营地面板，并传入节点ID
        UIMgr.Instance.ShowPanel<CampPanel>(E_UILayerType.middle);
        var campPanel = UIMgr.Instance.GetPanel<CampPanel>();
        campPanel.ShowWithNodeId(nodeId);
        //并进入等待状态
        _isWaitingForOption = true;
    }

    // 监听营地选项确认事件
    private void OnCampOptionConfirm((E_CampOption option, string nodeId) data)
{
    if (data.nodeId != this.nodeId) return;

    switch (data.option)
    {
        case E_CampOption.TiaoXi:
            LevelFlowMgr.Instance.CompleteNode(nodeId);
            break;
        case E_CampOption.WuDao:
            // 获取4本未拥有的典籍
            var bookOptions = GrowthMgr.Instance.GetRandomUnownedBooks(4);
            int needSelect = Mathf.Min(2, bookOptions.Count); // 实际需要选择的数量
            // 打开典籍选择面板（多选）
            UIMgr.Instance.ShowPanel<BookSelectPanel>(E_UILayerType.middle, (bookSelectPanel) =>
            {
                bookSelectPanel.Init(bookOptions, needSelect ,(selectedBooks) =>
                {
                    // 添加选中的两本典籍
                    foreach (var book in selectedBooks)
                    {
                        GrowthMgr.Instance.AddBook(book.bookId);
                        EventCenter.Instance.EventTrigger(E_EventType.Growth_GetBook, book);
                    }
                    LevelFlowMgr.Instance.CompleteNode(nodeId);
                });
            }); 
            break;
    }
}

    private void OnBookSelected(E_BookType bookType)
    {
        if (!_isWaitingForBookSelect) return;

        // 添加典籍
        GrowthMgr.Instance.AddBook(bookType);
        var book = GrowthMgr.Instance.GetBookConfig(bookType);
        if (book != null)
        {
            EventCenter.Instance.EventTrigger(E_EventType.Growth_GetBook, book);
        }

        // 完成节点
        LevelFlowMgr.Instance.CompleteNode(nodeId);
        UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();

        _isWaitingForBookSelect = false;
    }
}
