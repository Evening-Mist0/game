using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 该类为关卡状态的父类，具体书写每个状态进入的行为
/// </summary>
public abstract class BaseLevelState 
{
    //当前状态类的状态枚举
    public abstract E_LevelState myState { get; }

    public BaseLevelState(LevelStateMachine machine)
    {
        this.machine = machine;
    }

    //关卡状态机
    public LevelStateMachine machine;

    public abstract void EnterState();

    public abstract void OnState();

    public abstract void ExitState();
 
    
}
