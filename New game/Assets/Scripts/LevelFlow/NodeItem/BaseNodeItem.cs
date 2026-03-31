using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 爬塔节点基类（兼容优化版）
/// 原有UI/交互/状态逻辑完全保留，仅补充层级属性适配
/// </summary>
public class BaseNodeItem : MonoBehaviour
{
    #region 序列化UI组件（Inspector拖拽绑定）
    [Header("核心UI组件")]
    [Tooltip("节点点击按钮")]
    [SerializeField] private Button _nodeButton;
    [Tooltip("节点图标（控制明暗/颜色）")]
    [SerializeField] private Image _nodeIcon;
    [Tooltip("锁定遮罩（可选，锁定时显示）")]
    [SerializeField] private Image _lockMask;
    [Tooltip("已完成对勾标记")]
    [SerializeField] private GameObject _completeMark; // 对勾标记
    //[Tooltip("选中高亮效果")]
    //[SerializeField] private GameObject _highlightObj;
    //[Tooltip("BOSS节点闪烁特效（仅BOSS节点绑定）")]
    //[SerializeField] private GameObject _bossFlashEffect; // BOSS高亮闪烁特效
    //[Header("动画配置（可选）")]
    //[Tooltip("节点状态动画控制器")]
    //[SerializeField] private Animator _nodeAnimator;
    #endregion

    #region 节点基础属性（补充layerIndex）
    /// <summary> 节点唯一ID </summary>
    public string nodeId { get; set; }
    /// <summary> 节点类型 </summary>
    public E_TowerNodeType nodeType { get; set; }
    /// <summary> 所属楼层 </summary>
    public int layerIndex { get; set; } // 新增：楼层属性赋值
    /// <summary> 当前节点状态 </summary>
    protected E_NodeState _currentState = E_NodeState.Locked;
    #endregion

    #region 生命周期（完全保留原有逻辑）
    protected virtual void Awake()
    {
        // 绑定节点点击事件
        _nodeButton.onClick.AddListener(OnNodeClick);
        // 初始化默认UI状态（锁定）
        ResetUIState();
        // 监听全局节点状态变更事件（来自LevelFlowMgr）
        EventCenter.Instance.AddEventListener<string>(E_EventType.Tower_NodeStateChanged, OnGlobalNodeStateChanged);
    }

    protected virtual void OnDestroy()
    {
        // 移除事件监听，避免内存泄漏
        EventCenter.Instance.RemoveEventListener<string>(E_EventType.Tower_NodeStateChanged, OnGlobalNodeStateChanged);
        _nodeButton.onClick.RemoveAllListeners();
    }
    #endregion

    #region 核心初始化方法（补充layerIndex赋值）
    /// <summary>
    /// 节点初始化（TowerPanel生成节点时调用）
    /// </summary>
    /// <param name="nodeId">节点唯一ID</param>
    /// <param name="nodeType">节点类型</param>
    /// <param name="layerIndex">所属楼层</param>
    public virtual void Init(string nodeId, E_TowerNodeType nodeType, int layerIndex)
    {
        this.nodeId = nodeId;
        this.nodeType = nodeType;
        this.layerIndex = layerIndex; // 补充：楼层赋值
        // 从LevelFlowMgr获取初始状态并刷新UI
        _currentState = LevelFlowMgr.Instance.GetNodeState(nodeId);
        RefreshNodeState(_currentState);
        // BOSS节点特殊初始化：默认隐藏闪烁特效
        //if (nodeType == E_TowerNodeType.BossBattle && _bossFlashEffect != null)
        //{
        //    _bossFlashEffect.SetActive(false);
        //}
    }
    #endregion

    #region 状态管理核心逻辑（完全保留原有逻辑）
    /// <summary>
    /// 监听全局节点状态变更，同步刷新自身UI
    /// </summary>
    private void OnGlobalNodeStateChanged(string changedNodeId)
    {
        if (changedNodeId != nodeId) return;
        _currentState = LevelFlowMgr.Instance.GetNodeState(nodeId);
        RefreshNodeState(_currentState);
    }

    /// <summary>
    /// 刷新节点UI状态（核心可视化逻辑，完全保留）
    /// </summary>
    public virtual void RefreshNodeState(E_NodeState targetState)
    {
        _currentState = targetState;
        // 先重置所有UI状态（避免叠加）
        ResetUIState();

        Debug.Log($"【最终刷新】节点{nodeId} 目标状态：{targetState}");
        switch (targetState)
        {
            case E_NodeState.Locked:
                // 锁定状态：灰显、不可点击、显示锁定遮罩
                _nodeIcon.color = Color.gray;
                _nodeButton.interactable = false; // 锁定状态强制不可点击
                _lockMask?.gameObject.SetActive(true);
                break;
            case E_NodeState.Unlocked:
                // 解锁状态：正常显示、可点击、隐藏遮罩
                _nodeIcon.color = Color.white;
                _nodeButton.interactable = true; // 【核心修复】解锁状态强制可点击
                //_highlightObj.SetActive(true);
                break;
            case E_NodeState.Current:
                // 当前选中状态：高亮、不可重复点击、播放选中动画
                _nodeIcon.color = Color.yellow;
                _nodeButton.interactable = false;
                //_highlightObj.SetActive(true);
                //_nodeAnimator?.SetTrigger("Selected");
                break;
            case E_NodeState.Completed:
                // 已完成状态：暗灰、不可点击、显示对勾
                _nodeIcon.color = new Color(0.5f, 0.5f, 0.5f);
                _nodeButton.interactable = false;
                _completeMark.SetActive(true);
                break;
            case E_NodeState.BossUnlocked:
                // BOSS解锁状态：高亮闪烁、可点击、播放BOSS特效
                _nodeIcon.color = new Color(1f, 0.8f, 0f);
                _nodeButton.interactable = true; // BOSS解锁状态强制可点击
                //_bossFlashEffect?.SetActive(true);
                //_highlightObj.SetActive(true);
                //_nodeAnimator?.SetTrigger("BossUnlocked");
                break;
        }
        // 【关键日志】刷新后最终的按钮状态，确认赋值生效
        Debug.Log($"【最终校验】节点{nodeId} 刷新后按钮可点击：{_nodeButton.interactable}");
    }

    private void ResetUIState()
    {
        _lockMask?.gameObject.SetActive(false);
        _completeMark.SetActive(false);
        //_highlightObj.SetActive(false);
        //_bossFlashEffect?.SetActive(false);
        // 【关键修复】默认重置按钮为不可点击，由后续状态分支单独赋值
        _nodeButton.interactable = false;
    }
    #endregion

    #region 点击交互逻辑（完全保留原有逻辑）
    protected virtual void OnNodeClick()
    {
        if (_currentState != E_NodeState.Unlocked && _currentState != E_NodeState.BossUnlocked)
        {
            Debug.LogWarning($"节点{nodeId}当前状态为{_currentState}，无法点击");
            return;
        }
        LevelFlowMgr.Instance.EnterNode(nodeId);
    }
    #endregion

    #region 辅助方法（完全保留）
    public void ForceUpdateState()
    {
        _currentState = LevelFlowMgr.Instance.GetNodeState(nodeId);
        RefreshNodeState(_currentState);
    }

    public E_NodeState GetCurrentState()
    {
        return _currentState;
    }
    #endregion
}