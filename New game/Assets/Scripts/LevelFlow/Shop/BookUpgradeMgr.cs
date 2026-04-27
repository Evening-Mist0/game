using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookUpgradeMgr : BaseMgr<BookUpgradeMgr>
{
    private BookUpgradeMgr() { }

    public bool CanUpgrade(E_BookType bookType)
    {
        int currentLevel = GetUpgradeLevel(bookType);
        return currentLevel <= 2; // 最多升级2次
    }

    public int GetUpgradeLevel(E_BookType bookType)
    {
        BaseBook book = GamePlayer.Instance.playerBag.GetBook(bookType);
        if(book != null)
        {
            return book.currentLevel;
        }

        return 0;
    }

    public void UpgradeBook(E_BookType bookType)
    {
        if (!CanUpgrade(bookType)) return;
        int newLevel = GetUpgradeLevel(bookType) + 1;
        BaseBook book = GamePlayer.Instance.playerBag.GetBook(bookType);
        if(book != null)
        {
            book.LevelUp(newLevel);
            EventCenter.Instance.EventTrigger(E_EventType.Book_Upgraded, (bookType, newLevel));
        }
    }
}
