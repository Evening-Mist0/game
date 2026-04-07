using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPanel : BasePanel
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
        UIMgr.Instance.HidePanel<VictoryPanel>();


        //切换音乐
        AudioMgr.Instance.PlayBGM("爬塔面板_青阶缓行");
        //关卡回归初始
        LevelStepMgr.Instance.ResetMe();
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.Idle);

            SceneMgr.Instance.LoadSceneAsync("ClimbingTowerScene", () => {
                BattleMgr.Instance.SimulateBattleWin();
                //清理对象池
                PoolMgr.Instance.Clear();
            });
      
    }
}
