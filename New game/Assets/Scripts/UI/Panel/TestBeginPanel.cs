using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBeginPanel : BasePanel
{
    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch (name)
        {
            case "btnEnter":
                HandleEnter();
                break;

        }
    }

    private void HandleEnter()
    {
      

        SceneMgr.Instance.LoadSceneAsync("ClimbingTowerScene",() => {
            LevelFlowMgr.Instance.ClearAllData();
            GrowthMgr.Instance.ResetGrowthData();
            //隐藏自己
            UIMgr.Instance.HidePanel<TestBeginPanel>();
            // 重新初始化爬塔面板
            UIMgr.Instance.GetPanel<TowerPanel>()?.ClearTowerPanel();
            UIMgr.Instance.ShowPanel<TowerPanel>(E_UILayerType.middle);
            //初始化游戏流程
            LevelFlowMgr.Instance.InitNewGame();
            AudioMgr.Instance.PlayBGM("爬塔面板_青阶缓行");
        });
    }
}
