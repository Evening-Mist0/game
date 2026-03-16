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
        levelState = GetComponent<BaseLevelState>();
        if (levelState == null)
            Debug.LogError("请为该对象挂载BaseLevelState");

    }

    public T TurnToSubStater<T>() where T : BaseLevelState
    {
        return levelState as T;
    }

    /// <summary>
    /// 注册全局模块需要监听的事件
    /// </summary>
    protected abstract void RegisterOperateEvents();

    /// <summary>
    /// 泛型获取当前状态
    /// </summary>
    /// <typeparam name="T">必须继承 BaseLevelState</typeparam>
    /// <param name="state">输出当前状态</param>
    /// <returns>是否是目标状态</returns>
    protected bool TryGetCurrentState<T>(out T state) where T : BaseLevelState
    {
        state = null;

        // 全局单例判空
        if (LevelStepMgr.Instance == null || LevelStepMgr.Instance.machine == null)
            return false;

        // 获取当前状态
        BaseLevelState nowState = LevelStepMgr.Instance.machine.nowState;

        // 判断是否是目标类型
        if (nowState is T targetState)
        {
            state = targetState;
            return true;
        }

        return false;
    }
}