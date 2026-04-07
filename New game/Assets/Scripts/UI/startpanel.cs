using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startpanel : BasePanel
{
    [Header("游戏主场景名称（请填写正确）")]
    public string gameSceneName = "GameMain";

    /// <summary>
    /// 重写按钮点击事件（框架自动调用）
    /// </summary>
    protected override void ButtonClick(string name)
    {
        switch (name)
        {
            // 开始游戏按钮
            case "startBtn":
                OnStartGame();
                break;

            // 设置按钮
            case "setBtn":
                OnSetting();
                break;

            // 退出游戏按钮
            case "exitBtn":
                OnExitGame();
                break;
        }
    }

    /// <summary>
    /// 开始游戏
    /// </summary>
    private void OnStartGame()
    {
        Debug.Log("开始游戏，加载场景：" + gameSceneName);
        SceneManager.LoadScene(gameSceneName);
    }

    /// <summary>
    /// 打开设置
    /// </summary>
    private void OnSetting()
    {
        Debug.Log("打开设置面板");
        // 你后续可以在这里写：UIMgr.Instance.OpenPanel<SettingPanel>();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    private void OnExitGame()
    {
        Debug.Log("退出游戏");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
