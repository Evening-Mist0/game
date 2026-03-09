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
/// 用于逻辑层面的网格生成
/// </summary>
public class GridMgr : BaseMonoMgr<GridMgr>
{

    [Header("格子地图基础配置")]
  
    [Tooltip("生成格子的原点")]
    public Vector3 origin = new Vector3(-4.5f,2.952f,0);
    private GameObject gridsRoot;
    [Tooltip("格子宽间距")]
    [SerializeField]
    private float gridWide = 5;
    [Tooltip("格子高间距")]
    [SerializeField]
    private float gridHigh = 5;

    //格子加载路径
    private string plotRes = "Level/Plot";
    public string PlotRes 
    {
        get { return plotRes; } 
        private set { plotRes = value; } 
    }

  

    public Dictionary<GridPos, Plot> plotDic = new Dictionary<GridPos, Plot>();

    private void Awake()
    {
        CreatGridMap(6, 4);
    }


    /// <summary>
    /// 创建格子地图
    /// </summary>
    /// <param name="length">行</param>
    /// <param name="wide">列</param>
    public void CreatGridMap(int wideCount,int hightCount)
    {
        if (gridsRoot == null)
        {
            gridsRoot = new GameObject();
            gridsRoot.name = "GridsRoot";
            gridsRoot.transform.position = origin;
        }


        for(int i = 0; i < wideCount; i++)
        {
            for(int j = 0; j < hightCount; j++)
            {
                //实例化格子对象
                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(PlotRes), gridsRoot.transform,false);
                obj.name = "plot_" + j + "_" + i;
                Plot plot = obj.GetComponent<Plot>();
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
                plotDic.Add(gridPos, obj.GetComponent<Plot>());
            }
        }
    }


}
