using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_ShopItemType
{
    WhiteRelic,   // 白色奇物
    GreenRelic,   // 绿色奇物
    BlueRelic,    // 蓝色奇物
    Book,         // 典籍（未获得的）
    BookUpgrade   // 典籍升级（已拥有的）
}

[System.Serializable]
public class ShopItem
{
    public string itemId;           // 奇物ID或典籍枚举名
    public E_ShopItemType type;
    public int price;
    public bool isSold;             // 是否已售出
    // 额外字段（用于显示）
    public string name;
    public Sprite icon;
    public string description;
    public string BookLeveldescription;
}
