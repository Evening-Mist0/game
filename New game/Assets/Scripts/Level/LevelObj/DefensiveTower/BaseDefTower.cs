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
public abstract class BaseDefTower : BaseLevelObject
{
    [Header("防御塔基础配置")]
    [Tooltip("防御塔血量")]
    public int maxHP;
    /// <summary>
    /// 当前血量
    /// </summary>
    private int currentHP;

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
    /// <param name="value">具体的伤害值</param>
    public void Hurt(int value)
    {
        currentHP -= value;
        Debug.Log($"[防御塔]防御塔受到伤害{value},现在剩余血量{currentHP}");
        if (currentHP <= 0)
            DestroyMe();
    }

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
