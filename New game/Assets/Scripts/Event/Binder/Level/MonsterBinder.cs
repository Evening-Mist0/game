using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物事件注册绑定器,用于注册怪物需要监听的事件
/// </summary>
public class MonsterBinder : MonoBehaviour
{
    MonsterMoveState monsterMoveState;
    private void Awake()
    {
        RegistEvents();
    }

    private void RegistEvents()
    {
        TypeSafeEventCenter.Instance.Register<MonsterTurn_StartMoveEvent>(LevelStepMgr.Instance, OnStartMove);
    }

    public void OnStartMove(MonsterTurn_StartMoveEvent evt)
    {
        BaseMonster monster = evt.SourceMonster;
    }
}
