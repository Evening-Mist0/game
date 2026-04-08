using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//注意：继承该类的子类，在Resouces文件夹的使用规则为： 在BaseCardScriptableObject文件夹下，名字

/// <summary>
/// 卡牌SO单例模式，用于具体的卡牌SO调用
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class SingleCardScriptableObject<T> : ScriptableObject where T : ScriptableObject
{
    private static T instance;

    public static T Instance
    {
        get
        {        
            if (instance == null)
                instance = JsonMgr.Instance.LoadScriptableObjectData<T>("CardInfo_" + typeof(T).Name);        
            return instance;
        }
    }
}
