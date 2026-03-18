using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格子占用状态
/// </summary>
public enum CellStateType
{
    /// <summary>
    /// 被玩家占据
    /// </summary>
    PlayerOccupied,
    /// <summary>
    /// 被怪物占据
    /// </summary>
    MonsterOccupied,
    /// <summary>
    /// 被防御塔占据
    /// </summary>
    DefTowerOccupied,
    /// <summary>
    /// 没有被占据
    /// </summary>
    None,
}

public enum CellSlectType
{
    /// <summary>
    /// 选择到当前单元格
    /// </summary>
    Slected,
    /// <summary>
    /// 预选到当前单元格
    /// </summary>
    Preslected,
    /// <summary>
    /// 没有选择到当前单元格
    /// </summary>
    None,
}

[RequireComponent(typeof(CellEffectControl))]
/// <summary>
/// 关卡中，地图的最小单元格
/// </summary>
public class Cell : MonoBehaviour
{
    //世界坐标
    //[HideInInspector]
    public Vector3 myWorldPos;
    //网格逻辑坐标
    [HideInInspector]
    public GridPos logicalPos;
    //当前状态
    [HideInInspector]
    public CellStateType nowStateType;
    //自身UI控件
    [HideInInspector]
    public CellEffectControl myUIControl;
    //当前格子上的物体
    public BaseGameObject nowObj;

    private void Awake()
    {
        myUIControl = GetComponent<CellEffectControl>();
    }

    private void Start()
    {
    
    }

    public void InitMyValue(Vector2 myWorldPos, GridPos logicalPos)
    {
        this.myWorldPos = myWorldPos;
        this.logicalPos = logicalPos;
        nowStateType = CellStateType.None;
    }

    /// <summary>
    /// 更新当前单元格被占有的状态
    /// </summary>
    /// <param name="nowObj">占有该单元格的物体()</param>
    public void UpdateOccupiedState(CellStateType state, BaseGameObject nowObj)
    {
        if (nowObj == null)
        {
            this.nowObj = nowObj;
            nowStateType = CellStateType.None;
            return;
        }
            

        switch (nowObj.gameObjectType)
        {
            case E_GameObjectType.Player:
                nowStateType = CellStateType.PlayerOccupied;
                break;
            case E_GameObjectType.Monster:
                nowStateType = CellStateType.MonsterOccupied;
                break;
            case E_GameObjectType.DefTower:
                nowStateType = CellStateType.DefTowerOccupied;
                break;
            default:
                nowStateType = CellStateType.None;
                break;
        }
        this.nowObj = nowObj;

    }
}