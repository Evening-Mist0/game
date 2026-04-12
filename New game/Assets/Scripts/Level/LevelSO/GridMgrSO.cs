using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridMgrSO", menuName = "BaseMgrScriptable/GridMgrSO")]
public class GridMgrSO : SingleMgrScriptableObject<GridMgrSO>
{
    [Header("格子地图基础配置")]
    [Tooltip("生成格子的原点")]
    public Vector3 origin;
    [Tooltip("格子宽间距")]
    public float gridWide;
    [Tooltip("格子高间距")]
    public float gridHigh;
    [Tooltip("格子横向数量")]
    public int gridWideCount;
    [Tooltip("格子纵向数量")]
    public int gridHighCount;
    [Tooltip("格子加载路径")]
    public string cellRes = "Level/Cell";
    [Tooltip("格子整体倾斜程度")]
    public float gridRote;
}
