using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 爬塔路线UI组件
/// 负责路线渲染、状态切换、虚线样式
/// </summary>
public class TowerRouteLine : MonoBehaviour
{
    [Header("路线组件")]
    public Image routeLineImg;
    public RectTransform lineRect;

    [Header("路线状态配色")]
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 0.6f); // 锁定灰色虚线
    public Color unlockedColor = new Color(0.8f, 0.8f, 0.8f, 0.8f); // 解锁浅白色
    public Color completedColor = new Color(1f, 0.9f, 0.2f, 1f); // 已通过亮黄色

    private string _currentRouteId;

    /// <summary>
    /// 初始化路线
    /// </summary>
    public void InitRoute(TowerRouteData routeData, Vector2 startPos, Vector2 endPos)
    {
        _currentRouteId = routeData.routeId;
        // 设置路线位置与长度
        SetLinePositionAndSize(startPos, endPos);
        // 刷新路线状态
        RefreshRouteState(routeData);
    }

    /// <summary>
    /// 设置路线的位置、长度、旋转
    /// </summary>
    private void SetLinePositionAndSize(Vector2 startPos, Vector2 endPos)
    {
        // 计算两点距离
        float distance = Vector2.Distance(startPos, endPos);
        // 设置路线长度
        lineRect.sizeDelta = new Vector2(distance, lineRect.sizeDelta.y);
        // 设置路线中点位置
        lineRect.anchoredPosition = (startPos + endPos) / 2f;
        // 计算旋转角度
        Vector2 dir = endPos - startPos;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        lineRect.localEulerAngles = new Vector3(0, 0, angle);
    }

    /// <summary>
    /// 刷新路线状态
    /// </summary>
    public void RefreshRouteState(TowerRouteData routeData)
    {
        routeLineImg.color = routeData.routeState switch
        {
            E_NodeState.Locked => lockedColor,
            E_NodeState.Unlocked => unlockedColor,
            E_NodeState.Completed => completedColor,
            _ => lockedColor
        };

        // 虚线材质适配：锁定状态用虚线，解锁/已完成可调整样式
        // 注意：虚线效果可通过Image的材质实现，美术提供虚线材质球赋值给routeLineImg即可
    }
}