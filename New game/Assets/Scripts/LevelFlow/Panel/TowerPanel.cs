using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 楼层节点分组枚举（替代原字符串Key，避免拼写错误）
public enum TowerNodeGroup
{
    TwoNodes,
    ThreeNodes,
    FourNodes,
    FiveNodes,
    SixNodes,
    SevenNodes
}

public class TowerPanel : BasePanel
{
    // 常量定义（完全保留原有常量）
    private const string NODE_ITEM_RESOURCE_PATH = "UI/NodeItem/";
    private const int RANDOM_NODE_TYPE_COUNT = 4;

    // 启程按钮（原有字段，补充绑定逻辑）
    //public Button departBtn;

    [Header("第一层节点")]
    [SerializeField] private GameObject _startNode;
    [Header("第二层随机节点容器")]
    [SerializeField] private List<GameObject> _twoRandomNodeContainers = new List<GameObject>();
    [Header("第二层固定节点")]
    [SerializeField] private GameObject _secondFixedNode;
    [Header("第三层随机节点容器")]
    [SerializeField] private List<GameObject> _threeRandomNodeContainers = new List<GameObject>();
    [Header("第三层精英节点")]
    [SerializeField] private GameObject _eliteNode;
    [Header("第四层随机节点容器")]
    [SerializeField] private List<GameObject> _fourRandomNodeContainers = new List<GameObject>();
    [Header("第五层随机节点容器")]
    [SerializeField] private List<GameObject> _fiveRandomNodeContainers = new List<GameObject>();
    [Header("第六层随机节点容器")]
    [SerializeField] private List<GameObject> _sixRandomNodeContainers = new List<GameObject>();
    [Header("第六层固定节点")]
    [SerializeField] private GameObject _sixthFixedNode;
    [Header("第七层随机节点容器")]
    [SerializeField] private List<GameObject> _sevenRandomNodeContainers = new List<GameObject>();
    [Header("Boss节点")]
    [SerializeField] private GameObject _bossNode;
    [Header("可随机的节点类型列表")]
    [SerializeField] private List<E_TowerNodeType> _randomNodeTypes = new List<E_TowerNodeType>();

    // 原有字典完全保留
    private Dictionary<TowerNodeGroup, List<GameObject>> _allNodes = new Dictionary<TowerNodeGroup, List<GameObject>>();
    // 新增：节点管理字典，用于初始化注册
    private Dictionary<string, BaseNodeItem> _nodeItemDic = new Dictionary<string, BaseNodeItem>();

    #region 生命周期（新增事件监听与按钮绑定）
    protected override void Awake()
    {
        base.Awake();
        // 绑定启程按钮点击事件
        //departBtn?.onClick.AddListener(OnDepartBtnClick);
        // 初始启程按钮置灰
        //departBtn.interactable = false;
        //监听爬塔界面节点生成
        EventCenter.Instance.AddEventListener(E_EventType.Tower_Bron, GenerateTowerRandomNodes);
        // 监听启程按钮状态变更事件
        //EventCenter.Instance.AddEventListener<bool>(E_EventType.UI_DepartBtnStateChanged, OnDepartBtnStateChanged);
        // 监听爬塔初始化完成事件
        EventCenter.Instance.AddEventListener(E_EventType.Tower_InitComplete, OnInitComplete);
    }

    private void OnDestroy()
    {
        // 移除事件监听
        //departBtn?.onClick.RemoveAllListeners();
        //EventCenter.Instance.RemoveEventListener<bool>(E_EventType.UI_DepartBtnStateChanged, OnDepartBtnStateChanged);
        EventCenter.Instance.RemoveEventListener(E_EventType.Tower_InitComplete, OnInitComplete);
        EventCenter.Instance.RemoveEventListener(E_EventType.Tower_Bron, GenerateTowerRandomNodes);
        // 清空面板
        ClearTowerPanel();
    }
    #endregion



