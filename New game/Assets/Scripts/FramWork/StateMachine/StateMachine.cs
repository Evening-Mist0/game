using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//状态机类，用于管理怪物的状态切换
/// <summary>
/// 使用该类的时候为怪物怪载该脚本即可
/// </summary>
public class StateMachine : MonoBehaviour
{
    Dictionary<E_AIStateType, BaseMonsterState> stateDic = new Dictionary<E_AIStateType, BaseMonsterState>();
    BaseMonsterState nowState;
    [HideInInspector]
    public E_AIStateType currentState;

    //使用该状态机的类必将继承该接口，可以通过里氏替换获得该怪物的类
    public I_AIAction I_aiAction;

    public void Init(I_AIAction action)
    {
        I_aiAction = action;
    }

    /// <summary>
    /// 添加状态
    /// </summary>
    /// <param name="type">状态类型</param>
    public void AddState(E_AIStateType type)
    {
        if (!stateDic.ContainsKey(type))
        {
            switch (type)
            {
                case E_AIStateType.Atk:             
                stateDic.Add(type, new AtkState(this));               
                break;

                case E_AIStateType.Move:               
                stateDic.Add(type, new MoveState(this));
                break;

                case E_AIStateType.Dead:
                stateDic.Add(type, new DeadState(this));
                break;
            }
        }

    }
    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="stateType">状态类型</param>
    public void ChangeState(E_AIStateType stateType)
    {
        if (nowState != null)
            nowState.ExitState();

       if(stateDic.ContainsKey(stateType))
        {
            nowState = stateDic[stateType];
            currentState = stateType;
        }
        EnterState();
    }

    public void UpdateState()
    {
        if(nowState!= null)
        {
            nowState.OnState();
        }
    }

    public void EnterState()
    {
        if(nowState != null)
        {
            nowState.EnterState();
        }
    }
}
