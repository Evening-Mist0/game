using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerNodeItem : MonoBehaviour
{
    [Header("节点配置（请在Inspector中填写）")]
    public string nodeId; // 节点唯一ID，必须与配置SO中的ID一致

    private NodeConfig nodeConfig;
    private Button btn_Node;
    private Image img_NodeIcon;
    private TextMeshProUGUI txt_NodeName;
    private Image img_CompletedMark; // 已完成标记（可选）

    // 不同节点类型的图标 需在Inspector赋值
    public Sprite normalBattleSprite;
    public Sprite eliteBattleSprite;
    public Sprite campSprite;
    public Sprite bossSprite;
    public Sprite eventSprite;

    private void Awake()
    {
        // 获取控件引用
        btn_Node = GetComponent<Button>();
        img_NodeIcon = transform.Find("img_Icon")?.GetComponent<Image>();
        txt_NodeName = transform.Find("txt_Name")?.GetComponent<TextMeshProUGUI>();
        img_CompletedMark = transform.Find("img_CompletedMark")?.GetComponent<Image>();

        if (btn_Node != null)
        {
            btn_Node.onClick.AddListener(OnNodeClick);
        }
    }

    /// <summary> 初始化节点数据（由TowerPanel调用） </summary>
    public void InitNode(NodeConfig config)
    {
        nodeConfig = config;
        // 确保ID一致
        if (nodeId != config.nodeId)
        {
            Debug.LogWarning($"节点预设ID({nodeId})与配置ID({config.nodeId})不一致，请检查！");
        }

        // 设置节点名称和图标
        if (txt_NodeName != null)
            txt_NodeName.text = GetNodeName(config.nodeType);
        if (img_NodeIcon != null)
            img_NodeIcon.sprite = GetNodeSprite(config.nodeType);
    }

    /// <summary> 刷新节点状态 </summary>
    public void RefreshState()
    {
        if (nodeConfig == null) return;

        E_NodeState state = LevelFlowMgr.Instance.GetNodeState(nodeConfig.nodeId);

        // 根据状态更新UI
        switch (state)
        {
            case E_NodeState.Locked:
                if (btn_Node != null) btn_Node.interactable = false;
                if (img_NodeIcon != null) img_NodeIcon.color = Color.gray;
                if (img_CompletedMark != null) img_CompletedMark.gameObject.SetActive(false);
                break;
            case E_NodeState.Unlocked:
                if (btn_Node != null) btn_Node.interactable = true;
                if (img_NodeIcon != null) img_NodeIcon.color = Color.white;
                if (img_CompletedMark != null) img_CompletedMark.gameObject.SetActive(false);
                break;
            case E_NodeState.Completed:
                if (btn_Node != null) btn_Node.interactable = false;
                if (img_NodeIcon != null) img_NodeIcon.color = Color.gray;
                if (img_CompletedMark != null) img_CompletedMark.gameObject.SetActive(true);
                break;
            case E_NodeState.Current:
                if (btn_Node != null) btn_Node.interactable = true;
                if (img_NodeIcon != null) img_NodeIcon.color = Color.yellow; // 当前选中高亮
                if (img_CompletedMark != null) img_CompletedMark.gameObject.SetActive(false);
                break;
        }
    }

    private void OnNodeClick()
    {
        if (nodeConfig != null)
        {
            LevelFlowMgr.Instance.EnterNode(nodeConfig.nodeId);
        }
    }

    private string GetNodeName(E_TowerNodeType type)
    {
        return type switch
        {
            E_TowerNodeType.NormalBattle => "普通战斗",
            E_TowerNodeType.EliteBattle => "精英战斗",
            E_TowerNodeType.Camp => "休整营地",
            E_TowerNodeType.BossBattle => "BOSS战",
            E_TowerNodeType.RandomEvent => "随机事件",
            _ => ""
        };
    }

    private Sprite GetNodeSprite(E_TowerNodeType type)
    {
        return type switch
        {
            E_TowerNodeType.NormalBattle => normalBattleSprite,
            E_TowerNodeType.EliteBattle => eliteBattleSprite,
            E_TowerNodeType.Camp => campSprite,
            E_TowerNodeType.BossBattle => bossSprite,
            E_TowerNodeType.RandomEvent => eventSprite,
            _ => normalBattleSprite
        };
    }
}
