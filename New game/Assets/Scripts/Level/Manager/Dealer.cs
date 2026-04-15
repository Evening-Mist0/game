using System;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : BaseMonoMgr<Dealer>
{
    [Header("最大手牌容量")]
    public int capicity = 7;
    [Header("基础手牌容量")]
    public int baseCardCapicity = 4;

    /// <summary>
    /// 额外抽牌数(部首牌)
    /// </summary>
    public int extraCardCount = 0;

    /// <summary>
    /// 每回合抽牌数量（基础牌）
    /// </summary>
    public int drawCardCount = 2;

    public int NowCapicity => nowCards.Count;
    public List<BaseCard> nowCards = new List<BaseCard>();

    public BaseRadicalCard slotXi;
    public BaseRadicalCard slotYe;
    public BaseRadicalCard slotKe;
    public BaseRadicalCard slotPi;
    private float dealCardMultiple = 0.5f;

    // ========== 概率加权法相关字段 ==========
    // 索引 0=火,1=水,2=土,3=木
    private int[] baseCardDrawCount = new int[4];

    // 可调节的权重衰减因子（越大越倾向于平均，越小越随机）
    // 推荐值 5~10，你可以根据游戏感受调整
    [Header("抽牌平衡强度 (越大越平均，1=完全随机)")]
    [SerializeField] private int balanceStrength = 6;


    private bool AddCard(BaseCard card)
    {
        if (card == null)
        {
            Debug.LogWarning("添加的卡牌为空，无法加入手牌");
            return false;
        }

        nowCards.RemoveAll(card => card == null);

        switch (card.cardType)
        {
            case E_CardType.Base:
            case E_CardType.Combine:
            case E_CardType.BasicCombine:
                if (NowCapicity <= capicity)
                {
                    nowCards.Add(card);
                    Debug.Log($"卡牌{card.name}创建并成功加入手牌，当前手牌数量：{NowCapicity}");
                    return true;
                }
                else
                {
                    Debug.LogWarning("无法向手牌添加卡牌，手牌已达容量上限");
                    return false;
                }

            case E_CardType.Radical:
                BaseRadicalCard radicalCard = card as BaseRadicalCard;
                if (radicalCard == null)
                {
                    Debug.LogError("该牌的类型为部首牌，但是无法进行里氏替换");
                    return false;
                }
                switch (radicalCard.radicalCardType)
                {
                    case E_RadicalCardType.Xi:
                        slotXi.AddCardCount();
                        break;
                    case E_RadicalCardType.Ye:
                        slotYe.AddCardCount();
                        break;
                    case E_RadicalCardType.Ke:
                        slotKe.AddCardCount();
                        break;
                    case E_RadicalCardType.Pi:
                        slotPi.AddCardCount();
                        break;
                }
                Debug.Log($"卡牌{card.name}成功加入到部首卡槽，当前{card.name}手牌数量：{radicalCard.myCardCount}");
                return true;

            default:
                Debug.LogWarning($"未知卡牌类型{card.cardType}，无法添加");
                return false;
        }
    }

    public BaseCard CreateAndAddCard(string resPath, int creatPos, Transform parent = null)
    {
        Debug.Log($"[Dealer]尝试创建卡牌，资源路径：{resPath}，创建位置：{creatPos}");
        var cardPanel = UIMgr.Instance.GetPanel<CardPlayingPanel>();
        if (cardPanel == null || cardPanel.originMainPos == null)
        {
            Debug.LogWarning("[Dealer] 无法获取 CardPlayingPanel 或其 originMainPos，请检查 UI 初始化顺序");
            return null;
        }
        parent = cardPanel.originMainPos.transform;

        GameObject cardPrefab = PoolMgr.Instance.GetObj(resPath);
        if (cardPrefab == null)
        {
            Debug.LogError($"卡牌加载失败，资源路径{resPath}无效");
            return null;
        }

        BaseCard newCard = cardPrefab.GetComponent<BaseCard>();

        //调用典籍调整卡牌激活状态
        GamePlayer.Instance.playerBag.BookOnComposite(newCard);

        //判定卡牌是否被激活，未被激活则合成失败
        if (newCard.isActive == false)
        {
            Debug.Log($"[Dealer]判定到卡牌{newCard.name}没有被激活，合成失败");
            RemoveCard(newCard);
            return null;
        }

        cardPrefab.transform.SetParent(parent, false);
        newCard.cardEffectControl.ResetTransform();

        if (newCard.cardType != E_CardType.Radical)
        {
            cardPrefab.transform.SetSiblingIndex(creatPos);
        }

        if (AddCard(newCard))
        {
            Debug.Log($"[Dealer]卡牌{newCard.name}成功创建并添加到手牌");
            return newCard;
        }
        else
        {
            PoolMgr.Instance.PushObj(cardPrefab);
            Debug.LogWarning($"[Dealer]卡牌{newCard.name}创建失败");
            return null;
        }
    }


    /// <summary>
    /// 根据权重获取基础牌资源路径（计数越小权重越大，但不会完全排除计数大的牌）
    /// </summary>
    public string GetWeightedBaseCardResName()
    {
        // 计算每个牌型的权重
        // 公式: 权重 = 平衡强度 / (计数 + 1)  -> 计数越大权重越小，但永不为0
        // 你也可以使用线性: 权重 = balanceStrength - baseCardDrawCount[i] (但需保证最小为1)
        // 这里使用指数衰减，更平滑
        int[] weights = new int[4];
        int totalWeight = 0;

        for (int i = 0; i < 4; i++)
        {
            // 使用除法权重：计数越大，权重越小，但最小为1
            // balanceStrength 越大，计数对权重影响越大（更平均）
            weights[i] = Mathf.Max(1, balanceStrength / (baseCardDrawCount[i] + 1));
            totalWeight += weights[i];
        }

        // 随机选择
        int rand = UnityEngine.Random.Range(0, totalWeight);
        int accum = 0;
        for (int i = 0; i < 4; i++)
        {
            accum += weights[i];
            if (rand < accum)
            {
                // 更新计数
                baseCardDrawCount[i]++;
                return GetResPathByIndex(i);
            }
        }

        // fallback (理论上不会走到这里)
        baseCardDrawCount[0]++;
        return GetResPathByIndex(0);
    }

    /// <summary>
    /// 根据索引返回对应基础牌的资源路径
    /// </summary>
    private string GetResPathByIndex(int index)
    {
        switch (index)
        {
            case 0: return DataCenter.Instance.cardResNameData.base_fire_huo;
            case 1: return DataCenter.Instance.cardResNameData.base_water_shui;
            case 2: return DataCenter.Instance.cardResNameData.base_earth_tu;
            case 3: return DataCenter.Instance.cardResNameData.base_wood_mu;
            default: return string.Empty;
        }
    }

    /// <summary>
    /// 重置抽牌计数器（可在回合开始或战斗开始时调用）
    /// </summary>
    public void ResetDrawCount()
    {
        for (int i = 0; i < baseCardDrawCount.Length; i++)
        {
            baseCardDrawCount[i] = 0;
        }
        Debug.Log("[Dealer] 抽牌计数器已重置");
    }


    [Obsolete("请使用 GetWeightedBaseCardResName 实现平均抽牌")]
    public string RandomBaseCardResName()
    {
        int random = UnityEngine.Random.Range(0, 4);
        return GetResPathByIndex(random);
    }

    // 主要使用的发牌方法（你代码中实际调用的）
    public void DealBasicCards(bool isFirst)
    {
        Debug.Log("[荷官发牌]此次的发牌行为是" + isFirst);
        float cardCount;
        if (isFirst)
            cardCount = baseCardCapicity;
        else
            cardCount = drawCardCount + extraCardCount;

        if (NowCapicity + cardCount > capicity)
        {
            cardCount = capicity - NowCapicity;
            Debug.Log($"[发牌逻辑]预发牌数量超过总容量上限，强制修正预发牌数量为剩余容量{cardCount}");
        }

        Debug.Log($"[发牌逻辑]本次要发的卡牌数量为{cardCount}");

        for (int i = 0; i < cardCount; i++)
        {
            // 使用概率加权法抽牌
            BaseCard card = CreateAndAddCard(GetWeightedBaseCardResName(), 0);
            if (card != null)
            {
                Debug.Log(card.name + "创建成功");
                GamePlayer.Instance.playerBag.OnDrawCard(card);
            }
        }
        SortNowCards();
    }

    // 旧版发牌（如果还有地方调用，改为调用 DealBasicCards 或更新）
    [Obsolete("请使用 DealBasicCards")]
    public void DealBasicCard(bool isFirst)
    {
        DealBasicCards(isFirst);
    }

    private int GetBaseCardCount()
    {
        int count = 0;
        for (int i = 0; i < nowCards.Count; i++)
        {
            if (nowCards[i] == null) continue;
            if (nowCards[i].cardType == E_CardType.Base)
            {
                count++;
            }
        }
        Debug.Log($"[发牌逻辑]获取到基础牌数量为{count}");
        return count;
    }

    public void RemoveCard(BaseCard card)
    {
        if (card == null) return;

        switch (card.cardType)
        {
            case E_CardType.Base:
            case E_CardType.Combine:
            case E_CardType.BasicCombine:
                Debug.Log("[合成bug检测]删除卡牌" + card.cardID);
                if (nowCards.Contains(card))
                {
                    Debug.Log("[合成bug检测]检测到卡牌在持有卡牌中，进行表移除" + card.cardID);
                    bool removed = nowCards.Remove(card);
                    Debug.Log($"RemoveCard: 尝试移除 {card.cardID}, 结果={removed}");
                    if (removed)
                    {
                        GamePlayer.Instance.RemoveCardInCompositeList(card);
                        card.DestroyMe();
                    }
                    else
                        Debug.LogWarning($"卡牌 {card.cardID} 不在 nowCards 中，无法销毁！");
                }
                break;

            case E_CardType.Radical:
                BaseRadicalCard radicalCard = card as BaseRadicalCard;
                if (radicalCard == null)
                {
                    Debug.LogError("该牌的类型为部首牌，但是无法进行里氏替换");
                    return;
                }
                if (radicalCard.isSlot)
                {
                    card.cardEffectControl.ForceUnlockAndReturn();
                    radicalCard.ReduceCardCount();
                }
                else
                {
                    Debug.Log("[合成成功删除卡牌]用部首牌进行合成");
                    PoolMgr.Instance.PushObj(card.gameObject);
                }
                break;
        }
    }

    public void RemoveAllCards()
    {
        for (int i = nowCards.Count - 1; i >= 0; i--)
        {
            BaseCard card = nowCards[i];
            if (card != null)
            {
                RemoveCard(card);
                Debug.Log($"[元素湮灭] 成功移除：{card.name}");
            }
        }
    }

    public void RemoveAllRadicalCards()
    {
        slotXi.CardCountTurnZero();
        slotPi.CardCountTurnZero();
        slotKe.CardCountTurnZero();
        slotYe.CardCountTurnZero();
    }

    public void GetRadicalCardSlot(BaseRadicalCard radicalCard)
    {
        switch (radicalCard.radicalCardType)
        {
            case E_RadicalCardType.Xi:
                if (slotXi == null)
                    slotXi = radicalCard as Radical_Xi;
                break;
            case E_RadicalCardType.Ye:
                if (slotYe == null)
                    slotYe = radicalCard as Radical_Ye;
                break;
            case E_RadicalCardType.Ke:
                if (slotKe == null)
                    slotKe = radicalCard as Radical_Ke;
                break;
            case E_RadicalCardType.Pi:
                if (slotPi == null)
                    slotPi = radicalCard as Radical_Pi;
                break;
        }
    }

    public void ClearSlots()
    {
        slotXi = null;
        slotPi = null;
        slotKe = null;
        slotYe = null;
    }

    public void SortNowCards()
    {
        nowCards.RemoveAll(card => card == null);
        nowCards.Sort((a, b) => a.weight.CompareTo(b.weight));
        RefreshCardDisplayOrder();
    }

    private void RefreshCardDisplayOrder()
    {
        for (int i = 0; i < nowCards.Count; i++)
        {
            nowCards[i].transform.SetSiblingIndex(i);
        }
    }

    public void RemoveNowCardsExceptRadical()
    {
        for (int i = nowCards.Count - 1; i >= 0; i--)
        {
            RemoveCard(nowCards[i]);
        }
        nowCards.Clear();
    }

    /// <summary>
    /// 重置荷官的状态更新为初始状态
    /// </summary>
    public void ResetDealer()
    {
        // 清除所有部首卡槽计数
        RemoveAllRadicalCards();

        // 清除所有基础牌和合成牌（包括可能残留的任意卡牌）
        for (int i = nowCards.Count - 1; i >= 0; i--)
        {
            var card = nowCards[i];
            if (card != null)
            {
                GameObject.Destroy(card.gameObject);
            }
        }
        nowCards.Clear();

        // 清除部首卡槽引用
        ClearSlots();

        // 重置抽牌计数器
        ResetDrawCount();
    }
}