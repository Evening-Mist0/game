using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    /// <summary>
    /// 生成网格的初始位置，从左到右从下到上依次生成
    /// </summary>
    [SerializeField]
    private Vector2 rawPos;

    //世界坐标
    public Vector2 myWorldPos;
    //网格坐标
    public int[] myWebPos;

}
