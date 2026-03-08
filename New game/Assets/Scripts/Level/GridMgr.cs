using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMgr : BaseMonoMgr<GridMgr>
{
    /// <summary>
    /// 生成网格的初始位置，从左到右从下到上依次生成
    /// </summary>
    [SerializeField]
    private Vector2 rawPos;


    /// <summary>
    /// 创建格子地图
    /// </summary>
    /// <param name="length">行</param>
    /// <param name="wide">列</param>
    public void CreatGridMap(int length,int wide)
    {
        for(int i = 0; i < length; i++)
        {
            for(int j = 0; j < wide; j++)
            {

            }
        }
    }


}
