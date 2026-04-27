using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BooksViewControl : MonoBehaviour
{
    List<BookIconControl> books = new List<BookIconControl>();

    public RectTransform father;

    /// <summary>
    /// 每次开始游戏时调用，遍历玩家背包中的奇物，实例化在面板上
    /// </summary>
    public void Refresh()
    {
        RemoveTreasureIcons();

        foreach(BaseBook book in GamePlayer.Instance.playerBag.books.Values)
        {
            CreateBookIcon(book);

        }
    }

    private void CreateBookIcon(BaseBook book)
    {
        if (book == null)
        {
            Debug.LogError("从玩家背包里找到的books字典对应的值为空");
            return;
        }

        string path = books.GetType().Name;
        GameObject obj = Instantiate(Resources.Load<GameObject>("BookIcon/BookIcon_" + book.BookType.ToString()));

        if (obj == null)
        {
            Debug.LogError("加载 典籍图标 失败：" + path);
            return;
        }
        BookIconControl icon = obj.GetComponent<BookIconControl>();
        if (icon == null)
        {
            Debug.LogError("prefab 缺少 BookIconControl 组件");
            Destroy(obj);
            return;
        }
        if (icon == null) return;

        AddTreasureIcon(icon);
    
        icon.gameObject.transform.SetParent(father, false);
    }

    private void AddTreasureIcon(BookIconControl icon)
    {
        if (!books.Contains(icon))
            books.Add(icon);
    }

    private void RemoveTreasureIcon(BookIconControl icon)
    {
        if (books.Contains(icon))
            books.Remove(icon);
    }

    private void RemoveTreasureIcons()
    {
        List<BookIconControl> tempList = books;
        for (int i = 0; i < tempList.Count; i++)
        {
            if (tempList[i] != null && tempList[i].gameObject != null)
                Destroy(tempList[i].gameObject);

        }
        books.Clear();
    }

}
