using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum E_TowerType
{
    earth_ke,
    earth_po,
    wood_mu,
    wood_lin,
    wood_sen,
    earth_yao,
    water_miao
}
public abstract class BaseDefTower : BaseGameObject
{
    [Header("防御塔基础配置")]
    [Tooltip("防御塔血量")]
    public int maxHP;
    /// <summary>
    /// 当前血量
    /// </summary>
    protected int currentHP;

    /// <summary>
    /// 自身处于哪个单元格
    /// </summary>
    private Cell myCell;

    private void Awake()
    {
        InitValue();
    }

    void InitValue()
    {
        currentHP = maxHP;
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="value">被哪个怪物伤害伤害</param>
    public abstract void Hurt(BaseMonster monster);


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
        myCell.UpdateOccupiedState(CellStateType.DefTowerOccupied, this);
    }
}
