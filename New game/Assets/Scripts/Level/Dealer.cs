
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
            return newCard;
        }
        else
        {
            PoolMgr.Instance.PushObj(cardPrefab);
            Debug.LogWarning($"卡牌{newCard.name}创建失败");
            return null;
        }
    }


    public string RandomBaseCardResName()
    {
        int random = UnityEngine.Random.Range(0, 4);
        switch (random)
        {
            case 0:
                return DataCenter.Instance.cardResNameData.base_fire_huo;
            case 1:
                return DataCenter.Instance.cardResNameData.base_water_shui;
            case 2:
                return DataCenter.Instance.cardResNameData.base_earth_tu;
            case 3:
                return DataCenter.Instance.cardResNameData.base_wood_mu;
            default:
                return string.Empty;
        }
    }

    /// <summary>
    /// 依据（牌容量-持有牌）/2抽牌，弃用
    /// </summary>
    /// <param name="isFirst"></param>

    [Obsolete]
    public void DealBasicCard(bool isFirst)
    {
        Debug.Log("[荷官发牌]此次的发牌行为是" + isFirst);
        float cardCount;
        if (isFirst)
            cardCount = baseCardCapicity;
        else
            cardCount = (baseCardCapicity - GetBaseCardCount()) * dealCardMultiple;

        if (cardCount < 0)
        {
            Debug.Log($"[发牌逻辑]基础牌数量count为负数，强制修正为0");
            cardCount = 0;
        }
        //获得预先发牌数
        int result = Mathf.FloorToInt(cardCount) + extraCardCount;
        if(NowCapicity + result > capicity)//当前持有牌数量+预发牌数量超过总容量上限时，修正预发牌数量为剩余容量
        {
            result = capicity - NowCapicity;
            Debug.Log($"[发牌逻辑]预发牌数量超过总容量上限，强制修正预发牌数量为剩余容量{result}");
        }


        Debug.Log($"[发牌逻辑]本次要发的卡牌数量为{result}");

        for (int i = 0; i < result; i++)
        {
            BaseCard card = CreateAndAddCard(RandomBaseCardResName(), 0);
            if(card != null)
            {
                Debug.Log(card.name + "创建成功");
                //触发所有奇物抽牌效果
                GamePlayer.Instance.playerBag.OnDrawCard(card);
            }
           
        }

        SortNowCards();
    }

    public void DealBasicCards(bool isFirst)
    {
        Debug.Log("[荷官发牌]此次的发牌行为是" + isFirst);
        float cardCount;
        if (isFirst)
            cardCount = baseCardCapicity;
        else
            cardCount = drawCardCount + extraCardCount;


        if (NowCapicity + cardCount > capicity)//当前持有牌数量+预发牌数量超过总容量上限时，修正预发牌数量为剩余容量
        {
            cardCount = capicity - NowCapicity;
            Debug.Log($"[发牌逻辑]预发牌数量超过总容量上限，强制修正预发牌数量为剩余容量{cardCount}");
        }


        Debug.Log($"[发牌逻辑]本次要发的卡牌数量为{cardCount}");

        for (int i = 0; i < cardCount; i++)
        {
            BaseCard card = CreateAndAddCard(RandomBaseCardResName(), 0);
            if (card != null)
            {
                Debug.Log(card.name + "创建成功");
                //触发所有奇物抽牌效果
                GamePlayer.Instance.playerBag.OnDrawCard(card);
            }

        }
        SortNowCards();
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
                    Debug.Log("[合成bug检测]检测到卡牌在持有卡牌中，进行表移除"+card.cardID);

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
                //card.DestroyMe();   // 确保一定执行销毁
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

    public void RemoveAllRadicalCard()
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
        RemoveAllRadicalCard();

        // 清除所有基础牌和合成牌（包括可能残留的任意卡牌）
        for (int i = nowCards.Count - 1; i >= 0; i--)
        {
            var card = nowCards[i];
            if (card != null)
            {
                //// 直接销毁，绕过可能的状态检查
                //card.DestroyMe();
                 GameObject.Destroy(card.gameObject);
            }
        }
        nowCards.Clear();

        // 清除部首卡槽引用
        ClearSlots();
    }
}