    #region 原有节点生成逻辑（一行未改，完全保留）
    /// <summary>
    /// 生成所有楼层的随机节点（原有逻辑100%保留）
    /// </summary>
    public void GenerateTowerRandomNodes()
    {
        ClearTowerPanel();


        // 生成第一层起始节点（新增初始化注册）
        GenerateFixedNode(_startNode, "Layer1_Start", E_TowerNodeType.NormalBattle, 1);

        // 生成第二层节点（3~4个随机节点 + 1个固定节点）
        var twoNodes = GenerateRandomNodes(_twoRandomNodeContainers, Random.Range(3, 5), 2);
        // 生成第二层固定节点（新增初始化注册）
        var secondFixedNodeObj = GenerateFixedNode(_secondFixedNode, "Layer2_Fixed", E_TowerNodeType.Camp, 2);
        twoNodes.Add(secondFixedNodeObj);
        _allNodes.Add(TowerNodeGroup.TwoNodes, twoNodes);

        // 生成第三层节点（2~3个随机节点 + 1个精英节点）
        var threeNodes = GenerateRandomNodes(_threeRandomNodeContainers, Random.Range(2, 4), 3);
        // 生成第三层精英节点（新增初始化注册）
        var eliteNodeObj = GenerateFixedNode(_eliteNode, "Layer3_Elite", E_TowerNodeType.EliteBattle, 3);
        threeNodes.Add(eliteNodeObj);
        _allNodes.Add(TowerNodeGroup.ThreeNodes, threeNodes);

        // 生成第四层节点（2~4个随机节点）
        var fourNodes = GenerateRandomNodes(_fourRandomNodeContainers, Random.Range(2, 5), 4);
        _allNodes.Add(TowerNodeGroup.FourNodes, fourNodes);

        // 生成第五层节点（2~4个随机节点）
        var fiveNodes = GenerateRandomNodes(_fiveRandomNodeContainers, Random.Range(2, 5), 5);
        _allNodes.Add(TowerNodeGroup.FiveNodes, fiveNodes);

        // 生成第六层节点（2~4个随机节点 + 1个固定节点）
        var sixNodes = GenerateRandomNodes(_sixRandomNodeContainers, Random.Range(2, 5), 6);
        // 生成第六层固定节点（新增初始化注册）
        var sixthFixedNodeObj = GenerateFixedNode(_sixthFixedNode, "Layer6_Fixed", E_TowerNodeType.Camp, 6);
        sixNodes.Add(sixthFixedNodeObj);
        _allNodes.Add(TowerNodeGroup.SixNodes, sixNodes);

        // 生成第七层节点（2~4个随机节点）
        var sevenNodes = GenerateRandomNodes(_sevenRandomNodeContainers, Random.Range(2, 5), 7);
        _allNodes.Add(TowerNodeGroup.SevenNodes, sevenNodes);

        // 生成BOSS节点（新增初始化注册）
        GenerateFixedNode(_bossNode, "Layer8_Boss", E_TowerNodeType.BossBattle, 8);


    }

    /// <summary>
    /// 通用随机节点生成方法（原有逻辑100%保留，仅补充节点初始化注册）
    /// </summary>
    private List<GameObject> GenerateRandomNodes(List<GameObject> containers, int count, int layerIndex)
    {
        List<GameObject> generatedNodes = new List<GameObject>();
        count = Mathf.Min(count, containers.Count);
        for (int i = 0; i < count; i++)
        {
            //随机逻辑
            E_TowerNodeType nodeType = GetRandomNodeType();
            GameObject nodeObj = Instantiate(
                Resources.Load<GameObject>($"{NODE_ITEM_RESOURCE_PATH}{nodeType}"),
                containers[i].transform
            );
            nodeObj.name = $"Layer{layerIndex}_Random_{i + 1}";

            //节点初始化与注册
            BaseNodeItem nodeItem = nodeObj.GetComponent<BaseNodeItem>();
            if (nodeItem != null)
            {
                string nodeId = nodeObj.name;
                nodeItem.Init(nodeId, nodeType, layerIndex);
                LevelFlowMgr.Instance.RegisterNode(nodeId, nodeType, layerIndex);
                _nodeItemDic.Add(nodeId, nodeItem);
            }


            generatedNodes.Add(nodeObj);
        }
        return generatedNodes;
    }

