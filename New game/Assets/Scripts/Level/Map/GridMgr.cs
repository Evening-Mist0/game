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

    public static GridPos operator +(GridPos a, GridPos b)
    {
        return new GridPos(
            a.x + b.x,
            a.y + b.y
        );
    }
}

/// <summary>
/// 逻辑层面的网格管理：生成网格，通过GridPos获取网格
/// </summary>
public class GridMgr : BaseMonoMgr<GridMgr>
{

    [Header("格子地图基础配置")]
    [Tooltip("生成格子的原点")]
    private Vector3 origin = new Vector3(-689, -100, 0);

    private GameObject gridsRoot;
    [Tooltip("格子宽间距")]
    public float gridWide;
    [Tooltip("格子高间距")]
    public float gridHigh;
    [Tooltip("格子横向数量")]
    public int gridWideCount;
    [Tooltip("格子纵向数量")]
    public int gridHighCount;

    //格子加载路径
    private string cellRes = "Level/Cell";
    public string CellRes
    {
        get { return cellRes; }
    }



    public Dictionary<GridPos, Cell> cellDic = new Dictionary<GridPos, Cell>();



    /// <summary>
    /// 创建格子地图
    /// </summary>
    /// <param name="length">行</param>
    /// <param name="wide">列</param>
    //public void CreatGridMap()
    //{
    //    if (gridsRoot == null)
    //    {
    //        gridsRoot = new GameObject();
    //        gridsRoot.name = "GridsRoot";
    //        UIMgr.Instance.ShowPanel<MapGridPanel>();
    //        gridsRoot.transform.SetParent(UIMgr.Instance.GetPanel<MapGridPanel>().transform);
    //        Debug.Log("origin坐标" + origin);
    //        gridsRoot.transform.localPosition = origin;
    //    }


    //    for (int i = 0; i < gridWideCount; i++)
    //    {
    //        for (int j = 0; j < gridHighCount; j++)
    //        {
    //            //实例化格子对象
    //            GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(CellRes), gridsRoot.transform, false);
    //            obj.name = "plot_" + j + "_" + i;
    //            Cell plot = obj.GetComponent<Cell>();
    //            if (plot == null)
    //            {
    //                Debug.LogError("物体没有挂载Plot脚本，请挂载");
    //                return;
    //            }

    //            //初始化每个格子的位置,获取格子世界坐标
    //            Vector2 newPos = new Vector2(gridWide * i, gridHigh * j);
    //            obj.transform.localPosition = newPos;
    //            plot.myWorldPos = newPos;

    //            //获取格子的逻辑坐标
    //            GridPos gridPos = new GridPos(i, j);
    //            plot.logicalPos = gridPos;

    //            //添加到字典用于逻辑层面的管理
    //            cellDic.Add(gridPos, obj.GetComponent<Cell>());
    //        }
    //    }
    //}

