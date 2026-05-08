using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static CampNodeItem;

public class CampPanel : BasePanel
{
    [SerializeField] private Button tiaoXiBtn;
    [SerializeField] private Button wuDaoBtn;
    [SerializeField] private TextMeshProUGUI tiaoXiDesc;
    [SerializeField] private TextMeshProUGUI wuDaoDesc;

    private string currentNodeId; // 记录关联的节点ID

    protected override void Awake()
    {
        base.Awake();
        tiaoXiBtn.onClick.AddListener(OnTiaoXi);
        wuDaoBtn.onClick.AddListener(OnWuDao);
    }

    /// <summary>
    /// 显示面板，并传入节点ID
    /// </summary>
    public void ShowWithNodeId(string nodeId)
    {
        currentNodeId = nodeId;
        ShowMe();

        //检测血量
        //if(GrowthMgr.Instance.growthData.playerCurrentHp == GrowthMgr.Instance.growthData.playerMaxHp)
        //{
        //    tiaoXiBtn.interactable = false;
        //}
        int unownedCount = GrowthMgr.Instance.GetTotalUnownedBooksCount();
        bool canWuDao = unownedCount >= 2;

        wuDaoBtn.interactable = canWuDao;
        if (!canWuDao)
        {
            if (unownedCount < 2)
                wuDaoDesc.text = "未获得的典籍不足2本，无法悟道";
            else
                wuDaoDesc.text = "无法悟道";
        }
        else
        {
            wuDaoDesc.text = "从四本典籍中选择两本获得";
        }


    }

    private void OnTiaoXi()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        // 触发事件，携带选项和节点ID
        // 加血
        Debug.Log($"加血前血量: {GrowthMgr.Instance.growthData.playerCurrentHp}");
        GrowthMgr.Instance.PlayerRecoverHp(15);
        Debug.Log($"加血后血量: {GrowthMgr.Instance.growthData.playerCurrentHp}");
        EventCenter.Instance.EventTrigger(E_EventType.Camp_OptionConfirm, (E_CampOption.TiaoXi, currentNodeId));
        ClosePanel();
    }

    private void OnWuDao()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        EventCenter.Instance.EventTrigger(E_EventType.Camp_OptionConfirm, (E_CampOption.WuDao, currentNodeId));
        //ClosePanel(); // 关闭营地面板，后续典籍选择面板由节点打开
    }


    private void ClosePanel()
    {
        HideMe(); // 销毁面板
        // 显示爬塔面板
        UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
    }
}
