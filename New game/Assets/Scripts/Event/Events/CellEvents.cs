using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单元格事件基类，获取单元格变量（Cell）
/// </summary>
public class CellEventBase : GameEventBase
{
    /// <summary>
    /// 触发事件的单元格实例
    /// </summary>
    public Cell SourceCell { get; protected set; }

    public CellEventBase(Cell sourceCell)
    {
        SourceCell = sourceCell;
    }
}

public class CellUpdateAllowedHighLightEvent : CellEventBase
{
    public bool isAllowed { get; }

    public CellUpdateAllowedHighLightEvent(Cell sourceCell, bool isAllowed) : base(sourceCell)
    {
        this.isAllowed = isAllowed;
    }
}
