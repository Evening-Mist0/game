using System.Collections.Generic;
using UnityEngine;

// 玩家背包组件：存储物品、提供添加/查询接口
public class PlayerInventory : MonoBehaviour
{
    // 背包物品列表（可在Inspector查看）
    [SerializeField] private List<ItemData> _inventoryItems = new();

    // 添加物品到背包（对外提供）
    public void AddItem(ItemData item)
    {
        // 校验：物品为空则报错
        if (item == null)
        {
            Debug.LogError("尝试添加空物品到背包！");
            return;
        }
        // 添加物品（可扩展：物品叠加、容量限制、重复检测）
        _inventoryItems.Add(item);
    }

    // 获取背包物品总数（对外提供）
    public int GetItemCount()
    {
        return _inventoryItems.Count;
    }

    // 可选：清空背包（测试用）
    public void ClearInventory()
    {
        _inventoryItems.Clear();
        Debug.Log("【背包】已清空！");
    }
}