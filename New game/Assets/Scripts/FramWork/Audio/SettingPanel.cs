using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingPanel : BasePanel
{
    [Header("BGM")]
    [SerializeField] private Toggle bgmToggle;
    [SerializeField] private Slider bgmSlider;
    [Header("SFX")]
    [SerializeField] private Toggle sfxToggle;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button closeBtn;

    private float lastBgmVolume = 0.8f;   
    private float lastSfxVolume = 0.8f;   

    protected override void Awake()
    {
        base.Awake();
        bgmSlider.onValueChanged.AddListener(OnBgmSliderChanged);
        sfxSlider.onValueChanged.AddListener(OnSfxSliderChanged);
        bgmToggle.onValueChanged.AddListener(OnBgmToggleChanged);
        sfxToggle.onValueChanged.AddListener(OnSfxToggleChanged);
        closeBtn.onClick.AddListener(OnClose);
    }

    public override void ShowMe()
    {
        base.ShowMe();
        // 加载保存的数值
        LoadSettings();
        // 初始化 UI 显示
        UpdateUI();
        //更新玩家呼出设置面板标识
        GamePlayer.Instance.callExit.isOpen = true;
    }

    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch(name)
        {
            case "btnReturn":
                HandlebtnReturn();
            break;
        }
    }

    private void HandlebtnReturn()
    {
        GoBackToStart();
    }

    /// <summary>
    /// 按钮绑定这个方法
    /// 安全、无报错、彻底清理
    /// </summary>
    void GoBackToStart()
    {
        ClearMgrInstance<UIMgr>();

        // 1. 销毁所有跨场景残留的对象（最关键）
        DestroyAllDontDestroyOnLoad();

        // 2. 加载初始场景 = 回到刚打开游戏的状态
        SceneManager.LoadScene("BeginScene");
    }

    void ClearMgrInstance<T>() where T : class
    {
        var type = typeof(T);
        var field = type.BaseType.GetField("instance",
            BindingFlags.NonPublic | BindingFlags.Static);

        if (field != null)
        {
            field.SetValue(null, null);
        }
    }

    /// <summary>
    /// 安全清理所有DontDestroyOnLoad，不报错
    /// </summary>
    void DestroyAllDontDestroyOnLoad()
    {
        // 找到所有DontDestroy对象
        GameObject[] objs = DontDestroyOnLoadObjects();

        foreach (var go in objs)
        {
            // 不销毁自己（避免执行中断）
            if (go == gameObject) continue;

            Destroy(go);
        }
    }

    /// <summary>
    /// 安全获取DontDestroyOnLoad对象（Unity官方方法，无报错）
    /// </summary>
    GameObject[] DontDestroyOnLoadObjects()
    {
        var temp = new GameObject();
        DontDestroyOnLoad(temp);

        var scene = temp.scene;
        DestroyImmediate(temp);

        return scene.GetRootGameObjects();
    }



private void LoadSettings()
    {
        // 从 PlayerPrefs 读取，没有则使用默认值 0.8
        lastBgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.8f);
        lastSfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.8f);
        bool bgmMuted = PlayerPrefs.GetInt("BgmMuted", 0) == 1;
        bool sfxMuted = PlayerPrefs.GetInt("SfxMuted", 0) == 1;

        // 应用静音状态到音量
        if (bgmMuted)
            AudioMgr.Instance.bgmVolume = 0f;
        else
            AudioMgr.Instance.bgmVolume = lastBgmVolume;

        if (sfxMuted)
            AudioMgr.Instance.sfxVolume = 0f;
        else
            AudioMgr.Instance.sfxVolume = lastSfxVolume;

        // 更新控件值
        bgmSlider.value = lastBgmVolume;
        sfxSlider.value = lastSfxVolume;
        bgmToggle.isOn = !bgmMuted;
        sfxToggle.isOn = !sfxMuted;
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat("BgmVolume", lastBgmVolume);
        PlayerPrefs.SetFloat("SfxVolume", lastSfxVolume);
        PlayerPrefs.SetInt("BgmMuted", bgmToggle.isOn ? 0 : 1);
        PlayerPrefs.SetInt("SfxMuted", sfxToggle.isOn ? 0 : 1);
        PlayerPrefs.Save();
    }

    private void UpdateUI()
    {
        bgmSlider.interactable = bgmToggle.isOn;
        sfxSlider.interactable = sfxToggle.isOn;
    }

    private void OnBgmSliderChanged(float value)
    {
        lastBgmVolume = value;
        if (bgmToggle.isOn)
        {
            AudioMgr.Instance.bgmVolume = value;
        }
        // 如果静音状态，不实际改变音量，但存储目标音量
        SaveSettings();
    }

    private void OnSfxSliderChanged(float value)
    {
        lastSfxVolume = value;
        if (sfxToggle.isOn)
        {
            AudioMgr.Instance.sfxVolume = value;
        }
        SaveSettings();
    }

    private void OnBgmToggleChanged(bool isOn)
    {
        if (isOn)
            AudioMgr.Instance.bgmVolume = lastBgmVolume;
        else
            AudioMgr.Instance.bgmVolume = 0f;
        UpdateUI();
        SaveSettings();
    }

    private void OnSfxToggleChanged(bool isOn)
    {
        if (isOn)
            AudioMgr.Instance.sfxVolume = lastSfxVolume;
        else
            AudioMgr.Instance.sfxVolume = 0f;
        UpdateUI();
        SaveSettings();
    }

    private void OnClose()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");
        SaveSettings();
        HideMe();
        //更新玩家呼出设置面板标识
        GamePlayer.Instance.callExit.isOpen = false;
    }
}