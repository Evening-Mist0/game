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
    //网格坐标
    [HideInInspector]
    public int[] myWebPos = new int [2];

    private void Start()
    {
    
    }

}
