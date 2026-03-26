using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 选择面板管理器：控制面板显示/隐藏、动态生成按钮、维护多选状态、处理确定逻辑
public class SelectionPanelManager : MonoBehaviour
{
    [Header("面板核心组件（拖拽赋值）")]
    public GameObject selectionPanel;       // 选择面板根物体（控制显示/隐藏）
    public Transform buttonContainer;       // 动态按钮的父容器（ButtonContainer）
    public Button confirmButton;            // 确定按钮
    public GameObject itemButtonPrefab;     // ItemButton预制体（拖拽赋值）

    [Header("内部状态（无需赋值）")]
    private List<ItemButton> _allSpawnedButtons = new(); // 存储所有生成的按钮（用于清空）
    private List<ItemButton> _selectedButtons = new();   // 存储所有选中的按钮（多选核心）
    private PlayerInventory _playerInventory;            // 玩家背包引用

    // 初始化：获取背包、绑定事件、初始隐藏面板
    private void Awake()
    {
        // 查找/创建背包组件（全局唯一）
        _playerInventory = FindAnyObjectByType<PlayerInventory>();
        if (_playerInventory == null)
        {
            Debug.LogWarning("未找到背包组件，自动创建全局背包！");
            _playerInventory = new GameObject("PlayerInventory").AddComponent<PlayerInventory>();
        }

        // 初始隐藏面板
        HidePanel();

        // 绑定确定按钮点击事件
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        // 初始禁用确定按钮（无选中时不可点击）
        SetConfirmButtonState(false);
    }

    // 对外提供：显示面板并加载指定物品数据（复用核心）
    public void ShowPanel(List<ItemData> itemDatas)
    {
        // 清空旧按钮和选中状态（避免复用残留）
        ClearAllButtons();
        // 遍历物品数据，动态生成对应按钮
        foreach (ItemData data in itemDatas)
        {
            SpawnSingleItemButton(data);
        }
        // 显示面板
        selectionPanel.SetActive(true);
    }

    // 对外提供：隐藏面板（复用核心，只隐藏不销毁）
    public void HidePanel()
    {
        selectionPanel.SetActive(false);
        // 清空选中状态（下次打开面板时无残留）
        ClearSelectedState();
        // 禁用确定按钮
        SetConfirmButtonState(false);
    }

    // 动态生成单个物品按钮
    private void SpawnSingleItemButton(ItemData itemData)
    {
        // 校验预制体是否赋值
        if (itemButtonPrefab == null)
        {
            Debug.LogError("ItemButton预制体未赋值！请在Inspector中拖拽指定");
            return;
        }

        // 实例化按钮并挂载到容器下
        GameObject btnObj = Instantiate(itemButtonPrefab, buttonContainer);
        // 获取按钮的核心逻辑组件
        ItemButton itemBtn = btnObj.GetComponent<ItemButton>();
        if (itemBtn == null)
        {
            Debug.LogError("ItemButton预制体缺少ItemButton脚本！");
            Destroy(btnObj); // 销毁无效按钮
            return;
        }

        // 初始化按钮（绑定数据和管理器）
        itemBtn.Init(itemData, this);
        // 将按钮加入“所有生成的按钮”列表
        _allSpawnedButtons.Add(itemBtn);
    }

    // 按钮状态切换时的回调（由ItemButton调用）
    public void OnItemButtonToggled(ItemButton toggledBtn)
    {
        // 选中状态：加入选中列表（避免重复添加）
        if (toggledBtn.IsSelected())
        {
            if (!_selectedButtons.Contains(toggledBtn))
            {
                _selectedButtons.Add(toggledBtn);
            }
        }
        // 取消选中：从选中列表移除
        else
        {
            if (_selectedButtons.Contains(toggledBtn))
            {
                _selectedButtons.Remove(toggledBtn);
            }
        }

        // 更新确定按钮状态：有选中则启用，无则禁用
        SetConfirmButtonState(_selectedButtons.Count > 0);
    }

    // 确定按钮点击逻辑：批量添加选中物品到背包
    private void OnConfirmButtonClick()
    {
        // 校验：无选中则直接返回
        if (_selectedButtons.Count == 0) return;

        // 遍历所有选中的按钮，将物品加入背包
        foreach (ItemButton btn in _selectedButtons)
        {
            ItemData selectedItem = btn.GetBindedItem();
            _playerInventory.AddItem(selectedItem);
            Debug.Log($"【背包】{selectedItem.itemName} 已添加！当前背包物品数：{_playerInventory.GetItemCount()}");
        }

        // 关闭面板（自动清空选中状态）
        HidePanel();
    }

    // 清空所有生成的按钮（面板复用/关闭时调用）
    private void ClearAllButtons()
    {
        // 先清空选中状态
        ClearSelectedState();
        // 销毁所有生成的按钮
        foreach (ItemButton btn in _allSpawnedButtons)
        {
            Destroy(btn.gameObject);
        }
        // 清空列表
        _allSpawnedButtons.Clear();
    }

    // 清空所有按钮的选中状态（不销毁按钮）
    private void ClearSelectedState()
    {
        // 遍历选中列表，强制设置为未选中
        foreach (ItemButton btn in _selectedButtons)
        {
            btn.SetSelected(false);
        }
        // 清空选中列表
        _selectedButtons.Clear();
    }

    // 设置确定按钮的可点击状态
    private void SetConfirmButtonState(bool isInteractable)
    {
        confirmButton.interactable = isInteractable;
        // 可选：修改按钮颜色提示状态（禁用时灰色，启用时正常）
        // Image btnImage = confirmButton.GetComponent<Image>();
        // btnImage.color = isInteractable ? Color.white : new Color(0.7f, 0.7f, 0.7f);
    }
}