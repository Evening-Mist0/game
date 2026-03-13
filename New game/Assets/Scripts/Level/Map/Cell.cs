using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格子占用状态
/// </summary>
public enum CellStateType
{
    Occupied,
    None,
}

public enum CellSlectType
{
    Slected,
    Preslected,
    None,
}

[RequireComponent(typeof(CellEffectControl))]
/// <summary>
/// 关卡中，地图的最小单元格
/// </summary>
public class Cell : MonoBehaviour
{
    //世界坐标
    [HideInInspector]
    public Vector2 myWorldPos;
    //网格逻辑坐标
    [HideInInspector]
    public GridPos logicalPos;
    //当前状态
    [HideInInspector]
    public CellStateType nowStateType;
    //自身UI控件
    [HideInInspector]
    public CellEffectControl myUIControl;

    private void Awake()
    {
        myUIControl = GetComponent<CellEffectControl>();
    }

    private void Start()
    {
    
    }

    /// <summary>
    /// 切换单元格的状态
    /// </summary>
    /// <param name="state">需要切换的状态</param>
    public void ChangeStateType(CellStateType state)
    {
        if(nowStateType != state)
            nowStateType = state;
    }

}
