using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelFailState : BaseLevelState
{
    public override E_LevelState myStateType => E_LevelState.LevelLose;

    public override void EnterState()
    {
        Debug.Log("进入游戏失败状态");

        GamePlayer.Instance.RemoveAllCardInCompositeList();

        //隐藏打牌面板
        UIMgr.Instance.HidePanel<CardPlayingPanel>();
        UIMgr.Instance.HidePanel<InkExchangePanel>();

        //展示失败面板
        if (LevelStepMgr.Instance.isTeach)
            UIMgr.Instance.ShowPanel<FailTeachPanel>(E_UILayerType.system);
        else
            UIMgr.Instance.ShowPanel<FailPanel>(E_UILayerType.system);


    }

    public override void ExitState()
    {
        Debug.Log("退出游戏失败状态");
        GamePlayer.Instance.playerBag.ClearAllItems();
        GamePlayer.Instance.HideMe();
    }

    public override void OnState()
    {
        
    }

   
}
