using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// 爬塔节点UI项
/// 负责节点图标、状态、点击交互
/// </summary>
public class TowerNodeItem : MonoBehaviour
{
    [Header("核心组件")]
    public Button nodeBtn;
    public Image nodeBg; // 节点背景
    public Image nodeIcon; // 节点类型图标
    public Image completedCheckMark; // 已完成对勾
    public GameObject lockMask; // 锁定遮罩
    public GameObject highlightEffect; // 高亮选中效果
    public GameObject blinkEffect; // BOSS节点闪烁特效

    [Header("状态配色")]
    public Color lockedColor = new Color(0.3f, 0.3f, 0.3f, 1f); // 锁定灰色
    public Color unlockedColor = new Color(1f, 1f, 1f, 1f); // 解锁白色
    public Color completedColor = new Color(0.5f, 0.5f, 0.5f, 1f); // 已完成暗灰色

    [Header("图标配置（匹配UI设计）")]
    public Sprite normalBattleIcon; // 普通战斗：毛笔
    public Sprite eliteBattleIcon; // 精英战斗：交叉毛笔
    public Sprite bossBattleIcon; // BOSS战：交叉毛笔+皇冠
    public Sprite campIcon; // 营地：砚台
    public Sprite randomEventIcon; // 随机事件：卷轴铜钱

    private string _currentNodeId;
    private Action<string> _onClickEvent;

    void Awake()
    {
        // 绑定点击事件
        nodeBtn.onClick.AddListener(OnNodeClick);
    }

    /// <summary>
    /// 初始化节点
    /// </summary>
    public void InitNode(TowerNodeData nodeData)
    {
        _currentNodeId = nodeData.nodeId;
        // 强制设置节点尺寸（避免预设体尺寸错误）
        RectTransform nodeRect = transform as RectTransform;
        nodeRect.sizeDelta = new Vector2(60, 60); // 固定节点尺寸60×60px
        // 设置节点图标
        SetNodeIcon(nodeData.nodeType);
        // 刷新节点状态
        RefreshState(nodeData);
    }

    /// <summary>
    /// 设置节点类型图标
    /// </summary>
    private void SetNodeIcon(E_TowerNodeType nodeType)
    {
        Sprite targetIcon = nodeType switch
        {
            E_TowerNodeType.NormalBattle => normalBattleIcon,
            E_TowerNodeType.EliteBattle => eliteBattleIcon,
            E_TowerNodeType.BossBattle => bossBattleIcon,
            E_TowerNodeType.Camp => campIcon,
            E_TowerNodeType.RandomEvent => randomEventIcon,
            _ => normalBattleIcon
        };

        nodeIcon.sprite = targetIcon;
        nodeIcon.SetNativeSize();
    }

    /// <summary>
    /// 刷新节点状态视觉
    /// </summary>
    public void RefreshState(TowerNodeData nodeData)
    {
        // 重置所有特效
        completedCheckMark.gameObject.SetActive(false);
        lockMask.SetActive(false);
        highlightEffect.SetActive(false);
        blinkEffect.SetActive(false);

        switch (nodeData.nodeState)
        {
            case E_NodeState.Locked:
                // 锁定状态：全灰、加遮罩、不可点击
                nodeBg.color = lockedColor;
                nodeIcon.color = lockedColor;
                lockMask.SetActive(true);
                nodeBtn.interactable = false;
                break;

            case E_NodeState.Unlocked:
                // 解锁状态：高亮、可点击
                nodeBg.color = unlockedColor;
                nodeIcon.color = unlockedColor;
                nodeBtn.interactable = true;
                // BOSS节点解锁后开启闪烁特效
                if (nodeData.nodeType == E_TowerNodeType.BossBattle)
                {
                    blinkEffect.SetActive(true);
                }
                break;

            case E_NodeState.Completed:
                // 已完成状态：变暗、显示对勾、不可点击
                nodeBg.color = completedColor;
                nodeIcon.color = completedColor;
                completedCheckMark.gameObject.SetActive(true);
                nodeBtn.interactable = false;
                break;

            case E_NodeState.Current:
                // 当前选中状态：高亮特效
                highlightEffect.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// 节点点击事件
    /// </summary>
    private void OnNodeClick()
    {
        TowerNodeMgr.Instance.OnNodeClick(_currentNodeId);
    }
}