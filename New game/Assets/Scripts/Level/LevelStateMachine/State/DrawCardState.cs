using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽牌阶段
/// </summary>
public class DrawCardState : BaseLevelState
{
    /// <summary>
    /// 状态运行标识
    /// </summary>
    private bool isDrawCarding = true;
    public override E_LevelState myStateType => E_LevelState.PlayerTurn_DrawCard;

    public override void EnterState()
    {
        Debug.Log("进入DrawCardState状态");
        if(isDrawCarding)
        {
            Debug.Log("进入DrawCardSate,补充基础卡牌");
            Dealer.Instance.DealBasicCards(false);
        }
        isDrawCarding = false;
    }

    public override void ExitState()
    {
        Debug.Log("退出DrawCardSate");
        isDrawCarding = true;
    }

    public override void OnState()
    {
        if (!isDrawCarding)
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_CardOperate);

    }
}
