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

    public static T Instance
    {
        get
        {
            if(instance == null)
            {
                GameObject obj = new GameObject();
                
                obj.name = typeof(T).Name;

                instance = obj.AddComponent<T>();

                DontDestroyOnLoad(obj);
            }
            return instance;
        }
    }
}
