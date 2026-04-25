
using UnityEngine;

/// <summary>
/// 典籍种类
/// </summary>
public enum E_BookShape
{
    /// <summary>
    /// 放置防御塔的典籍
    /// </summary>
    Tower,
    /// <summary>
    /// 效果类的典籍
    /// </summary>
    Effect,
}
[System.Serializable]
public abstract class BaseBook 
{
    public abstract E_BookType BookType { get; }

    public abstract E_BookShape E_BookShape { get; }

    public int currentLevel = 1;

 

    /// <summary>
    /// 合成卡牌时触发（主要用于解锁三字牌）
    /// </summary>
    /// <param name="card"></param>
    public virtual void BookOnCreateNewCard(BaseCard card)
    {
        //不是典籍卡牌不进行升级判定
        if (card.bookType == E_BookType.None)
            return;   
    }

    /// <summary>
    /// 创建出新的建筑物时，根据典籍得到的建筑物数据，进行升级
    /// </summary>
    /// <param name="tower"></param>
    public virtual void BookOnCreateNewDefTower(BaseDefTower tower)
    {
        //不是典籍卡牌不进行升级判定
        if (tower.bookType == E_BookType.None)
            return;
    }

    /// <summary>
    /// 抽牌时触发
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnDrawCard(BaseCard card)
    {

    }


    /// <summary>
    /// 卡牌打出时触发
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnPlay(BaseCard card)
    {

    }

    /// <summary>
    /// 卡牌生成建筑物时触发
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnPlaceDefTower(BaseCard card)
    {

    }

    /// <summary>
    /// 当鼠标进入播放完卡牌弹出动画时触发（用于与合成显示）
    /// </summary>
    /// <param name="data"></param>

    public virtual void OnPrevSlected(BaseCardScriptableData data)
    {
        //不是典籍卡牌不进行升级判定
        if (data.bookType == E_BookType.None)
            return;
    }

    /// <summary>
    /// 典籍升级
    /// </summary>
    /// <param name="level">升级后的等级</param>
    public void LevelUp(int level)
    {
        if (level < 1 || level > 3)
        {
            Debug.LogWarning("传入非法等级，等级只能为1，2，3");
            return;

        }
        currentLevel = level;
    }

}
