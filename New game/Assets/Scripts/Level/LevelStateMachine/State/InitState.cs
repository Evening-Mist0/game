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
        //创建地图
        Debug.Log("进入Init状态,初始化地图,生成怪物,显示打牌面板，初始化玩家卡牌");
        GamePlayer.Instance.ShowMe();
        //创建地图
        GridMgr.Instance.CreatGridMap();
        //显示玩家
        GamePlayer.Instance.ShowMe();
        //更新玩家护甲
        GamePlayer.Instance.UpdateDef();
        //更新玩家血条
        GamePlayer.Instance.UpdateBlood();
        //显示打牌面板
        UIMgr.Instance.ShowPanel<CardPlayingPanel>();

        //为荷官获取面板引用
        Dealer.Instance.GetRadicalCardSlot(UIMgr.Instance.GetPanel<CardPlayingPanel>().slotXi);
        Dealer.Instance.GetRadicalCardSlot(UIMgr.Instance.GetPanel<CardPlayingPanel>().slotPi);
        Dealer.Instance.GetRadicalCardSlot(UIMgr.Instance.GetPanel<CardPlayingPanel>().slotKe);
        Dealer.Instance.GetRadicalCardSlot(UIMgr.Instance.GetPanel<CardPlayingPanel>().slotYe);

        //置灰面板
        UIMgr.Instance.GetPanel<CardPlayingPanel>().EnterAsh();
        ////创建怪
        LevelStepMgr.Instance.EnterCreatMonsterState();

        //重置连击
        ComboMgr.Instance.ClearCombo();
        //重置笔墨
        GamePlayer.Instance.ResetInkValue();
        //更新笔墨
        GamePlayer.Instance.AddInkWithGrowInk();
    }

    public override void ExitState()
    {
        Debug.Log("退出Init状态");
        isIniting = true;
    }

    public override void OnState()
    {
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_DrawCard);
    }
}
