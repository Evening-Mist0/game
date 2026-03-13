using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 格子网格逻辑的坐标
/// </summary>
public struct GridPos
{
    public int x;
    public int y;

    public GridPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

/// <summary>
/// 逻辑层面的网格管理：生成网格，通过GridPos获取网格
/// </summary>
public class GridMgr : BaseMonoMgr<GridMgr>
{

    [Header("格子地图基础配置")]
    [Tooltip("生成格子的原点")]
    private Vector3 origin = new Vector3(-689,-100,0);

    private GameObject gridsRoot;
    [Tooltip("格子宽间距")]
    public float gridWide;
    [Tooltip("格子高间距")]
    public float gridHigh;
    [Tooltip("格子横向数量")]
    public int gridWideCount;
    [Tooltip("格子纵向数量")]
    public int gridHighCount;

    //当前点击选中的格子
    [HideInInspector]
    private Cell nowCell;

    //格子加载路径
    private string cellRes = "Level/Cell";
    public string CellRes
    {
        get { return cellRes; } 
    }

  

    public Dictionary<GridPos, Cell> plotDic = new Dictionary<GridPos, Cell>();



    /// <summary>
    /// 创建格子地图
    /// </summary>
    /// <param name="length">行</param>
    /// <param name="wide">列</param>
    public void CreatGridMap()
    {
        if (gridsRoot == null)
        {
            gridsRoot = new GameObject();
            gridsRoot.name = "GridsRoot";
            UIMgr.Instance.ShowPanel<MapGridPanel>();
            gridsRoot.transform.SetParent(UIMgr.Instance.GetPanel<MapGridPanel>().transform);
            Debug.Log("origin坐标" + origin);
            gridsRoot.transform.localPosition = origin;
        }


        for (int i = 0; i < gridWideCount; i++)
        {
            for(int j = 0; j < gridHighCount; j++)
            {
                //实例化格子对象
                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(CellRes), gridsRoot.transform,false);
                obj.name = "plot_" + j + "_" + i;
                Cell plot = obj.GetComponent<Cell>();
                if (plot == null)
                {
                    Debug.LogError("物体没有挂载Plot脚本，请挂载");
                    return;
                }

                //初始化每个格子的位置,获取格子世界坐标
                Vector2 newPos = new Vector2(gridWide * i, gridHigh * j);
                obj.transform.localPosition = newPos;
                plot.myWorldPos = newPos;

                //获取格子的逻辑坐标
                GridPos gridPos = new GridPos(j,i);
                plot.logicalPos = gridPos;
                
                //添加到字典用于逻辑层面的管理
                plotDic.Add(gridPos, obj.GetComponent<Cell>());
            }
        }
    }

    /// <summary>
    /// 生成卡牌检测范围
    /// </summary>
    /// <param name="cell">玩家点击的单元格</param>
    /// <param name="card">使用的卡牌</param>
    public void CreatCheckRange(Cell cell,BaseCard card)
    {
        nowCell = cell;
        switch (card.CardRangeType)
        {
            case E_CardRangeType.Rectangle:
                CreatRectangleRange(card.currentRecRangeWide,card.currentRecRangeHigh);
                break;
            case E_CardRangeType.MySelf:
                Debug.Log("该卡牌对自身产生效果");
                break;
            case E_CardRangeType.cross:
                CreatCrossRange(card.currentCrossRangeUp,card.currentCrossRangeDown, card.currentRecRangeHigh,card.currentRecRangeWide);
                break;
        }
    }

    private void CreatRectangleRange(int wide,int high)
    {
        Debug.Log($"生成矩形范围{wide}*{high}");
    }

    private void CreatCrossRange(int up,int down,int high,int wide)
    {
        Debug.Log("生成十字范围" + up + down + high + wide);
    }
}