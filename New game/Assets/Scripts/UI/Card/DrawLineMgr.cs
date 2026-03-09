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

    public bool isDrawing = false;


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

        EventCenter.Instance.AddEventListener<Vector3>(E_EventType.OnCardClick0, EnterDrawing);
        EventCenter.Instance.AddEventListener(E_EventType.OnCardClick1, ExitDrawing);
        lr.positionCount = 0;
    }

    private void Update()
    {
        if (isDrawing)
        {
            //Debug.Log("正在绘画");
            if (startPos == Vector3.zero)
                Debug.LogWarning("起始位置可能获得错误，需要检查");
            endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endPos.z = 0;
            DrawLine(startPos);
        }
    }

    public void EnterDrawing(Vector3 startPos)
    {
        lr.positionCount = 2;
        isDrawing = true;
        this.startPos = startPos;
        lr.SetPosition(0, startPos);
    }

    public void ExitDrawing()
    {
        isDrawing = false;
        startPos = Vector3.zero;
        lr.positionCount = 0;
    }

    /// <summary>
    /// 切换绘线状态，true会变为false，false会变为true
    /// </summary>
    /// <param name="startPos">绘画先开始的位置，应当是该卡牌UI的世界坐标</param>
    public void ChangeDrawState(Vector3 startPos)
    {
        //进入绘线状态
        if(!isDrawing)
        {
            lr.positionCount = 2;
            isDrawing = true;
            this.startPos = startPos;
            lr.SetPosition(0, startPos);
        }
        else//退出绘线状态
        {
            isDrawing = false;
            startPos = Vector3.zero;
            lr.positionCount = 0;
        }    
    }


    private void DrawLine(Vector3 startPos)
    {
        lr.SetPosition(1, endPos);
    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<Vector3>(E_EventType.OnCardClick0, EnterDrawing);
        EventCenter.Instance.RemoveEventListener(E_EventType.OnCardClick1, ExitDrawing);
    }
}
