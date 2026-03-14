using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 这个状态用于继承其他的怪物状态类，注意是怪物状态类
/// </summary>
public abstract class BaseMonsterState
{
    //状态的持有者
    protected StateMachine stateMachine;

    public BaseMonsterState(StateMachine machine)
    {
        stateMachine = machine;
    }

    
    //当前状态的状态类型
    public abstract E_AIStateType AIState { get; }
   
    abstract public void EnterState();

    abstract public void OnState();

    abstract public void ExitState();
}
