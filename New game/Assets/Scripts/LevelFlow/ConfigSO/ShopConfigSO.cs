using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopConfig", menuName = "游戏配置/商店配置")]
public class ShopConfigSO : ScriptableObject
{
    public int refreshCostFirstFree = 0;   // 第一次免费
    public int refreshCost = 15;           // 后续刷新消耗15铜
    public List<ShopSlotConfig> slots = new List<ShopSlotConfig>();
}

[System.Serializable]
public class ShopSlotConfig
{
    public E_ShopItemType type;
    public int count;                      // 每次刷出几个（如白色奇物3个）
    public int price;                      // 固定价格，也可配置
    public bool isRandomFromPool = true;   // 是否从奇物/典籍池随机
}
