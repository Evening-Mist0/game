using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startpanel : BasePanel
{

    /// <summary>
    /// 重写按钮点击事件（框架自动调用）
    /// </summary>
    protected override void ButtonClick(string name)
    {
        switch (name)
        {
            // 开始游戏按钮
            case "btnStart":
                HandleEnter();
                break;

            // 设置按钮
            case "btnSet":
                OnSetting();
                break;

            // 退出游戏按钮
            case "btnExit":
                OnExitGame();
                break;
        }
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    

    /// <summary>
    /// 打开设置
    /// </summary>
    private void OnSetting()
    {
        Debug.Log("打开设置面板");
        UIMgr.Instance.ShowPanel<RulePanel>(E_UILayerType.middle);
        // 你后续可以在这里写：UIMgr.Instance.OpenPanel<SettingPanel>();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void OnExitGame()
    {
        Application.Quit();
    }
    private void HandleEnter()
    {


        SceneMgr.Instance.LoadSceneAsync("ClimbingTowerScene", () => {
            LevelFlowMgr.Instance.ClearAllData();
            GrowthMgr.Instance.ResetGrowthData();
            //隐藏自己
            UIMgr.Instance.HidePanel<startpanel>();
            // 重新初始化爬塔面板
            UIMgr.Instance.GetPanel<TowerPanel>()?.ClearTowerPanel();
            UIMgr.Instance.ShowPanel<TowerPanel>(E_UILayerType.middle);
            //初始化游戏流程
            LevelFlowMgr.Instance.InitNewGame();
            AudioMgr.Instance.PlayBGM("爬塔面板_青阶缓行");
        });
    }
}
