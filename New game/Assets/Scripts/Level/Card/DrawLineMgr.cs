using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class DrawLineMgr : MonoBehaviour
{
    private static DrawLineMgr instance;
    public static DrawLineMgr Instance => instance;

    [Header("相关脚本")]
    private LineRenderer lr;

    [HideInInspector]
    public Vector3 startPos = new Vector3(0, 0, 0);
    [HideInInspector]
    public Vector3 endPos;

    public bool isDrawing = false;

    private DrawLineBinder binder;

    private Camera uiCamera;

    private Canvas canvas;

    // 先在类顶部加这两个变量（确保有）
    public Material inkLineMaterial; // 拖你的水墨材质球
    private float _inkWaveTimer;


    protected void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
        lr = this.GetComponent<LineRenderer>();
        if (lr == null)
        {
            Debug.LogWarning("请为该单例挂载LineRenderer脚本，已自动挂载默认脚本");
            lr = gameObject.AddComponent<LineRenderer>();

        }
        lr.positionCount = 0;

        binder = this.gameObject.GetComponent<DrawLineBinder>();
        if (binder == null)
            binder = this.gameObject.AddComponent<DrawLineBinder>();

        uiCamera = UIMgr.Instance.UICamera;
        if (uiCamera == null)
            Debug.Log("UICamera为空");

        canvas = UIMgr.Instance.canvas;
    }

    private void Update()
    {
        if (isDrawing)//进入绘线
        {
            Ray ray = uiCamera.ScreenPointToRay(Input.mousePosition);
            Plane uiPlane = new Plane(Vector3.forward, new Vector3(0, 0, 179.3501f));
            float dist;
            if (uiPlane.Raycast(ray, out dist))
            {
                endPos = ray.GetPoint(dist);
            }

            DrawLine(endPos);

            if (lr != null && lr.material != null)
            {
                _inkWaveTimer += Time.deltaTime * 3f;

                // 超级明显的水墨波动（幅度放大到 1.0！）
                float inkValue = Mathf.Lerp(1.5f, 3.5f,
                    (Mathf.Sin(_inkWaveTimer) + 1f) / 2f);

                //传给材质
                lr.material.SetFloat("_InkSpread", inkValue);
            }
        }
    }

    public void EnterDrawing(Vector3 startPos)
    {
        lr.positionCount = 2;
        isDrawing = true;
        this.startPos = startPos;
        lr.SetPosition(0, startPos);
        // 强制绑定水墨材质，核心！
        if (inkLineMaterial != null && lr != null)
        {
            lr.material = inkLineMaterial;
            Debug.Log("材质绑定成功");
        }
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
        if (!isDrawing)
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

    }

    
}
