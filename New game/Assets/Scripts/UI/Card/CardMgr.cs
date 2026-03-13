using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 管理卡牌的创建（抽取、获得）、销毁
/// </summary>
public class CardMgr : BaseMonoMgr<CardMgr>
{
    // 手牌上限
    private const int capicity = 15;

    private int nowCapicity;
    /// <summary>
    /// 当前持有卡牌数量(不包括部首牌)
    /// </summary>
    public int NowCapicity => nowCapicity;

    // 当前玩家持有的卡牌(不包括部首牌)
    public List<BaseCard> nowCards = new List<BaseCard>(capicity);
    // 当前玩家拥有的部首牌
    public List<BaseCard> nowRadicalCards = new List<BaseCard>(capicity);

    /// <summary>
    /// 添加卡牌到手牌（核心方法）
    /// </summary>
    /// <param name="card">持有的卡牌</param>
    /// <returns>是否添加成功</returns>
    public bool AddCard(BaseCard card)
    {
        if (card == null)
        {
            Debug.LogWarning("添加的卡牌为空，无法加入手牌");
            return false;
        }

        switch (card.cardType)
        {
            case E_CardType.Base:
            case E_CardType.Combine:
                // 检查手牌上限
                if (nowCapicity < capicity)
                {
                    // 避免重复添加同一卡牌
                    if (!nowCards.Contains(card))
                    {
                        nowCards.Add(card);
                        nowCapicity++; // 同步更新当前手牌数量
                        return true;
                    }
                    else
                    {
                        Debug.LogWarning($"卡牌{card.name}已存在于手牌中，无需重复添加");
                        return false;
                    }
                }
                else
                {
                    Debug.LogWarning("无法将卡牌添加到手牌，手牌容量已满");
                    return false;
                }

            case E_CardType.Radical:
                if (!nowRadicalCards.Contains(card))
                {
                    nowRadicalCards.Add(card);
                    return true;
                }
                else
                {
                    Debug.LogWarning($"部首牌{card.name}已存在，无需重复添加");
                    return false;
                }

            default:
                Debug.LogWarning($"未知卡牌类型{card.cardType}，无法添加");
                return false;
        }
    }

    /// <summary>
    /// 移除手牌卡牌
    /// </summary>
    /// <param name="card">持有的卡牌</param>
    public void RemoveCard(BaseCard card)
    {
        if (card == null) return;

        switch (card.cardType)
        {
            case E_CardType.Base:
            case E_CardType.Combine:
                if (nowCards.Remove(card))
                {
                    nowCapicity--; // 移除后更新数量
                    card.DestroyMe(); // 销毁卡牌对象
                }
                break;

            case E_CardType.Radical:
                if (nowRadicalCards.Remove(card))
                {
                    card.DestroyMe();
                }
                break;
        }
    }

    /// <summary>
    /// 移除所有部首牌
    /// </summary>
    public void RemoveAllRadicalCard()
    {
        //反向遍历避免索引异常
        for (int i = nowRadicalCards.Count - 1; i >= 0; i--)
        {
            nowRadicalCards[i].DestroyMe();
        }
        nowRadicalCards.Clear();
    }

    /// <summary>
    /// 移除除部首牌外的所有牌
    /// </summary>
    public void RemoveAllNowCards()
    {
        for (int i = nowCards.Count - 1; i >= 0; i--)
        {
            nowCards[i].DestroyMe();
        }
        nowCards.Clear();
        nowCapicity = 0; // 重置手牌数量
    }

    /// <summary>
    /// 创建卡牌并自动加入手牌（创建+添加联动）
    /// </summary>
    /// <param name="resPath">卡牌预设体路径(通过DataCenter获得)</param>
    /// <param name="parent">卡牌的父物体（如手牌面板）</param>
    /// <returns>创建并添加成功的卡牌</returns>
    public BaseCard CreateAndAddCard(string resPath, Transform parent = null)
    {
        // 1. 加载卡牌预设体
        BaseCard cardPrefab = Resources.Load<BaseCard>("UI/Card/base_fire_huo");
        if (cardPrefab == null)
        {
            Debug.LogError($"创建卡牌失败：资源路径{resPath}无效，未找到卡牌预设体");
            return null;
        }

        // 2. 实例化卡牌
        BaseCard newCard = Instantiate(cardPrefab, parent);
        if (newCard == null)
        {
            Debug.LogError("卡牌实例化失败，无法创建游戏对象");
            return null;
        }
 

        // 3. 调用AddCard将卡牌加入手牌
        if (AddCard(newCard))
        {
            Debug.Log($"卡牌{newCard.name}创建并成功加入手牌，当前手牌数量：{nowCapicity}");
            return newCard;
        }
        else
        {
            // 添加失败时销毁实例化的卡牌，避免内存泄漏
            Destroy(newCard.gameObject);
            Debug.LogWarning($"卡牌{newCard.name}创建成功，但添加到手牌失败");
            return null;
        }
    }
}