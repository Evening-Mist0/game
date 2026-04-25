using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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
            for (int i = 0; i < slot.count; i++)
            {
                ShopItem item = null;
                switch (slot.type)
                {
                    case E_ShopItemType.WhiteRelic:
                        item = GenerateRelicItem(E_RelicQuality.White, slot.price);
                        if (item != null) data.whiteRelics.Add(item);
                        break;
                    case E_ShopItemType.GreenRelic:
                        item = GenerateRelicItem(E_RelicQuality.Green, slot.price);
                        if (item != null) data.greenRelics.Add(item);
                        break;
                    case E_ShopItemType.BlueRelic:
                        item = GenerateRelicItem(E_RelicQuality.Blue, slot.price);
                        if (item != null) data.blueRelics.Add(item);
                        break;
                    case E_ShopItemType.Book:
                        item = GenerateBookItem(slot.price);
                        if (item != null) data.books.Add(item);
                        break;
                    case E_ShopItemType.BookUpgrade:
                        item = GenerateUpgradeItem(slot.price);
                        if (item != null) data.upgrades.Add(item);
                        break;
                }
            }
        }
        return data;
    }

    private ShopItem GenerateRelicItem(E_RelicQuality quality, int price)
    {
        var ownedIds = GrowthMgr.Instance.growthData.ownedRelicIds;
        var candidates = relicConfig.relicConfigs
            .Where(r => r.quality == quality && !ownedIds.Contains(r.relicId))
            .ToList();
        if (candidates.Count == 0) return null;
        var relic = candidates[Random.Range(0, candidates.Count)];
        return new ShopItem
        {
            itemId = relic.relicId,
            type = MapQualityToShopType(quality),
            price = price,
            name = relic.relicName,
            icon = relic.relicIcon,
            description = relic.relicDesc,
            isSold = false
        };
    }

    private ShopItem GenerateBookItem(int price)
    {
        var unowned = GrowthMgr.Instance.GetRandomUnownedBooks(1);
        if (unowned.Count == 0) return null;
        var book = unowned[0];
        return new ShopItem
        {
            itemId = book.bookId.ToString(),
            type = E_ShopItemType.Book,
            price = price,
            name = book.bookName,
            icon = book.bookIcon,
            description = book.bookDesc,
            isSold = false
        };
    }

     private ShopItem GenerateUpgradeItem(int price)
     {
         // 获取所有可升级（未满级）且已拥有的典籍
         var upgradable = GrowthMgr.Instance.growthData.ownedBooks
             .Where(bookType => BookUpgradeMgr.Instance.CanUpgrade(bookType))
             .Select(bookType => GrowthMgr.Instance.GetBookConfig(bookType))
             .Where(cfg => cfg != null)
             .ToList();
         if (upgradable.Count == 0) return null;
         var book = upgradable[Random.Range(0, upgradable.Count)];
         return new ShopItem
         {
             itemId = book.bookId.ToString(),
             type = E_ShopItemType.BookUpgrade,
             price = price,
             name = $"升级《{book.bookName}》",
             icon = book.bookIcon,
             description = "提升典籍效果",
             isSold = false
         };
     }

    private E_ShopItemType MapQualityToShopType(E_RelicQuality quality)
    {
        switch (quality)
        {
            case E_RelicQuality.White: 
              return E_ShopItemType.WhiteRelic;
            case E_RelicQuality.Green: 
              return E_ShopItemType.GreenRelic;
            case E_RelicQuality.Blue: 
              return E_ShopItemType.BlueRelic;
            default: return E_ShopItemType.WhiteRelic;
        }
    }
}
