using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
//using UnityEngine.Animations.Rigging;


public class DrawLineMgr : MonoBehaviour
{
    private static DrawLineMgr instance;
    public static DrawLineMgr Instance => instance;

    [Header("相关脚本")]
    private LineRenderer lr;

    [HideInInspector]
    public Vector3 startPos = new Vector3(0,0,0);
    [HideInInspector]
    public Vector3 endPos;
    
    public bool isDrawing = true;


    protected  void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        lr = this.GetComponent<LineRenderer>();
        if (lr == null)
        {
            Debug.LogWarning("请为该单例挂载LineRenderer脚本，已自动挂载默认脚本");
            lr = gameObject.AddComponent<LineRenderer>();

        }

        lr.positionCount = 2;
    }

    private void Update()
    {
        if (isDrawing)
        {
            DrawLine(startPos, Camera.main.ScreenToWorldPoint(Input.mousePosition));
        }
    }


    public void DrawLine(Vector3 startPos, Vector3 endPos)
    {
        lr.SetPosition(0, startPos);
        lr.SetPosition(1, endPos);
    }
}
