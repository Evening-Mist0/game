using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡状态机，用于状态的切换。仅用于挂载到LevelMgr上
/// </summary>
public class LevelStateMachine : MonoBehaviour
{
    
    private BaseLevelState nowState = null;

    Dictionary<E_LevelState, BaseLevelState> LevelStateDic = new Dictionary<E_LevelState, BaseLevelState>();
    

    public void ChangeState(E_LevelState newState)
    {
        if (!LevelStateDic.ContainsKey(newState))
        {
            Debug.LogError("该状态类型不存在,请在LevelStateMachine类中的Awake函数添加该状态");
            return;
        }

        Debug.Log("nowState" + nowState.myState);

        if (nowState.myState == newState)
        {
            Debug.Log("已处于当前状态,无法切换");
            return;
        }

     
       nowState.ExitState();
       nowState = LevelStateDic[newState];
       nowState.EnterState();
    }

    /// <summary>
    /// 开始获取所有的状态,参考E_LevelState,如果有新的状态,需要在后续添加该状态到字典
    /// </summary>
    private void Awake()
    {
        LevelStateDic.Add(E_LevelState.Init, new InitState(this));
        LevelStateDic.Add(E_LevelState.PlayerTurn_DrawCard, new DrawCardState(this));
        nowState = LevelStateDic[E_LevelState.Init];
    }

    void Update()
    {
        if(nowState != null)
        {
            nowState.OnState();
        }
    }
}
