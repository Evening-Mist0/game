using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerInfoPanel : BasePanel
{
    #region 局外玩家血量

    [Tooltip("最大生命值")]
    public int maxHp => GrowthMgr.Instance.growthData.playerMaxHp;

    [Tooltip("当前生命值")]
    public int currentHp => GrowthMgr.Instance.growthData.playerCurrentHp;

    public TMP_Text blood;

    [SerializeField] private Button bagBtn;
    [SerializeField] private Button ruleBtn;
    [SerializeField] private Button settingBtn;

    //玩家经验可视化
    public PlayerLevelViewControl playerLevelViewControl;

    public void UpdateBlood((int hp,int maxHp) hpData)
    {
        //更新text血量
        string strBlood = hpData.hp.ToString() + "/" + hpData.maxHp.ToString();
        blood.text = strBlood;
    }


    #endregion

    protected override void Awake()
    {
        base.Awake();
        bagBtn.onClick.AddListener(OpenBag);
        ruleBtn.onClick.AddListener(OpenRule);
        settingBtn.onClick.AddListener(OpenSetting);

        EventCenter.Instance.AddEventListener<(int, int)>(E_EventType.UI_PlayerInfoUpdate, UpdateBlood);
        EventCenter.Instance.AddEventListener(E_EventType.UI_PlayerLevelUpdate, UpdatePlayerLevel);
        
        //初始化血量
        UpdateBlood((currentHp,maxHp));
        if (playerLevelViewControl == null)
            Debug.LogError("请挂载PlayerLevelViewControl组件");
    }

    #region 玩家面板
    public void OpenBag()
    {
        UIMgr.Instance.ShowPanel<BackpackPanel>(E_UILayerType.system);
    }

    public void OpenRule()
    {
        UIMgr.Instance.ShowPanel<AllRulePanel>();
    }
   
    public void OpenSetting()
    {
        UIMgr.Instance.ShowPanel<SettingPanel>(E_UILayerType.middle);
    }

    public void UpdatePlayerLevel()
    {
        playerLevelViewControl.UpdateLevelCapacity();
        playerLevelViewControl.UpdateNowLevel();
    }


    #endregion
    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<(int, int)>(E_EventType.UI_PlayerInfoUpdate, UpdateBlood);
        EventCenter.Instance.RemoveEventListener(E_EventType.UI_PlayerLevelUpdate, UpdatePlayerLevel);

    }

}
