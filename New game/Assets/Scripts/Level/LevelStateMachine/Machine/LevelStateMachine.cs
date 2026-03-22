using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 关卡状态机，用于状态的切换。仅用于挂载到LevelMgr上
/// </summary>
public class LevelStateMachine : BaseMonoMgr<LevelStateMachine>
{
    
    public BaseLevelState nowState = null;

    private E_LevelState nowStateType;

    public E_LevelState NowStateType => nowStateType;

    Dictionary<E_LevelState, BaseLevelState> LevelStateDic = new Dictionary<E_LevelState, BaseLevelState>();


    public void AddState<T>(T state) where T : BaseLevelState
    {
        if (!LevelStateDic.ContainsKey(state.myStateType))
        {
            switch (state.myStateType)
            {
                case E_LevelState.Init:
                    LevelStateDic.Add(E_LevelState.Init, state);
                    break;
                case E_LevelState.PlayerTurn_DrawCard:
                    LevelStateDic.Add(E_LevelState.PlayerTurn_DrawCard, state);
                    break;
                case E_LevelState.PlayerTurn_CardOperate:
                    LevelStateDic.Add(E_LevelState.PlayerTurn_CardOperate, state);
                    break;
                case E_LevelState.PlayerTurn_EndSettle:
                    LevelStateDic.Add(E_LevelState.PlayerTurn_EndSettle, state);
                    break;
                case E_LevelState.MonsterTurn_EnterSettle:
                    LevelStateDic.Add(E_LevelState.MonsterTurn_EnterSettle, state);
                    break;  
                case E_LevelState.MonsterTurn_CreatMonster:
                    LevelStateDic.Add(E_LevelState.MonsterTurn_CreatMonster, state);
                    break;
                case E_LevelState.MonsterTurn_Move:
                    LevelStateDic.Add(E_LevelState.MonsterTurn_Move, state);
                    break;
                //case E_LevelState.MonsterTurn_EndSettle:
                //    LevelStateDic.Add(E_LevelState.MonsterTurn_EndSettle, state);
                    //break;    
                case E_LevelState.LevelWin:
                    LevelStateDic.Add(E_LevelState.LevelWin, state);
                    break;
                case E_LevelState.LevelLose:
                    LevelStateDic.Add(E_LevelState.LevelLose, state);
                    break;
              
                case E_LevelState.Pause:
                    LevelStateDic.Add(E_LevelState.Pause, state);
                    break;
            }
        }
    }

    /// <summary>
    /// 获取字典中含有的状态
    /// </summary>
    /// <returns></returns>
    public T GetState<T>(E_LevelState stateType) where T : BaseLevelState
    {
        if(LevelStateDic.ContainsKey(stateType))
        return LevelStateDic[stateType] as T;
        else
        {
            Debug.LogError("传入的参数有误，无法获得当前状态");
            return null;
        }
    }

    public void UpdateState()
    {
        if (nowState != null)
        {
            nowState.OnState();
        }
    }

    public void EnterState()
    {
        if (nowState != null)
        {
            nowState.EnterState();
        }
    }

    public void ChangeState(E_LevelState newStateType)
    {
        if (!LevelStateDic.ContainsKey(newStateType))
        {
            Debug.LogError("该状态类型" + newStateType + "不存在,请检查LevelMgr上是否挂载了该状态脚本");
            return;
        }     

        if (nowState != null)
        {
            Debug.Log("执行退出状态");
            nowState.ExitState();
        }

        nowState = LevelStateDic[newStateType];
        nowStateType = newStateType;
        nowState.EnterState();
    }

 

    void Update()
    {
        UpdateState();
    }
}
