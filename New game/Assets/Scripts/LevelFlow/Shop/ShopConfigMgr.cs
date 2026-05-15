using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class ShopAreaData
{
    public List<ShopItem> whiteRelics = new List<ShopItem>();
    public List<ShopItem> greenRelics = new List<ShopItem>();
    public List<ShopItem> blueRelics = new List<ShopItem>();
    public List<ShopItem> books = new List<ShopItem>();
    public List<ShopItem> upgrades = new List<ShopItem>();
}

[System.Serializable]
public class ShopConfigMgr : BaseMgr<ShopConfigMgr>
{
    private ShopConfigSO config;
    private RelicConfigSO relicConfig;
    private BookConfigSO bookConfig;

    private ShopConfigMgr()
    {
        config = Resources.Load<ShopConfigSO>("Config/ShopConfig");
        relicConfig = Resources.Load<RelicConfigSO>("Config/RelicConfig");
        bookConfig = Resources.Load<BookConfigSO>("Config/BookConfig");
    }

    public int GetRefreshCost(bool isFirstRefresh) => isFirstRefresh ? config.refreshCostFirstFree : config.refreshCost;

    public ShopAreaData GenerateShopItems()
{
    ShopAreaData data = new ShopAreaData();

    foreach (var slot in config.slots)
    {
        switch (slot.type)
        {
            case E_ShopItemType.WhiteRelic:
                data.whiteRelics.AddRange(GenerateRelicItems(E_RelicQuality.White, slot.count, slot.price));
                break;
            case E_ShopItemType.GreenRelic:
                data.greenRelics.AddRange(GenerateRelicItems(E_RelicQuality.Green, slot.count, slot.price));
                break;
            case E_ShopItemType.BlueRelic:
                data.blueRelics.AddRange(GenerateRelicItems(E_RelicQuality.Blue, slot.count, slot.price));
                break;
            case E_ShopItemType.Book:
                data.books.AddRange(GenerateBookItems(slot.count, slot.price));
                break;
            case E_ShopItemType.BookUpgrade:
                data.upgrades.AddRange(GenerateUpgradeItems(slot.count, slot.price));
                break;
        }
    }
    return data;
}

private List<ShopItem> GenerateRelicItems(E_RelicQuality quality, int desiredCount, int price)
{
    List<ShopItem> result = new List<ShopItem>();
    var ownedIds = GrowthMgr.Instance.growthData.ownedRelicIds;
    var candidates = relicConfig.relicConfigs
        .Where(r => r.quality == quality && !ownedIds.Contains(r.relicId))
        .OrderBy(x => Guid.NewGuid())   // 随机打乱
        .ToList();
        

    int take = Mathf.Min(desiredCount, candidates.Count);
    for (int i = 0; i < take; i++)
    {
        var relic = candidates[i];
        result.Add(new ShopItem
        {
            itemId = relic.relicId,
            type = MapQualityToShopType(quality),
            price = price,
            name = relic.relicName,
            icon = relic.relicIcon,
            description = relic.relicDesc,
            isSold = false
        });
    }
    // 如果数量不足，可根据需要添加铜钱补位（略）
    return result;
}

private List<ShopItem> GenerateBookItems(int desiredCount, int price)
{
    List<ShopItem> result = new List<ShopItem>();
    var unowned = GrowthMgr.Instance.GetRandomUnownedBooks(desiredCount); // 此方法已保证不重复
    foreach (var book in unowned)
    {
        result.Add(new ShopItem
        {
            itemId = book.bookId.ToString(),
            type = E_ShopItemType.Book,
            price = price,
            name = book.bookName,
            icon = book.bookIcon,
            description = book.baseDesc,
            isSold = false
        });
    }
    return result;
}

    public List<ShopItem> GenerateUpgradeItems(int desiredCount, int basePrice)
    {
        List<ShopItem> result = new List<ShopItem>();
        var upgradable = GrowthMgr.Instance.growthData.ownedBooks
            .Where(bookType => BookUpgradeMgr.Instance.CanUpgrade(bookType))
            .Select(bookType => new 
            {
                BookType = bookType,
                Config = GrowthMgr.Instance.GetBookConfig(bookType),
                CurrentLevel = BookUpgradeMgr.Instance.GetUpgradeLevel(bookType)
            })
            .Where(x => x.Config != null)
            .ToList();

    int take = Mathf.Min(desiredCount, upgradable.Count);
    for (int i = 0; i < take; i++)
    {
        var item = upgradable[i];
        int nextLevel = item.CurrentLevel + 1;
        string currentDesc = item.Config.GetDescription(item.CurrentLevel);
        string upgradeDesc = item.Config.GetDescription(nextLevel);
        int upgradePrice = item.CurrentLevel == 1 ? 35 : 70;
        result.Add(new ShopItem
        {
            itemId = item.BookType.ToString(),
            type = E_ShopItemType.BookUpgrade,
            price = upgradePrice,
            name = $"升级《{item.Config.bookName}》",
            icon = item.Config.bookIcon,
            description = $"当前效果：{currentDesc}",
            BookLeveldescription = $"升级后效果：{upgradeDesc}",
            isSold = false
        });
    }
    return result;
    }

    private E_ShopItemType MapQualityToShopType(E_RelicQuality quality)
    {
        switch (quality)
        {
            case E_RelicQuality.White: return E_ShopItemType.WhiteRelic;
            case E_RelicQuality.Green: return E_ShopItemType.GreenRelic;
            case E_RelicQuality.Blue: return E_ShopItemType.BlueRelic;
            default: return E_ShopItemType.WhiteRelic;
        }
    }

    public int GetUpgradeSlotCount()
    {
        var slot = config.slots.FirstOrDefault(s => s.type == E_ShopItemType.BookUpgrade);
        return slot != null ? slot.count : 1;
    }

    public int GetUpgradeBasePrice()
    {
        var slot = config.slots.FirstOrDefault(s => s.type == E_ShopItemType.BookUpgrade);
        return slot != null ? slot.price : 35;
    }    
}
