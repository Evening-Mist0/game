using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlotType
{
    occupied,
    none,
}
public class Plot : MonoBehaviour
{
    //世界坐标
    [HideInInspector]
    public Vector2 myWorldPos;
    //网格逻辑坐标
    [HideInInspector]
    public GridPos logicalPos;

    private void Start()
    {
    
    }

}
