
using UnityEngine;

[System.Serializable]
public abstract class BaseBook 
{
    public int extraAtk;
    public int extraWide;
    public int extraHigh;

    public abstract E_BookType BookType { get; }

    /// <summary>
    /// 合成卡牌时触发（主要用于解锁三字牌）
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnComposite(BaseCard card)
    {
        Debug.Log(BookType + $"[典籍强化]卡牌{card.cardID}原始攻击{card.currentAtk}原始宽{card.currentRecRangeWide}原始高{card.currentRecRangeHigh}");
        card.currentAtk += extraAtk;
        card.currentRecRangeWide += extraWide;
        card.currentRecRangeHigh += extraHigh;
        Debug.Log(BookType + $"[典籍强化]卡牌{card.cardID}强化后攻击{card.currentAtk}强化后宽{card.currentRecRangeWide}强化后高{card.currentRecRangeHigh}");

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


    public virtual void OnPrevSlected(BaseCardScriptableData data)
    {
        Debug.Log(BookType + $"[典籍预强化]卡牌{data.cardID}原始攻击{data.baseAtk}原始宽{data.baseRecRangeWide}原始高{data.baseRecRangeHigh}");
        data.isFirstActive = true;
        data.baseAtk += extraAtk;
        data.baseRecRangeWide += extraWide;
        data.baseRecRangeHigh += extraHigh;
        Debug.Log(BookType + $"[典籍预强化]卡牌{data.cardID}强化后攻击{data.baseAtk}强化后宽{data.baseRecRangeWide}强化后高{data.baseRecRangeHigh}");
    }
}
