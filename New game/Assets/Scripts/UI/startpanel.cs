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
            //教学关卡
            case "btnTeach":
                OnTeach();
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
        AudioMgr.Instance.PlaySFX("按钮点击");

        //UIMgr.Instance.ShowPanel<RulePanel>(E_UILayerType.middle);
        UIMgr.Instance.ShowPanel<SettingPanel>(E_UILayerType.middle);
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void OnExitGame()
    {
        Application.Quit();
    }

    private void OnTeach()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");
        //添加奇物
        GamePlayer.Instance.playerBag.AddTreasure("FireMask");

        SceneMgr.Instance.LoadSceneAsync("TeachScene", () => {
            LevelFlowMgr.Instance.ClearAllData();
            GrowthMgr.Instance.ResetGrowthData();
            //隐藏自己
            UIMgr.Instance.HidePanel<startpanel>();
     
            //切换战斗音乐
            AudioMgr.Instance.PlayBGM("普通关_墨影阵图");

            // 构建战斗信息
            BattleInfo info = new BattleInfo();
            info.monsterCounts = 7;

            LevelStepMgr.Instance.isTeach = true;
            LevelStepMgr.Instance.UpdateBattleInfo(info);

            //重置玩家状态
            GamePlayer.Instance.effectControl.RestAnimator();
          

        });
     }
    private void HandleEnter()
    {

        AudioMgr.Instance.PlaySFX("按钮点击");
        SceneMgr.Instance.LoadSceneAsync("ClimbingTowerScene", () => {
            LevelFlowMgr.Instance.ClearAllData();
            GrowthMgr.Instance.ResetGrowthData();
            //隐藏自己
            UIMgr.Instance.HidePanel<startpanel>();
            // 重新初始化爬塔面板
            UIMgr.Instance.GetPanel<TowerPanel>()?.ClearTowerPanel();
            UIMgr.Instance.ShowPanel<TowerPanel>(E_UILayerType.middle);
            UIMgr.Instance.ShowPanel<PlayerInfoPanel>(E_UILayerType.system);
            //初始化游戏流程
            LevelFlowMgr.Instance.InitNewGame();
            AudioMgr.Instance.PlayBGM("爬塔面板_青阶缓行");
            //显示教程面板
            UIMgr.Instance.ShowPanel<AllRulePanel>();
        });
    }
}
