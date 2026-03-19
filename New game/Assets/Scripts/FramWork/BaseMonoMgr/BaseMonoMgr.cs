using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 继承了Mono的单例模式
/// 使用这个单例要注意：不会再加载场景时移除
/// </summary>
/// <typeparam name="T"></typeparam>
public class BaseMonoMgr<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    // 标记是否是手动创建的空对象（避免覆盖场景对象）
    private static bool isManualCreate = false;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 核心修复：先查找场景中已有的组件（优先用Inspector配置的对象）
                instance = FindObjectOfType<T>();

                // 场景中没有，才手动创建（兜底）
                if (instance == null)
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    instance = obj.AddComponent<T>();
                    DontDestroyOnLoad(obj);
                    isManualCreate = true;
                    Debug.LogWarning($"场景中未找到 {typeof(T).Name}，已手动创建空对象（无Inspector配置）");
                }
                else
                {
                    // 场景中有对象，确保不销毁
                    DontDestroyOnLoad(instance.gameObject);
                    Debug.Log($"使用场景中已有的 {typeof(T).Name}（读取Inspector配置）");
                }
            }
            return instance;
        }
    }

    protected virtual void Awake()
    {
        // 手动创建的对象，跳过（避免覆盖）
        if (isManualCreate) return;

        // 场景中有多个实例，销毁多余的
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            Debug.LogWarning($"场景中有多个 {typeof(T).Name}，已销毁多余实例");
            return;
        }

        // 场景中第一个实例，赋值并标记
        instance = this as T;
        DontDestroyOnLoad(gameObject);

        // 子类初始化入口（替代Awake）
        OnInit();
    }

    // 供子类重写的初始化方法（读取Inspector参数）
    protected virtual void OnInit() { }
}
