using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI坐标转换静态工具类（适配UICamera固定分辨率1920*1080场景）
/// 核心功能：Image的UI坐标 → 主相机屏幕坐标 → 主相机世界坐标
/// </summary>
public static class UIWithUICameraConverter
{
    // UICamera的设计分辨率（固定1920*1080）
    private const float UI_DESIGN_WIDTH = 1920f;
    private const float UI_DESIGN_HEIGHT = 1080f;

    /// <summary>
    /// 将UICamera下的Image坐标转换为主相机的屏幕坐标（像素级精准映射）
    /// </summary>
    /// <param name="targetImage">目标Image组件</param>
    /// <param name="uiCamera">渲染UI的专用相机（必须是正交相机，且分辨率为1920*1080）</param>
    /// <returns>主游戏屏幕的像素坐标（左下为原点，和Input.mousePosition同坐标系）</returns>
    public static Vector2 ConvertImageToMainScreenPos(Image targetImage, Camera uiCamera)
    {
        // 1. 空值校验
        if (targetImage == null)
        {
            Debug.LogError("[UIWithUICameraConverter] 目标Image组件为空！");
            return Vector2.zero;
        }
        if (uiCamera == null)
        {
            Debug.LogError("[UIWithUICameraConverter] UICamera未赋值！");
            return Vector2.zero;
        }
        if (!uiCamera.orthographic)
        {
            Debug.LogWarning("[UIWithUICameraConverter] UICamera建议设置为正交相机（Orthographic）！");
        }

        RectTransform rectTrans = targetImage.rectTransform;
        Canvas canvas = targetImage.canvas;

        // 2. 获取Image在UICamera下的世界坐标（基于anchoredPosition）
        Vector3 uiWorldPos = rectTrans.TransformPoint(rectTrans.anchoredPosition);

        // 3. 将UICamera世界坐标转为UICamera视角的屏幕坐标（1920*1080）
        Vector2 uiScreenPos = uiCamera.WorldToScreenPoint(uiWorldPos);

        // 4. 计算分辨率缩放比例，将UICamera的1920*1080坐标映射到主屏幕实际分辨率
        float scaleX = Screen.width / UI_DESIGN_WIDTH;
        float scaleY = Screen.height / UI_DESIGN_HEIGHT;

        // 5. 得到主相机的屏幕坐标（精准映射）
        Vector2 mainScreenPos = new Vector2(
            uiScreenPos.x * scaleX,
            uiScreenPos.y * scaleY
        );

        return mainScreenPos;
    }

    /// <summary>
    /// （一键转换）将Image的UI坐标直接转换为主相机的世界坐标（给LineRenderer等3D组件使用）
    /// </summary>
    /// <param name="targetImage">目标Image组件</param>
    /// <param name="uiCamera">渲染UI的专用相机</param>
    /// <param name="mainCamera">主游戏相机</param>
    /// <param name="zDistance">主相机到目标世界平面的Z轴距离（根据场景调整，避免和相机重叠）</param>
    /// <returns>主相机视角下的世界坐标</returns>
    public static Vector3 ConvertImageToMainWorldPos(Image targetImage, Camera uiCamera, Camera mainCamera, float zDistance =0f)
    {
        // 1. 先转主相机屏幕坐标
        Vector2 mainScreenPos = ConvertImageToMainScreenPos(targetImage, uiCamera);

        // 2. 空值校验
        if (mainCamera == null)
        {
            Debug.LogError("[UIWithUICameraConverter] 主相机未赋值！");
            return Vector3.zero;
        }

        // 3. 屏幕坐标转世界坐标（指定Z轴距离）
        Vector3 mainWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(
            mainScreenPos.x,
            mainScreenPos.y,
            zDistance
        ));

        return mainWorldPos;
    }
}