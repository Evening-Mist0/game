

[System.Serializable]
public abstract class BaseBook 
{

    public abstract E_BookType BookType { get; }

    /// <summary>
    /// 合成卡牌时触发（主要用于解锁三字牌）
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnComposite(BaseCard card)
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
}
