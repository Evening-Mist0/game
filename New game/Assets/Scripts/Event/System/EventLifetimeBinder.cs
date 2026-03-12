// 路径：Scripts/Event/EventLifetimeBinder.cs
using UnityEngine;

/// <summary>
/// 辅助类：监听对象销毁，自动注销事件
/// </summary>
public class EventLifetimeBinder : MonoBehaviour
{
    private Object _target;
    private TypeSafeEventCenter _eventCenter;

    /// <summary>
    /// 初始化绑定关系
    /// </summary>
    public void Init(Object target, TypeSafeEventCenter eventCenter)
    {
        _target = target;
        _eventCenter = eventCenter;
    }

    private void OnDestroy()
    {
        // 对象销毁时，注销所有绑定的事件
        _eventCenter?.UnregisterAll(_target);
    }
}