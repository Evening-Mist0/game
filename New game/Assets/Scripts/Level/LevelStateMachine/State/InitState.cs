using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitState : BaseLevelState
{
    /// <summary>
    /// 状态运行标识
    /// </summary>
    private bool isIniting = true;

    public override E_LevelState myStateType => E_LevelState.Init;


    public override void EnterState()
    {
        if(isIniting)
        {
            Debug.Log("进入Init状态,初始化地图,生成怪物,显示打牌面板，初始化玩家卡牌");
            //创建地图
            GridMgr.Instance.CreatGridMap();
           
            //更新玩家护甲
            GamePlayer.Instance.UpdateDef();
            //显示打牌面板
            UIMgr.Instance.ShowPanel<CardPlayingPanel>();
            //为荷官获取面板引用
            Dealer.Instance.GetRadicalCardSlot(UIMgr.Instance.GetPanel<CardPlayingPanel>().slotXi);
            Dealer.Instance.GetRadicalCardSlot(UIMgr.Instance.GetPanel<CardPlayingPanel>().slotPi);
            Dealer.Instance.GetRadicalCardSlot(UIMgr.Instance.GetPanel<CardPlayingPanel>().slotKe);
            Dealer.Instance.GetRadicalCardSlot(UIMgr.Instance.GetPanel<CardPlayingPanel>().slotYe);
            //置灰面板
            UIMgr.Instance.GetPanel<CardPlayingPanel>().EnterAsh();
            //发牌
            if(LevelStepMgr.Instance.currentWave < 1)
            Dealer.Instance.DealBasicCards(true);;

            ////创建怪
            LevelStepMgr.Instance.EnterCreatMonsterState();

        }
        isIniting = false;
    }

    public override void ExitState()
    {
        Debug.Log("退出Init状态");
        isIniting = true;
    }

    public override void OnState()
    {
        if (!isIniting)
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_DrawCard);
        else
            Debug.Log("处于Init状态");

    }

    public override void Init()
    {
        base.Init();
    }
}
