using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailTeachPanel : BasePanel
{
    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch (name)
        {
            case "btnSure":
                HandleSure();
                break;

        }
    }

    private void HandleSure()
    {



        AudioMgr.Instance.PlaySFX("选牌音效");

        UIMgr.Instance.HidePanel<FailTeachPanel>();

        Destroy(GamePlayer.Instance.gameObject);
        EventCenter.Instance.EventTrigger(E_EventType.UI_LevelOver);
        //关卡回归初始
        LevelStepMgr.Instance.ResetMe();
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.Idle);
        //清理教学状态
        LevelStepMgr.Instance.isTeach = false;
        TeachState state = LevelStepMgr.Instance.machine.GetState<TeachState>(E_LevelState.PlayerTurn_Teach);
        if (state != null)
        {
            state.ResetEnterCount();
            state.HideAllPanel();
        }

        SceneMgr.Instance.LoadSceneAsync("BeginScene", () => {
            //清理对象池
            PoolMgr.Instance.Clear();
            GamePlayer.Instance.playerBag.ClearAllItems();
            UIMgr.Instance.HidePanel<PlayerInfoPanel>();
        });
    }
    public override void ShowMe()
    {
        base.ShowMe();
        AudioMgr.Instance.PlaySFX("局内游戏失败");
        UIMgr.Instance.HidePanel<InkExchangePanel>();

    }
}
