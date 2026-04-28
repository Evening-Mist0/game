using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailPanel : BasePanel
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

        UIMgr.Instance.HidePanel<FailPanel>();

        AudioMgr.Instance.PlayBGM("爬塔面板_青阶缓行");

        //关卡回归初始
        LevelStepMgr.Instance.ResetMe();
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.Idle);

        EventCenter.Instance.EventTrigger(E_EventType.UI_LevelOver);
        SceneMgr.Instance.LoadSceneAsync("BeginScene", () => {
            BattleMgr.Instance.SimulateBattleLose();
            //清理对象池
            PoolMgr.Instance.Clear();
        });

    }

    public override void ShowMe()
    {
        base.ShowMe();
        AudioMgr.Instance.PlaySFX("局内游戏失败");
    }
}
