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

//using UnityEngine;
//using UnityEngine.UI;

//public class DrawLineMgr : MonoBehaviour
//{
//    private static DrawLineMgr instance;
//    public static DrawLineMgr Instance => instance;

//    [Header("相机设置")]
//    [Tooltip("拖拽你的UICamera到这里（如果不拖拽，会自动查找）")]
//    public Camera targetUICamera;  // 可以在Inspector中手动指定

//    [Header("UI画线设置")]
//    private UILineDrawer uiLineDrawer;
//    private Canvas lineCanvas;
//    private RectTransform canvasRect;
//    private Camera uiCamera;

//    [HideInInspector]
//    public Vector3 startPos = new Vector3(0, 0, 0);
//    [HideInInspector]
//    public Vector3 endPos;

//    public bool isDrawing = false;

//    private DrawLineBinder binder;

//    protected void Awake()
//    {
//        instance = this;
//        DontDestroyOnLoad(this);

//        // 优先使用Inspector中指定的相机
//        if (targetUICamera != null)
//        {
//            uiCamera = targetUICamera;
//            Debug.Log($"[DrawLineMgr] 使用Inspector指定的UICamera: {uiCamera.name}");
//        }
//        else
//        {
//            // 否则自动查找UICamera
//            FindUICamera();
//        }

//        // 创建UI画线系统
//        CreateUILineSystem();

//        binder = this.gameObject.GetComponent<DrawLineBinder>();
//        if (binder == null)
//            binder = this.gameObject.AddComponent<DrawLineBinder>();
//    }

//    /// <summary>
//    /// 查找UICamera
//    /// </summary>
//    private void FindUICamera()
//    {
//        uiCamera = UIMgr.Instance.UICamera;     
//        if(uiCamera == null)
//        Debug.LogError("没有找到UI相机重开吧");
//    }

//    /// <summary>
//    /// 创建UI画线系统（替代LineRenderer）
//    /// </summary>
//    private void CreateUILineSystem()
//    {
//        if (uiCamera == null)
//        {
//            Debug.LogError("[DrawLineMgr] UICamera为空，无法创建线条系统！");
//            return;
//        }

//        // 1. 创建专门的画线Canvas
//        GameObject canvasObj = new GameObject("LineCanvas");
//        canvasObj.transform.SetParent(transform);

//        lineCanvas = canvasObj.AddComponent<Canvas>();
//        lineCanvas.renderMode = RenderMode.ScreenSpaceCamera;
//        lineCanvas.worldCamera = uiCamera;
//        lineCanvas.planeDistance = 0.5f; // 设置在UI前方
//        lineCanvas.sortingOrder = 999; // 最高层级，确保在所有UI之上

//        // 2. 添加CanvasScaler（适配屏幕）- 尝试复制主UI的设置
//        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();

//        // 查找主UI Canvas，复制其设置
//        Canvas mainCanvas = FindObjectOfType<Canvas>();
//        if (mainCanvas != null && mainCanvas != lineCanvas)
//        {
//            CanvasScaler mainScaler = mainCanvas.GetComponent<CanvasScaler>();
//            if (mainScaler != null)
//            {
//                scaler.uiScaleMode = mainScaler.uiScaleMode;
//                scaler.referenceResolution = mainScaler.referenceResolution;
//                scaler.screenMatchMode = mainScaler.screenMatchMode;
//                scaler.matchWidthOrHeight = mainScaler.matchWidthOrHeight;
//                scaler.referencePixelsPerUnit = mainScaler.referencePixelsPerUnit;
//                Debug.Log($"[DrawLineMgr] 复制主UI的CanvasScaler设置");
//            }
//        }

//        // 如果没有主UI，使用默认设置
//        if (scaler.uiScaleMode == CanvasScaler.ScaleMode.ConstantPixelSize)
//        {
//            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
//            scaler.referenceResolution = new Vector2(1920, 1080);
//        }

