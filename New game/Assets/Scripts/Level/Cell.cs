using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellStateType
{
    occupied,
    none,
}

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
