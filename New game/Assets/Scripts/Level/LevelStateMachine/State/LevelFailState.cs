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

        //Dealer.Instance.ResetDealer();//清空荷官记录的手牌
        //GamePlayer.Instance.ResetCardOperation();// 清空合成列表、选中卡牌等
        //GamePlayer.Instance.cardList.Clear();// 如果有使用 cardList 字段
        //隐藏打牌面板
        UIMgr.Instance.HidePanel<CardPlayingPanel>();
        
        //展示失败面板
        UIMgr.Instance.ShowPanel<FailPanel>(E_UILayerType.top);

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
