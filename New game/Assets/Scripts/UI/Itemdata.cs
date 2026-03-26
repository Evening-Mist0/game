using System;
using UnityEngine;

// 物品数据结构：存储按钮需要展示的核心信息（可序列化，支持在Inspector配置）
[Serializable] // 允许在Inspector面板编辑该类的实例
public class ItemData
{
    public int itemId;      // 物品唯一ID（用于区分不同物品）
    public string itemName; // 物品名称（显示在按钮上）
    public Sprite itemIcon; // 物品图标（显示在按钮上）
    public string itemDesc; 
    // 可扩展：物品类型、描述、数量等，根据需求添加
}
