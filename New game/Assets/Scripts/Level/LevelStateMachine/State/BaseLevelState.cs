using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 该类为关卡状态的父类，具体书写每个状态进入的行为
/// </summary>
public abstract class BaseLevelState : MonoBehaviour 
{
    //当前状态类的状态枚举
    public abstract E_LevelState myStateType { get; }

    //关卡状态机
    [HideInInspector]
    public LevelStateMachine machine;

    //事件绑定器 
    public BaseLevelStateBinder binder;


    private void Awake()
    {
        Init();
    }

    public virtual void Init()
    {
        machine = this.gameObject.GetComponentInParent<LevelStateMachine>();
        if (machine == null)
            Debug.LogError("请为父对象挂载LevelStateMachine脚本");
        machine.AddState<BaseLevelState>(this);

        binder = this.GetComponent<BaseLevelStateBinder>();
        if (binder == null)
            Debug.LogWarning($"没有找到{this.gameObject.name}该对象挂载对应的事件绑定器");     
    }

    public abstract void EnterState();

    public abstract void OnState();

    public abstract void ExitState();

}