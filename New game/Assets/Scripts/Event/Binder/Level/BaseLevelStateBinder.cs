// 路径：Scripts/Binder/GlobalEventBinder.cs
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 游戏关卡内，全局事件绑定器：管理绘线、出牌状态等全局模块的事件
/// 挂载在GameManager上，全局唯一
/// </summary>
public abstract class BaseLevelStateBinder : MonoBehaviour 
{
    public BaseLevelState levelState;

    private void Awake()
    {
        Init();
    }

    protected virtual void Init()
    {
        //注册事件
        RegisterOperateEvents();

    }

    public T TurnToSubStater<T>() where T : BaseLevelState
    {
        return levelState as T;
    }

    /// <summary>
    /// 注册全局模块需要监听的事件
    /// </summary>
    protected abstract void RegisterOperateEvents();
}