//        // 3. 关闭射线检测，让点击穿透
//        GraphicRaycaster raycaster = canvasObj.AddComponent<GraphicRaycaster>();
//        raycaster.enabled = false;

//        canvasRect = canvasObj.GetComponent<RectTransform>();

//        // 4. 创建线条绘制对象
//        GameObject lineObj = new GameObject("UILineDrawer");
//        lineObj.transform.SetParent(canvasObj.transform);

//        // 设置铺满全屏 - 关键修改：重置所有变换
//        RectTransform lineRect = lineObj.AddComponent<RectTransform>();

//        // 【重要】重置缩放为 1
//        lineRect.localScale = Vector3.one;

//        // 【重要】重置位置和旋转
//        lineRect.localPosition = Vector3.zero;
//        lineRect.localRotation = Quaternion.identity;

//        // 设置锚点铺满全屏
//        lineRect.anchorMin = Vector2.zero;
//        lineRect.anchorMax = Vector2.one;
//        lineRect.offsetMin = Vector2.zero;
//        lineRect.offsetMax = Vector2.zero;

//        // 确保 pivot 在中心
//        lineRect.pivot = new Vector2(0.5f, 0.5f);

//        // 5. 添加UILineDrawer组件
//        uiLineDrawer = lineObj.AddComponent<UILineDrawer>();
//        uiLineDrawer.color = Color.red;
//        uiLineDrawer.lineWidth = 10f;
//        uiLineDrawer.raycastTarget = false;

//        // 【重要】设置纹理（如果没有纹理，创建一个纯色纹理）
//        if (uiLineDrawer.lineTexture == null)
//        {
//            Texture2D tex = new Texture2D(1, 1);
//            tex.SetPixel(0, 0, Color.white);
//            tex.Apply();
//            uiLineDrawer.lineTexture = tex;
//        }

//        // 强制刷新材质
//        uiLineDrawer.SetAllDirty();

//        Debug.Log($"[DrawLineMgr] 线条Canvas创建成功，使用相机: {uiCamera.name}, 排序层级: {lineCanvas.sortingOrder}");
//    }

//    private void Update()
//    {
//        if (isDrawing)
//        {
//            if (startPos == Vector3.zero)
//                Debug.LogWarning("[DrawLineMgr] 起始位置可能获得错误，需要检查");

//            // 获取鼠标位置并转换为UI局部坐标
//            Vector2 mouseUIPos = ScreenToUILocalPoint(Input.mousePosition);
//            endPos = mouseUIPos;

//            DrawLine(startPos);
//        }
//    }

//    /// <summary>
//    /// 世界坐标转UI局部坐标 - 修正版
//    /// </summary>
//    private Vector2 WorldToUILocalPoint(Vector3 worldPos)
//    {
//        if (uiCamera == null || canvasRect == null)
//        {
//            Debug.LogError("[DrawLineMgr] uiCamera或canvasRect为空");
//            return Vector2.zero;
//        }

//        // 【关键修改】应该用世界相机（主相机）把3D坐标转屏幕坐标
//        Camera worldCamera = Camera.main;  // 你的3D游戏相机
//        if (worldCamera == null)
//        {
//            Debug.LogError("[DrawLineMgr] 找不到主相机");
//            return Vector2.zero;
//        }

//        // 1. 先用世界相机把3D坐标转屏幕坐标
//        Vector2 screenPos = worldCamera.WorldToScreenPoint(worldPos);

//        // 2. 再用UI相机把屏幕坐标转UI局部坐标
//        Vector2 uiLocalPos;
//        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
//            canvasRect,
//            screenPos,
//            uiCamera,  // 这里要用UI相机
//            out uiLocalPos
//        );

//        if (!success)
//        {
//            Debug.LogWarning($"[DrawLineMgr] 坐标转换失败: {worldPos}");
//            return Vector2.zero;
//        }

