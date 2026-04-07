using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelArchitect : BaseMonoMgr<LevelArchitect>
{

    /// 创建防御塔
    /// </summary>
    /// <param name="resName">防御塔资源路径</param>
    /// <param name="cell">放置防御塔的单元格</param>
    /// <param name="extraHp">额外的防御塔血量加成</param>
    public void PlaceDefTower(string resName, Cell cell,int extraHp)
    {
        GameObject obj = Resources.Load<GameObject>(resName);
        BaseDefTower tower = obj.GetComponent<BaseDefTower>();

        if (obj == null)
        {
            Debug.LogWarning($"[关卡建筑师]传入的资源名没有找到对应资源{resName}");
            return;
        }
     
        if(tower.myTowerType == E_TowerType.Ghost)//如果是幽灵类型的防御塔,可以创建
        {
            if (cell.nowStateType == CellStateType.EntityOccupied)//如果这个格子是防御塔,则不创建
                return;
        }
        else if(cell.nowStateType != CellStateType.None)//如果不是幽灵防御塔,且格子被占据,则不能创建
        {
            return;
        }


        GameObject realObj = Instantiate(obj);
        //创建防御塔并生成在对应位置
        realObj.transform.position = cell.myWorldPos;
        tower = realObj.GetComponent<BaseDefTower>();

      
        tower.maxHP += extraHp;
        tower.currentHP = tower.maxHP;
        tower.effectControl.UpdateBlood(tower.currentHP,tower.maxHP);
        //更新防御塔存在的单元格状态
        tower.SetMyCell(cell);


    }

    /// <summary>
    /// 删除防御塔
    /// </summary>
    /// <param name="cell">哪个单元格的防御塔</param>
    public void DeleteDefTower(Cell cell)
    {
        if (cell.nowStateType != CellStateType.EntityOccupied)
        {
            Debug.Log($"[关卡建筑师]格子{cell.logicalPos.x}{cell.logicalPos.y}当前没有被建筑物占据，无法删除防御塔");
            return;
        }
            
        BaseDefTower tower = cell.nowObj as BaseDefTower;
        //删除防御塔
        tower.DestroyMe();
    }
}
