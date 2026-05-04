using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealerPanel : BasePanel
{
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button cancelBtn;
    private string nodeId;

    public void Init(string nodeId)
    {
        this.nodeId = nodeId;
        descText.text = "偶遇医师，花费30铜钱可恢复全部血量。";
        confirmBtn.onClick.AddListener(OnConfirm);
        cancelBtn.onClick.AddListener(OnCancel);
    }

    private void OnConfirm()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        if (GrowthMgr.Instance.GetCopperCoins() < 30)
        {
            ShowTip("铜钱不足");
            return;
        }
        GrowthMgr.Instance.SpendCopperCoins(30);
        int maxHp = GrowthMgr.Instance.growthData.playerMaxHp;
        GrowthMgr.Instance.PlayerRecoverHp(maxHp);
        Finish();
    }

    private void OnCancel() => Finish();

    private void Finish()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        LevelFlowMgr.Instance.CompleteNode(nodeId);
        UIMgr.Instance.HidePanel<HealerPanel>();
        UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
    }

    private void ShowTip(string msg) 
    {
        UIMgr.Instance.ShowPanel<TipPanel>(E_UILayerType.bottom,(panel) =>
        {
            panel.Init(msg);
        });
    }
}
