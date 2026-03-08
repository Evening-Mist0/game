using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 单例，用于非继承Mono的类却要使用到继承Mono类中的方法
/// </summary>
public class MonoMgr : BaseMonoMgr<MonoMgr>
{
    private event UnityAction upadateEvent;

    private event UnityAction fixedUpadateEvent;

    private event UnityAction lateUpadateEvent;

    
    public void AddInUpdate(UnityAction updataEvent)
    {
        upadateEvent += updataEvent;
    }
    public void AddInFixedUpadate(UnityAction updataEvent)
    {
        upadateEvent += updataEvent;
    }
    public void AddInLateUpadate(UnityAction updataEvent)
    {
        upadateEvent += updataEvent;
    }

    public void RemoveInUpdate(UnityAction updataEvent)
    {
        upadateEvent -= updataEvent;
    }
    public void RemoveInFixedUpadate(UnityAction updataEvent)
    {
        upadateEvent -= updataEvent;
    }
    public void RemoveInLateUpadate(UnityAction updataEvent)
    {
        upadateEvent -= updataEvent;
    }

    void Update()
    {
        upadateEvent?.Invoke();
    }

    void FixedUpdate()
    {
        fixedUpadateEvent?.Invoke();
    }

    private void LateUpdate()
    {
        lateUpadateEvent?.Invoke();
    }
}
