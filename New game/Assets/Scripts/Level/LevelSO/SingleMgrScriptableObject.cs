using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 规则
///1.所有的 数据资源文件都放在 Resources文件夹下的ScriptableObject中
///2.需要复用的 唯一的数据资源文件名
///3.资源名是自己的SO类名
/// </summary>

public abstract class SingleMgrScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance;

    public static T Instance
    {
        get
        {
            //如果为空 首先应该去资源路劲下加载 对应的 数据资源文件
            if (instance == null)
            {             
                instance = Resources.Load<T>("ScriptableObject/" + typeof(T).Name);
            }
            //如果没有这个文件 为了安全起见 我们可以在这直接创建一个数据
            if (instance == null)
            {
                instance = CreateInstance<T>();
            }
            //甚至可以在这里 从json当中读取数据，但是我不建议用ScriptableObject来做数据持久化

            return instance;
        }
    }
}
