using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                GameObject obj = GameObject.Instantiate(Resources.Load<GameObject>(PlotRes), gridsRoot.transform,false);
                obj.name = "plot_" + j + "_" + i;
                Vector2 newPos = new Vector2(gridWide * i, gridHigh * j);
                obj.transform.localPosition = newPos;
            }
        }
    }


}
