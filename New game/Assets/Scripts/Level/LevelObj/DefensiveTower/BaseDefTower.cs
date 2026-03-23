using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 防御塔的类型
/// </summary>
public enum E_TowerType
{
    /// <summary>
    /// 实体，可以阻挡怪物
    /// </summary>
    Entity,
    /// <summary>
    /// 幽灵，怪物可以穿过
    /// </summary>
    Ghost,
}
public abstract class BaseDefTower : BaseGameObject
{
    [Header("防御塔基础配置")]
    [Tooltip("防御塔血量")]
    public int maxHP;
    [Tooltip("防御塔类型")]
    public E_TowerType myTowerType;

    /// <summary>
    /// 当前血量
    /// </summary>
    protected int currentHP;


    /// <summary>
    /// 自身处于哪个单元格
    /// </summary>
    public Cell myCell;

    protected virtual void Awake()
    {
        InitValue();
    }

    protected virtual void InitValue()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="value">被哪个怪物伤害伤害</param>
    public abstract void Hurt(BaseMonsterCore monster);


    /// <summary>
    /// 销毁自己
    /// </summary>
    public void DestroyMe()
    {
        myCell.UpdateOccupiedState(CellStateType.None, null);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 设置该防御塔在哪个单元格（重要）
    /// </summary>
    /// <param name="myCell">防御卡处于的单元格</param>
    public void SetMyCell(Cell myCell)
    {
        this.myCell = myCell;
        myCell.UpdateOccupiedState(CellStateType.EntityOccupied, this);
    }
}
