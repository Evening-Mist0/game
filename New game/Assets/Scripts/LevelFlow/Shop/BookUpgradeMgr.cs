using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookUpgradeMgr : BaseMgr<BookUpgradeMgr>
{
    private BookUpgradeMgr() { }

    public bool CanUpgrade(E_BookType bookType)
    {
        int currentLevel = GetUpgradeLevel(bookType);
        return currentLevel < 2; // 最多升级2次
    }

    public int GetUpgradeLevel(E_BookType bookType)
    {
        // if (GrowthMgr.Instance.growthData.bookUpgradeLevels.TryGetValue(bookType, out int level))
        //     return level;
        return 0;
    }

    public void UpgradeBook(E_BookType bookType)
    {
        // if (!CanUpgrade(bookType)) return;
        // int newLevel = GetUpgradeLevel(bookType) + 1;
        // GrowthMgr.Instance.growthData.bookUpgradeLevels[bookType] = newLevel;
        // // 触发事件，更新卡牌效果
        // EventCenter.Instance.EventTrigger(E_EventType.Book_Upgraded, (bookType, newLevel));
    }

    public void UpgradeBook(string bookTypeName)
    {
        E_BookType bookType = (E_BookType)System.Enum.Parse(typeof(E_BookType), bookTypeName);
        UpgradeBook(bookType);
    }
}
