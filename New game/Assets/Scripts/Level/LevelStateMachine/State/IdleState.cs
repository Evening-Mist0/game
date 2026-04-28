using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseLevelState
{
    public override E_LevelState myStateType => E_LevelState.Idle;

    public override void EnterState()
    {
        Debug.Log("½øÈëÏĞÖÃ×´Ì¬");
        Time.timeScale = 1.0f;

    }

    public override void ExitState()
    {
        Debug.Log("ÍË³öÏĞÖÃ×´Ì¬");

    }

    public override void OnState()
    {

    }


}
