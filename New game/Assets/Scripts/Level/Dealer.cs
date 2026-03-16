using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 荷官：管理卡牌的创建（抽取、获得）、销毁
/// </summary>
public class Dealer : BaseMonoMgr<Dealer>
{
    // 手牌上限
    private const int capicity = 15;
    //基础牌上限
    private const int baseCardCapicity = 9;

    private int nowCapicity;
    /// <summary>
    /// 当前持有卡牌数量(不包括部首牌)
    /// </summary>
    public int NowCapicity => nowCapicity;

    // 当前玩家持有的卡牌(不包括部首牌)
    public List<BaseCard> nowCards = new List<BaseCard>(capicity);
    // 当前玩家拥有的部首牌
    public List<BaseCard> nowRadicalCards = new List<BaseCard>(capicity);
    //发牌数量计算倍数(baseCardCapicity - 基础卡牌数)/dealCardMultiple
    private float dealCardMultiple = 0.5f;

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
    /// 随机出一张基础卡牌资源名
    /// </summary>
    /// <returns></returns>
    public string RandomBaseCardResName()
    {
        int random = Random.Range(0, 4);
        switch(random)
        {
            case 0:
                return DataCenter.Instance.resNameData.base_fire_huo;
            case 1:
                return DataCenter.Instance.resNameData.base_water_shui;
            case 2:
                return DataCenter.Instance.resNameData.base_earth_tu;
            case 3:
                return DataCenter.Instance.resNameData.base_wood_mu;
            default:
                return string.Empty;
        }

    }

    /// <summary>
    /// 发送卡牌到玩家手牌
    /// </summary>
    /// <param name="isFirst">是否初始化卡牌（发9张）</param>
    public void DealBasicCards(bool isFirst)
    {
        float cardCount;
        if (isFirst)//发baseCardCapicity张基础牌
            cardCount = baseCardCapicity;
        else//根据卡牌数量补充
            //根据抽牌公式计算要抽的卡牌数量
            cardCount = (baseCardCapicity - GetBaseCardCount()) * dealCardMultiple;

        //根据数量添加卡牌到手牌
        for (int i = 0; i < cardCount; i++)
        {
            CreateAndAddCard(RandomBaseCardResName());
        }                
    }

    /// <summary>
    /// 得到基础卡牌数量
    /// </summary>
    /// <returns>当前基础卡牌数量</returns>
    private int GetBaseCardCount()
    {
        int count = 0;
        for(int i = 0;i < nowCards.Count;i++)
        {
            if (nowCards[i].cardType == E_CardType.Base)
                count++;
        }
        return count;
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
    public void RemoveNowCardsExceptRadical()
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
        BaseCard cardPrefab = Resources.Load<BaseCard>(resPath);
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

   
    /// <summary>
    /// 卡牌排序（除部首牌）
    /// </summary>
    public void SrotNowCards()
    {
        //把现有卡牌数据计入临时表
        List<BaseCard> tempCardList = nowCards;
        //清空所有牌
        RemoveNowCardsExceptRadical();
        //根据权值进行卡牌排序
        tempCardList.Sort((a, b) => a.weight.CompareTo(b.weight));
        //按顺序重新创建卡牌
        for(int i = 0; i < tempCardList.Count;i++)
        {
            if (tempCardList[i].MyResName != null)
                CreateAndAddCard(tempCardList[i].MyResName);
            else
                Debug.LogError("传递的资源加载名有误");
        }
    }
}