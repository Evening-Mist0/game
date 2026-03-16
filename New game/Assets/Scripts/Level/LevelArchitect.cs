using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelArchitect : BaseMonoMgr<LevelArchitect>
{
    //// <summary>
    /// 创建防御塔
    /// </summary>
    /// <param name="ResName">防御塔预设体资源路径</param>
    /// <param name="cell">放置防御塔的单元格</param>
    public void PlaceDefTower(string resName, Cell cell)
    {

        //如果有占据物，不在这个格子创建
        if (cell.nowStateType != CellStateType.None)
            return;
       

        GameObject obj = Resources.Load<GameObject>(resName);

        if (obj == null)
        {
            Debug.LogWarning($"[关卡建筑师]传入的资源名没有找到对应资源{resName}");
            return;
        }

        obj = Instantiate(obj);
        //创建防御塔并生成在对应位置
        obj.transform.position = cell.myWorldPos;
        BaseDefTower tower = obj.GetComponent<BaseDefTower>();
        //更新防御塔存在的单元格状态
        tower.SetMyCell(cell);
    }

    /// <summary>
    /// 删除防御塔
    /// </summary>
    /// <param name="cell">哪个单元格的防御塔</param>
    public void DeleteDefTower(Cell cell)
    {
        if (cell.nowStateType != CellStateType.DefTowerOccupied)
        {
            Debug.Log($"[关卡建筑师]格子{cell.logicalPos.x}{cell.logicalPos.y}当前没有被建筑物占据，无法删除防御塔");
            return;
        }
            
        BaseDefTower tower = cell.nowObj as BaseDefTower;
        //删除防御塔
        tower.DestroyMe();
    }
}
