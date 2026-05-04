using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScholarPanel : BasePanel
{
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button cancelBtn;
    private string nodeId;

    public void Init(string nodeId)
    {
        this.nodeId = nodeId;
        descText.text = "偶遇学者，花费30铜钱可提升一级执照经验。";
        confirmBtn.onClick.AddListener(OnConfirm);
        cancelBtn.onClick.AddListener(OnCancel);
    }

    private void OnConfirm()
    {
        if (GrowthMgr.Instance.GetCopperCoins() < 30)
        {
            ShowTip("铜钱不足");
            return;
        }
        if (GrowthMgr.Instance.growthData.licenseLevel >= 5)
        {
            ShowTip("已达最高等级，无法提升");
            return;
        }
        GrowthMgr.Instance.SpendCopperCoins(30);
        // 增加2点经验（因为一级需要2点经验）
        GrowthMgr.Instance.AddLicenseExp(2);
        Finish();
    }

    private void Finish()
    {
        LevelFlowMgr.Instance.CompleteNode(nodeId);
        UIMgr.Instance.HidePanel<ScholarPanel>();
        UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
    }

    private void OnCancel() => Finish();

    private void ShowTip(string msg) 
    {
        UIMgr.Instance.ShowPanel<TipPanel>(E_UILayerType.bottom,(panel) =>
        {
            panel.Init(msg);
        });
    }
}
