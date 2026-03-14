using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardState : BaseLevelState
{
    
    public override E_LevelState myStateType => E_LevelState.PlayerTurn_DrawCard;

    public override void EnterState()
    {
        Debug.Log("进入DrawCardSate");
    }

    public override void ExitState()
    {
        Debug.Log("退出DrawCardSate");
    }

    public override void OnState()
    {
        Debug.Log("处于DrawCardSate");
    }
}