    public void CreatGridMap()
    {
        if (gridsRoot == null)
        {
            gridsRoot = new GameObject();
            gridsRoot.name = "GridsRoot";
            UIMgr.Instance.ShowPanel<MapGridPanel>();
            gridsRoot.transform.SetParent(UIMgr.Instance.GetPanel<MapGridPanel>().transform);

            // 设置父对象的本地坐标（原点）
            gridsRoot.transform.localPosition = origin;
            gridsRoot.transform.SetAsLastSibling();
        }

        // 缓存父物体Transform，避免循环内重复获取
        Transform rootTrans = gridsRoot.transform;

        for (int i = 0; i < gridWideCount; i++)
        {
            for (int j = 0; j < gridHighCount; j++)
            {
                // 实例化格子
                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(CellRes), rootTrans, false);
                obj.name = "plot_" + j + "_" + i;
                Cell plot = obj.GetComponent<Cell>();

                if (plot == null)
                {
                    Debug.LogError("格子预制体没有挂载Cell脚本！");
                    return;
                }

                // 计算本地坐标（相对于父物体）
                Vector2 localPos = new Vector2(gridWide * i, gridHigh * j);
                obj.transform.localPosition = localPos;

                // 把本地坐标 → 转换成 世界坐标 赋值给 myWorldPos
                plot.myWorldPos = rootTrans.TransformPoint(localPos);

                // 逻辑格子坐标
                GridPos gridPos = new GridPos(i, j);
                plot.logicalPos = gridPos;

                // 存入字典
                cellDic.Add(gridPos, plot);
            }
        }
    }

    /// <summary>
    /// 生成卡牌检测范围
    /// </summary>
    /// <param name="cell">玩家点击的单元格</param>
    /// <param name="card">使用的卡牌</param>
    /// <returns>辐射范围表(包括自身)</returns>
    public List<Cell> CreatCheckRange(Cell cell, BaseCard card)
    {
        Debug.Log($"进行范围生成,当前的单元格位置为{cell.logicalPos.x},{cell.logicalPos.y},卡牌的名字为{card.name}");
        switch (card.CardRangeType)
        {
            case E_CardRangeType.Rectangle:
                Debug.Log($"进行矩形范围生成wide{card.currentRecRangeWide}high{card.currentRecRangeHigh}");
                return CreatRectangleRange(cell, card.currentRecRangeWide, card.currentRecRangeHigh );
            case E_CardRangeType.MySelf:
                return CreatMySelfRange(cell);
            case E_CardRangeType.Cross:
                Debug.Log($"进行十字范围生成{card.currentCrossRangeUp}{card.currentCrossRangeDown}{card.baseCrossRangeLeft}{card.currentCrossRangeRight}");
                return CreatCrossRange(cell, card.currentCrossRangeUp, card.currentCrossRangeDown, card.baseCrossRangeLeft, card.currentCrossRangeRight);
            default:
                Debug.LogError("获取范围失败,list列表竟然为空");
                return null;
                
        }
    }

    private List<Cell> CreatRectangleRange(Cell cell, int wide, int high)
    {
        Debug.Log($"【最终最终版】wide={wide} high={high} 基准点={cell.logicalPos.x},{cell.logicalPos.y}");
        List<Cell> list = new List<Cell>();
        GridPos center = cell.logicalPos;

        // ====================== 核心正确计算 ======================
        // X：奇数居中，偶数靠左
        int offsetX = (wide - 1) / 2;

        // Y：奇数居中，偶数靠下（你的规则）
        int offsetY = (high - 1) / 2;

        // 起始坐标
        int startX = center.x - offsetX;
        int startY = center.y - offsetY;

        // ====================== 遍历 ======================
        for (int y = 0; y < high; y++)
        {
            for (int x = 0; x < wide; x++)
            {
                int tx = startX + x;
                int ty = startY + y;

                GridPos pos = new GridPos(tx, ty);
                if (cellDic.ContainsKey(pos))
                    list.Add(cellDic[pos]);
            }
        }

        return list;
    }


    /// 生成十字范围
    /// </summary>
    /// <param name="cell">中心单元格</param>
    /// <param name="up">向上辐射格数</param>
    /// <param name="down">向下辐射格数</param>
    /// <param name="left">向左辐射格数</param>
    /// <param name="right">向右辐射格数</param>
    private List<Cell> CreatCrossRange(Cell cell, int up, int down, int left, int right)
    {
        Debug.Log($"进行十字范围生成{up}{down}{left}{right}");

        List<Cell> list = new List<Cell>();
        GridPos center = cell.logicalPos;

        //添加中心格子
        if (cellDic.ContainsKey(center))
            list.Add(cellDic[center]);

        //向上生成 up 格
        for (int i = 1; i <= up; i++)
        {
            GridPos pos = new GridPos(center.x, center.y + i);
            if (cellDic.ContainsKey(pos))
                list.Add(cellDic[pos]);
        }

        //向下生成 down 格
        for (int i = 1; i <= down; i++)
        {
            GridPos pos = new GridPos(center.x, center.y - i);
            if (cellDic.ContainsKey(pos))
                list.Add(cellDic[pos]);
        }

        //向左生成left格
        for (int i = 1; i <= left; i++)
        {
            GridPos pos = new GridPos(center.x - i, center.y);
            if (cellDic.ContainsKey(pos))
                list.Add(cellDic[pos]);
        }

        //向右生成right格
        for (int i = 1; i <= right; i++)
        {
            GridPos pos = new GridPos(center.x + i, center.y);
            if (cellDic.ContainsKey(pos))
                list.Add(cellDic[pos]);
            else
                Debug.Log($"没有检测到{pos.x}{pos.y}");

        }
        return list;
    }

    private List<Cell> CreatMySelfRange(Cell cell)
    {
        Debug.Log("进行自身范围生成");

        List<Cell> list = new List<Cell>();
        list.Add(cell);
        return list;
    }
}