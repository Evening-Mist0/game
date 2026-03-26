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
    EntityOccupied,
    /// <summary>
    /// 没有被占据
    /// </summary>
    None,
    /// <summary>
    /// 被幽灵占据
    /// </summary>
    GhostOccupied
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
public class Cell : BaseGameObject
{
    //世界坐标
    //[HideInInspector]
    public Vector3 myWorldPos;
    //网格逻辑坐标
    [HideInInspector]
    public GridPos logicalPos;
    //当前状态
    public CellStateType nowStateType;
    //自身UI控件
    [HideInInspector]
    public CellEffectControl myUIControl;
    //当前格子上的物体
    public BaseGameObject nowObj;

    public override E_GameObjectType gameObjectType => E_GameObjectType.Cell;

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

        //记录格子上存在的对象
        switch (nowObj.gameObjectType)
        {
            case E_GameObjectType.Player:
                nowStateType = CellStateType.PlayerOccupied;
                this.nowObj = nowObj;

                break;
            case E_GameObjectType.Monster:
                nowStateType = CellStateType.MonsterOccupied;
                this.nowObj = nowObj;

                break;
            case E_GameObjectType.DefTower://如果是幽灵防御塔，格子不应该为占据状态
                 BaseDefTower tower = nowObj as BaseDefTower;
                Debug.Log($"防御塔{tower.gameObject.name},枚举为{tower.myTowerType}");
                switch (tower.myTowerType)
                {
                    case E_TowerType.Entity:
                        nowStateType = CellStateType.EntityOccupied;
                        this.nowObj = nowObj;
                        break;
                    case E_TowerType.Ghost:
                        //如果是幽灵对象，格子仍然置空
                        Debug.Log($"检测到对象是幽灵防御塔{tower.gameObject.name}");
                        nowStateType = CellStateType.GhostOccupied;
                        break;
                }

                break;
            default://这种情况只有bug的时候才会出现,因为有对象就会记录占据类型
                Debug.LogError("传入的对象的当前Cell状态为None,这是bug");
                nowStateType = CellStateType.None;
                this.nowObj = null;
                break;
        }
    }
}