//        // 添加调试信息
//        Debug.Log($"[DrawLineMgr] 世界坐标:{worldPos} -> 屏幕坐标:{screenPos} -> UI局部坐标:{uiLocalPos}");

//        return uiLocalPos;
//    }

//    /// <summary>
//    /// 屏幕坐标转UI局部坐标 - 修正版
//    /// </summary>
//    private Vector2 ScreenToUILocalPoint(Vector2 screenPos)
//    {
//        if (uiCamera == null || canvasRect == null)
//        {
//            Debug.LogError("[DrawLineMgr] uiCamera或canvasRect为空");
//            return Vector2.zero;
//        }

//        Vector2 uiLocalPos;
//        bool success = RectTransformUtility.ScreenPointToLocalPointInRectangle(
//            canvasRect,
//            screenPos,
//            uiCamera,  // 屏幕坐标转UI局部坐标需要用UI相机
//            out uiLocalPos
//        );

//        if (!success)
//        {
//            Debug.LogWarning($"[DrawLineMgr] 屏幕坐标转换失败: {screenPos}");
//            return Vector2.zero;
//        }

//        return uiLocalPos;
//    }

//    public void EnterDrawing(Vector3 startPos)
//    {
//        isDrawing = true;

//        // 将世界坐标转换为UI局部坐标
//        Vector2 uiStartPos = WorldToUILocalPoint(startPos);
//        this.startPos = uiStartPos;

//        // 设置UI线条的起点和终点（终点暂时和起点相同）
//        if (uiLineDrawer != null)
//        {
//            uiLineDrawer.SetStartPoint(uiStartPos);
//            uiLineDrawer.SetEndPoint(uiStartPos);
//            Debug.Log($"[DrawLineMgr] 开始画线，起点: {uiStartPos}");
//        }
//        else
//        {
//            Debug.LogError("[DrawLineMgr] uiLineDrawer为空！");
//        }
//    }

//    public void ExitDrawing()
//    {
//        isDrawing = false;
//        startPos = Vector3.zero;
//        if (uiLineDrawer != null)
//        {
//            uiLineDrawer.ClearPoints();
//        }
//        Debug.Log("[DrawLineMgr] 退出画线");
//    }

//    /// <summary>
//    /// 切换绘线状态，true会变为false，false会变为true
//    /// </summary>
//    /// <param name="startPos">绘画先开始的位置，应当是该卡牌UI的世界坐标</param>
//    public void ChangeDrawState(Vector3 startPos)
//    {
//        //进入绘线状态
//        if (!isDrawing)
//        {
//            EnterDrawing(startPos);
//        }
//        else//退出绘线状态
//        {
//            ExitDrawing();
//        }
//    }

//    private void DrawLine(Vector3 startPos)
//    {
//        // startPos已经是UI局部坐标
//        // endPos是鼠标的UI局部坐标
//        if (uiLineDrawer != null)
//        {
//            uiLineDrawer.SetEndPoint(endPos);
//        }
//    }

//    /// <summary>
//    /// 设置线条颜色
//    /// </summary>
//    public void SetLineColor(Color color)
//    {
//        if (uiLineDrawer != null)
//            uiLineDrawer.color = color;
//    }

//    /// <summary>
//    /// 设置线条宽度
//    /// </summary>
//    public void SetLineWidth(float width)
//    {
//        if (uiLineDrawer != null)
//            uiLineDrawer.lineWidth = width;
//    }

//    /// <summary>
//    /// 设置线条Canvas的排序层级
//    /// </summary>
//    public void SetLineCanvasOrder(int order)
//    {
//        if (lineCanvas != null)
//            lineCanvas.sortingOrder = order;
//    }

//    /// <summary>
//    /// 获取当前使用的UICamera
//    /// </summary>
//    public Camera GetUICamera()
//    {
//        return uiCamera;
//    }

//    private void OnDestroy()
//    {
//        if (instance == this)
//            instance = null;
//    }
//}