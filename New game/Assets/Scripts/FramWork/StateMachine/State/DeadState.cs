using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : BaseMonsterState
{
    public DeadState(StateMachine machine) : base(machine)
    {
    }

    public override E_AIStateType AIState => E_AIStateType.Dead;

    public override void EnterState()
    {
        throw new System.NotImplementedException();
    }

    public override void ExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void OnState()
    {
        throw new System.NotImplementedException();
    }
}
