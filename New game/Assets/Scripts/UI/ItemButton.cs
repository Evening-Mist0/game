using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 选择按钮的核心逻辑：绑定物品数据、点击切换选中状态、控制高亮显示
public class ItemButton : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [Header("UI组件关联（拖拽赋值）")] // 序列化字段，可在Inspector绑定组件
    public Image iconImage;      // 物品图标显示组件
    public Text nameText;        // 物品名称显示组件
    public Image highlightMask;  // 选中时的高亮遮罩（核心：保持高亮的视觉元素）

    [Header("调试用（无需赋值）")]
    [SerializeField] private bool _isSelected = false; // 选中状态（持久存储，核心变量）
    private ItemData _bindedItem; // 当前按钮绑定的物品数据
    private SelectionPanelManager _panelManager; // 面板管理器引用（用于通知状态变化）
    private RectTransform _btnRect;

    // 初始化按钮：绑定物品数据和面板管理器（由面板管理器调用）
    public void Init(ItemData itemData, SelectionPanelManager panelManager)
    {
        // 赋值核心引用
        _bindedItem = itemData;
        _panelManager = panelManager;
        // 初始化为未选中状态
        _isSelected = false;
        // 更新视觉（初始隐藏高亮）
        _btnRect = GetComponent<RectTransform>();
        UpdateHighlightVisual();

        // 绑定按钮点击事件：点击时切换选中状态
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 空值保护（保留）
        if (TooltipManager.Instance == null)
        {
            Debug.LogError("TooltipManager单例为空！");
            return;
        }
        if (_bindedItem == null || string.IsNullOrEmpty(_bindedItem.itemDesc))
        {
            Debug.LogWarning("物品描述为空或数据异常！");
            return;
        }
        if (_btnRect == null)
        {
            _btnRect = GetComponent<RectTransform>();
        }

        // ========== 核心修复：正确转换UI坐标→屏幕坐标 ==========
        Vector2 btnScreenPos;
        // Canvas为Screen Space - Overlay时，第二个参数传null（关键！）
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _btnRect.parent.GetComponent<RectTransform>(), // 父物体RectTransform（ItemButton的父面板）
            eventData.position, // 鼠标的屏幕坐标（而非按钮世界坐标）
            null, // Overlay模式传null
            out btnScreenPos
        );
        // 转换为面板的锚点坐标（Top Left）
        Vector2 finalScreenPos = _btnRect.parent.GetComponent<RectTransform>().TransformPoint(btnScreenPos);
        // 简化写法（直接用鼠标屏幕坐标，更精准）：
        // Vector2 finalScreenPos = Input.mousePosition;

        // 打印正确的坐标日志（验证）
        Debug.Log($"正确的鼠标屏幕坐标：{Input.mousePosition} | 按钮屏幕坐标：{finalScreenPos} | 屏幕尺寸：{Screen.width}x{Screen.height}");

        // 调用显示面板
        TooltipManager.Instance.ShowTooltip(_bindedItem.itemDesc, finalScreenPos);
    }

    // ========== 新增：鼠标离开事件（UGUI接口） ==========
    /// <summary>
    /// 鼠标离开按钮时触发
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("鼠标离开按钮！");
        if (TooltipManager.Instance != null)
        {
            // 隐藏描述面板
            TooltipManager.Instance.HideTooltip();
        }
    }
    // 按钮点击事件：核心逻辑——切换自身选中状态，并通知管理器
    private void OnButtonClick()
    {
        // 切换选中状态（true→false / false→true）
        _isSelected = !_isSelected;
        // 更新高亮视觉（选中显示遮罩，取消隐藏）
        UpdateHighlightVisual();
        // 通知面板管理器：当前按钮状态变化，更新选中列表
        _panelManager.OnItemButtonToggled(this);
    }

    // 更新高亮视觉效果（持久化，选中就显示，取消就隐藏）
    private void UpdateHighlightVisual()
    {
        // 方式1：显示/隐藏高亮遮罩（推荐，视觉更灵活）
        highlightMask.enabled = _isSelected;

        // 方式2（备选）：切换按钮底色（无需额外遮罩）
        // Image btnBg = GetComponent<Image>();
        // btnBg.color = _isSelected ? new Color(1f, 0.9f, 0.2f) : Color.white;
    }

    // 外部强制设置选中状态（用于清空所有选中时）
    public void SetSelected(bool isSelected)
    {
        _isSelected = isSelected;
        UpdateHighlightVisual();
    }

    // 获取当前按钮的选中状态（供管理器判断）
    public bool IsSelected()
    {
        return _isSelected;
    }

    // 获取当前按钮绑定的物品数据（供管理器添加背包）
    public ItemData GetBindedItem()
    {
        return _bindedItem;
    }

    // 销毁时移除事件监听（防止内存泄漏）
    private void OnDestroy()
    {
        GetComponent<Button>().onClick.RemoveListener(OnButtonClick);
    }
}