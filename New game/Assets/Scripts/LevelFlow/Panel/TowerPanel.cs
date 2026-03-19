using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TowerPanel : BasePanel
{
    [Header("固定节点引用（请在Inspector中拖拽）")]
    public List<TowerNodeItem> allNodeItems = new List<TowerNodeItem>();

    // 节点字典 key:节点ID value:节点Item
    private Dictionary<string, TowerNodeItem> nodeItemDic = new Dictionary<string, TowerNodeItem>();

    // 玩家状态控件
    private TextMeshProUGUI txt_Hp;
    private TextMeshProUGUI txt_LicenseLevel;
    private TextMeshProUGUI txt_Layer;

    protected override void Awake()
    {
        base.Awake();
        // 获取控件
        txt_Hp = GetControl<TextMeshProUGUI>("txt_Hp");
        txt_LicenseLevel = GetControl<TextMeshProUGUI>("txt_LicenseLevel");
        txt_Layer = GetControl<TextMeshProUGUI>("txt_Layer");

        // 初始化节点字典
        InitNodeDictionary();
        // 注册事件监听
        RegisterEvents();
    }

    /// <summary> 初始化节点字典 </summary>
    private void InitNodeDictionary()
    {
        nodeItemDic.Clear();
        foreach (var nodeItem in allNodeItems)
        {
            if (nodeItem != null && !string.IsNullOrEmpty(nodeItem.nodeId))
            {
                if (!nodeItemDic.ContainsKey(nodeItem.nodeId))
                {
                    nodeItemDic.Add(nodeItem.nodeId, nodeItem);
                    // 节点Item不需要在Awake中自己Init，由Panel统一管理
                }
                else
                {
                    Debug.LogWarning($"存在重复的节点ID：{nodeItem.nodeId}，请检查预设");
                }
            }
        }
    }

    private void RegisterEvents()
    {
        // 爬塔初始化完成 刷新所有节点
        EventCenter.Instance.AddEventListener(E_EventType.Tower_InitComplete, RefreshAllNodes);
        // 楼层变更 刷新节点显示
        EventCenter.Instance.AddEventListener<int>(E_EventType.Tower_LayerChanged, OnLayerChanged);
        // 节点状态变更 刷新单个节点
        EventCenter.Instance.AddEventListener<string>(E_EventType.Tower_NodeStateChanged, RefreshNodeState);
        // 玩家血量变更 刷新UI
        EventCenter.Instance.AddEventListener<(int, int)>(E_EventType.Growth_PlayerHpChanged, OnPlayerHpChanged);
        // 执照升级 刷新UI
        EventCenter.Instance.AddEventListener<int>(E_EventType.Growth_LicenseLevelUp, OnLicenseLevelUp);
    }

    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch (name)
        {
            case "btn_BackToMain":
                // 弹出确认弹窗
                //UIMgr.Instance.ShowPanel<ConfirmPanel>(E_UILayerType.top);
                break;
            case "btn_Bag":
                // 打开背包面板
                //UIMgr.Instance.ShowPanel<BagPanel>(E_UILayerType.middle);
                break;
            case "btn_Rule":
                // 打开规则面板
                //UIMgr.Instance.ShowPanel<RulePanel>(E_UILayerType.middle);
                break;
        }
    }

    #region UI刷新方法
    /// <summary> 刷新所有节点状态与显示 </summary>
    private void RefreshAllNodes()
    {
        int currentLayer = LevelFlowMgr.Instance.towerRuntimeData.currentLayerIndex;

        // 遍历所有固定节点
        foreach (var kvp in nodeItemDic)
        {
            string nodeId = kvp.Key;
            TowerNodeItem item = kvp.Value;

            // 获取节点配置
            NodeConfig config = LevelFlowMgr.Instance.GetNodeConfig(nodeId);
            if (config == null)
            {
                item.gameObject.SetActive(false);
                continue;
            }

            // 只显示当前楼层及之前的节点
            LayerConfig layerConfig = LevelFlowMgr.Instance.GetLayerConfig(config.layerIndex);
            if (layerConfig != null && config.layerIndex <= currentLayer)
            {
                item.gameObject.SetActive(true);
                item.InitNode(config); // 初始化节点数据
                item.RefreshState();   // 刷新节点状态
            }
            else
            {
                item.gameObject.SetActive(false);
            }
        }

        // 刷新基础信息
        RefreshBaseInfo();
    }

    /// <summary> 楼层变更回调 </summary>
    private void OnLayerChanged(int layerIndex)
    {
        RefreshAllNodes();
        txt_Layer.text = $"第 {layerIndex} 层";
    }

    /// <summary> 刷新单个节点状态 </summary>
    private void RefreshNodeState(string nodeId)
    {
        if (nodeItemDic.TryGetValue(nodeId, out var item))
        {
            item.RefreshState();
        }
    }

    /// <summary> 刷新玩家基础信息 </summary>
    private void RefreshBaseInfo()
    {
        var growthData = GrowthMgr.Instance.growthData;
        txt_Hp.text = $"{growthData.playerCurrentHp}/{growthData.playerMaxHp}";
        txt_LicenseLevel.text = $"执照等级：{growthData.licenseLevel}";
        txt_Layer.text = $"第 {LevelFlowMgr.Instance.towerRuntimeData.currentLayerIndex} 层";
    }

    private void OnPlayerHpChanged((int currentHp, int maxHp) hpInfo)
    {
        txt_Hp.text = $"{hpInfo.currentHp}/{hpInfo.maxHp}";
    }

    private void OnLicenseLevelUp(int level)
    {
        txt_LicenseLevel.text = $"执照等级：{level}";
    }
    #endregion

    private void OnDestroy()
    {
        // 移除事件监听 避免内存泄漏
        EventCenter.Instance.RemoveEventListener(E_EventType.Tower_InitComplete, RefreshAllNodes);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.Tower_LayerChanged, OnLayerChanged);
        EventCenter.Instance.RemoveEventListener<string>(E_EventType.Tower_NodeStateChanged, RefreshNodeState);
        EventCenter.Instance.RemoveEventListener<(int, int)>(E_EventType.Growth_PlayerHpChanged, OnPlayerHpChanged);
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.Growth_LicenseLevelUp, OnLicenseLevelUp);
    }
}