    /// <summary>
    /// 生成固定节点（新增方法，不影响原有逻辑）
    /// </summary>
    private GameObject GenerateFixedNode(GameObject parent, string nodeId, E_TowerNodeType nodeType, int layerIndex)
    {
        // 清空原有子节点
        if (parent.transform.childCount > 0)
        {
            DestroyImmediate(parent.transform.GetChild(0).gameObject);
        }

        // 实例化节点
        GameObject nodeObj = Instantiate(
            Resources.Load<GameObject>($"{NODE_ITEM_RESOURCE_PATH}{nodeType}"),
            parent.transform
        );
        nodeObj.name = nodeId;

        // 初始化与注册
        BaseNodeItem nodeItem = nodeObj.GetComponent<BaseNodeItem>();
        if (nodeItem != null)
        {
            nodeItem.Init(nodeId, nodeType, layerIndex);
            LevelFlowMgr.Instance.RegisterNode(nodeId, nodeType, layerIndex);
            _nodeItemDic.Add(nodeId, nodeItem);
        }

        return nodeObj;
    }

    /// <summary>
    /// 获取随机节点类型（原有逻辑完全保留）
    /// </summary>
    private E_TowerNodeType GetRandomNodeType()
    {
        int randomIndex = Random.Range(0, Mathf.Min(RANDOM_NODE_TYPE_COUNT, _randomNodeTypes.Count));
        return _randomNodeTypes[randomIndex];
    }

    /// <summary>
    /// 清空楼层面板（原有逻辑完全保留，补充字典清空）
    /// </summary>
    public void ClearTowerPanel()
    {
        foreach (var nodeGroup in _allNodes.Values)
        {
            foreach (var node in nodeGroup)
            {
                if (node != null) Destroy(node);
            }
        }
        // 清空固定节点
        if (_startNode.transform.childCount > 0) DestroyImmediate(_startNode.transform.GetChild(0).gameObject);
        if (_secondFixedNode.transform.childCount > 0) DestroyImmediate(_secondFixedNode.transform.GetChild(0).gameObject);
        if (_eliteNode.transform.childCount > 0) DestroyImmediate(_eliteNode.transform.GetChild(0).gameObject);
        if (_sixthFixedNode.transform.childCount > 0) DestroyImmediate(_sixthFixedNode.transform.GetChild(0).gameObject);
        if (_bossNode.transform.childCount > 0) DestroyImmediate(_bossNode.transform.GetChild(0).gameObject);

        _allNodes.Clear();
        _nodeItemDic.Clear();
    }
    #endregion

    #region 新增：启程按钮与事件回调逻辑
    /// <summary>
    /// 启程按钮点击事件
    /// </summary>
    private void OnDepartBtnClick()
    {
        LevelFlowMgr.Instance.OnDepartBtnClick();
    }

    /// <summary>
    /// 启程按钮状态变更回调
    /// </summary>
    //private void OnDepartBtnStateChanged(bool isInteractable)
    //{
    //    if (departBtn != null)
    //    {
    //        departBtn.interactable = isInteractable;
    //    }
    //}

    /// <summary>
    /// 爬塔初始化完成回调
    /// </summary>
    private void OnInitComplete()
    {
        // 刷新所有节点状态
        foreach (var nodeItem in _nodeItemDic.Values)
        {
            nodeItem.ForceUpdateState();
        }
    }
    #endregion

    #region 原有面板生命周期方法
    public override void ShowMe() => gameObject.SetActive(true);
    public override void HideMe() => gameObject.SetActive(false);
    #endregion
}