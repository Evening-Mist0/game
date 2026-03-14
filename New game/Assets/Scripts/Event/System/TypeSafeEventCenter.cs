// 路径：Scripts/Event/System/TypeSafeEventCenter.cs
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// 类型安全的事件中心，支持自动注销、多实例兼容
/// </summary>
public class TypeSafeEventCenter : MonoBehaviour
{
    // 单例（全局唯一，事件中心本身可以单例）
    private static TypeSafeEventCenter _instance;
    public static TypeSafeEventCenter Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<TypeSafeEventCenter>();
                if (_instance == null)
                {
                    var go = new GameObject("[TypeSafeEventCenter]");
                    _instance = go.AddComponent<TypeSafeEventCenter>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }

    // 事件注册表：Key=事件类型，Value=事件回调列表
    private Dictionary<Type, Delegate> _eventTable = new Dictionary<Type, Delegate>();
    // 生命周期绑定：Key=目标对象（如BaseCard实例），Value=需要注销的事件列表
    private Dictionary<Object, List<(Type, Delegate)>> _lifetimeBindings = new Dictionary<Object, List<(Type, Delegate)>>();

    #region 对外API：注册/注销/触发
    /// <summary>
    /// 注册事件（自动绑定生命周期，对象销毁时自动注销）
    /// </summary>
    /// <typeparam name="TEvent">事件类型</typeparam>
    /// <param name="listener">监听者</param>
    /// <param name="onEvent">回调方法</param>
    public void Register<TEvent>(Object listener, Action<TEvent> onEvent) where TEvent : GameEventBase
    {
        if (listener == null || onEvent == null)
        {
            Debug.LogError("注册事件失败：监听者或回调为空！");
            return;
        }

        Type eventType = typeof(TEvent);
        // 注册回调
        if (!_eventTable.ContainsKey(eventType))
        {
            _eventTable[eventType] = onEvent;
        }
        else
        {
            _eventTable[eventType] = Delegate.Combine(_eventTable[eventType], onEvent);
        }

        // 绑定生命周期（关键：按listener实例绑定，多卡牌不冲突）
        if (!_lifetimeBindings.ContainsKey(listener))
        {
            _lifetimeBindings[listener] = new List<(Type, Delegate)>();
            // 给监听者挂载生命周期绑定器
            if (listener is MonoBehaviour mb)
            {
                var binder = mb.GetComponent<EventLifetimeBinder>();
                if (binder == null)
                {
                    binder = mb.gameObject.AddComponent<EventLifetimeBinder>();
                }
                binder.Init(listener, this);
            }
        }
        _lifetimeBindings[listener].Add((eventType, onEvent));
    }

    /// <summary>
    /// 触发事件（类型安全）
    /// </summary>
    public void Trigger<TEvent>(TEvent eventData) where TEvent : GameEventBase
    {
        if (eventData == null)
        {
            Debug.LogError("触发事件失败：事件数据为空！");
            return;
        }

        Type eventType = typeof(TEvent);
        if (_eventTable.TryGetValue(eventType, out var callback))
        {
            try
            {
                (callback as Action<TEvent>)?.Invoke(eventData);
            }
            catch (Exception e)
            {
                Debug.LogError($"触发事件{eventType.Name}失败：{e.Message}");
            }
        }
    }

    /// <summary>
    /// 内部调用：注销指定监听者的所有事件
    /// </summary>
    internal void UnregisterAll(Object listener)
    {
        if (listener == null || !_lifetimeBindings.ContainsKey(listener)) return;

        foreach (var (eventType, callback) in _lifetimeBindings[listener])
        {
            _eventTable[eventType] = Delegate.Remove(_eventTable[eventType], callback);
            if (_eventTable[eventType] == null)
            {
                _eventTable.Remove(eventType);
            }
        }
        _lifetimeBindings.Remove(listener);
    }
    #endregion
}