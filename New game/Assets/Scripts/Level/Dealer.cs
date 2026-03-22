using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 发牌者，负责卡牌的创建、获取、添加、移除等
/// </summary>
public class Dealer : BaseMonoMgr<Dealer>
{
    // 手牌容量
    private const int capicity = 15;
    //基础卡牌初始数量
    private const int baseCardCapicity = 9;

    /// <summary>
    /// 当前手牌中的卡牌数(对外只读)
    /// </summary>
    public int NowCapicity => nowCards.Count;

    // 当前手牌中的卡牌(基础/组合牌)
    public List<BaseCard> nowCards = new List<BaseCard>(capicity);

    //偏旁卡槽索引（一种特殊的牌，获得的是它的卡槽，实际的卡牌数量只需要用卡槽里面的myCardCount判定）
    public BaseRadicalCard slotXi;
    public BaseRadicalCard slotYe;
    public BaseRadicalCard slotKe;
    public BaseRadicalCard slotPi;
    //发牌数量公式:(baseCardCapicity - 基础牌数量)/dealCardMultiple
    private float dealCardMultiple = 0.5f;

    /// <summary>
    /// 向手牌添加卡牌，公共方法
    /// </summary>
    /// <param name="card">要添加的卡牌</param>
    /// <returns>是否添加成功</returns>
    private bool AddCard(BaseCard card)
    {
        if (card == null)
        {
            Debug.LogWarning("添加的卡牌为空，无法加入手牌");
            return false;
        }

        //清理空对象,保证持有卡牌数量正确
        nowCards.RemoveAll(card => card == null);

        switch (card.cardType)
        {
            case E_CardType.Base:
            case E_CardType.Combine:
                // 检查手牌容量,添加手牌

                if (NowCapicity < capicity) // 直接用nowCards.Count判断容量
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

    /// <summary>
    /// 创建卡牌并添加到手牌
    /// </summary>
    /// <param name="resPath">卡牌资源路径</param>
    /// <param name="parent">父对象(默认为空就会找到主卡槽作为父对象)</param>
    /// <param name="creatPos">创建的位置（格子布局组件位置索引）</param>
    /// <returns></returns>
    public BaseCard CreateAndAddCard(string resPath, int creatPos, Transform parent = null)
    {
        if(parent == null)
        {
            parent = UIMgr.Instance.GetPanel<CardPlayingPanel>().originMainPos.transform;
        }

        GameObject cardPrefab = PoolMgr.Instance.GetObj(resPath);

        if (cardPrefab == null)
        {
            Debug.LogError($"卡牌加载失败，资源路径{resPath}无效");
            return null;
        }

        BaseCard newCard = cardPrefab.GetComponent<BaseCard>();
        cardPrefab.transform.SetParent(parent, false);

       
        newCard.cardEffectControl.ResetTransform();

        if (newCard.cardType != E_CardType.Radical)//如果不是部首牌才要改变位置插入 
        {
            cardPrefab.transform.SetSiblingIndex(creatPos);
        }



        if (AddCard(newCard))
            return newCard;
        else
        {
            PoolMgr.Instance.PushObj(cardPrefab);
            Debug.LogWarning($"卡牌{newCard.name}创建失败");
            return null;
        }
    }


    /// <summary>
    /// 随机获取一个基础卡牌资源名
    /// </summary>
    /// <returns></returns>
    public string RandomBaseCardResName()
    {
        int random = Random.Range(0, 4);
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
    /// 分发基础卡牌
    /// </summary>
    /// <param name="isFirst">是否是首次发牌（发9张）</param>
    public void DealBasicCards(bool isFirst)
    {
        float cardCount;
        if (isFirst)//首次发baseCardCapicity张基础牌
            cardCount = baseCardCapicity;
        else//根据当前基础牌数量计算
            //根据公式计算需要发的卡牌数量
            cardCount = (baseCardCapicity - GetBaseCardCount()) * dealCardMultiple;

        if (cardCount < 0)
        {
            Debug.Log($"[发牌逻辑]基础牌数量count为负数，强制修正为0");
            cardCount = 0;
        }
        int result = Mathf.FloorToInt(cardCount);

        Debug.Log($"[发牌逻辑]本次要发的卡牌数量为{result}");

        //循环创建并添加卡牌到手牌
        for (int i = 0; i < result; i++)
        {
            CreateAndAddCard(RandomBaseCardResName(), 0);
        }

        //排序卡牌
        SortNowCards();
    }

    /// <summary>
    /// 获取当前基础牌数量（自动过滤 Missing 空对象）
    /// </summary>
    /// <returns>当前基础牌数量</returns>
    private int GetBaseCardCount()
    {
        int count = 0;
        for (int i = 0; i < nowCards.Count; i++)
        {
            //过滤Missing/已销毁的卡牌
            if (nowCards[i] == null) continue;

            if (nowCards[i].cardType == E_CardType.Base)
            {
                count++;
            }
        }
        Debug.Log($"[发牌逻辑]获取到基础牌数量为{count}");
        return count;
    }

    /// <summary>
    /// 从手牌移除卡牌（全类型，自动清理）
    /// </summary>
    /// <param name="card">要移除的卡牌</param>
    public void RemoveCard(BaseCard card)
    {
        if (card == null) return;

        switch (card.cardType)
        {
            case E_CardType.Base:
            case E_CardType.Combine:
                if (nowCards.Remove(card))
                {
                    card.DestroyMe();
                }
                break;

            case E_CardType.Radical:
                
                BaseRadicalCard radicalCard = card as BaseRadicalCard;
                if (radicalCard == null)
                {
                    Debug.LogError("该牌的类型为部首牌，但是无法进行里氏替换");
                    return;
                }
                if(radicalCard.isSlot) //被移除的是卡槽，减少部首牌计数，强制返回未选中状态
                {
                   
                    card.cardEffectControl.ForceUnlockAndReturn();
                    radicalCard.ReduceCardCount();
                }
                else//被移除的是部首实例，直接放回对象池
                {
                    PoolMgr.Instance.PushObj(card.gameObject);
                }

                break;
        }
    }


    /// <summary>
    /// 移除手牌中的所有基础牌
    /// </summary>
    public void RemoveAllBasicCards()
    {
        for (int i = nowCards.Count - 1; i >= 0; i--)
        {
            BaseCard card = nowCards[i];
            if (card != null && card.cardType == E_CardType.Base)
            {
                RemoveCard(card);
                Debug.Log($"[移除基础牌] 成功移除：{card.name}");
            }
        }
    }

    /// <summary>
    /// 移除所有部首牌
    /// </summary>
    public void RemoveAllRadicalCard()
    {
        slotXi.CardCountTurnZero();
        slotPi.CardCountTurnZero();
        slotKe.CardCountTurnZero();
        slotYe.CardCountTurnZero();
    }


    /// <summary>
    /// 得到部首卡槽引用(按理来说，会在InitState显示面板的时候初始化打牌面板的时候，就能得到卡槽脚本)
    /// （后续会有卡牌掉落也是相同的脚本类型所以要判空处理）
    /// </summary>
    public void GetRadicalCardSlot(BaseRadicalCard radicalCard)
    {
        switch (radicalCard.radicalCardType)
        {
            case E_RadicalCardType.Xi:
                if(slotXi == null)
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

    /// <summary>
    /// 清空卡槽引用（游戏结束时调用）
    /// </summary>
    public void ClearSlots()
    {
        slotXi = null;
        slotPi = null;
        slotKe = null;
        slotYe = null;
    }


   
    /// <summary>
    /// 排序手牌（清理空值+按weight排序）
    /// </summary>
    public void SortNowCards()
    {
        //清理对象
        nowCards.RemoveAll(card => card == null);

        // 按 weight 从小到大排序（weight=0 的在前面）
        nowCards.Sort((a, b) => a.weight.CompareTo(b.weight));

        //刷新卡牌显示顺序，和列表顺序一致
        RefreshCardDisplayOrder();
    }

    /// <summary>
    /// 刷新卡牌的显示顺序，和 nowCards 列表顺序完全一致
    /// </summary>
    private void RefreshCardDisplayOrder()
    {
        for (int i = 0; i < nowCards.Count; i++)
        {
            // 设置卡牌在父节点中的排序，和列表显示顺序保持一致
            nowCards[i].transform.SetSiblingIndex(i);
        }
    }

    /// <summary>
    /// 移除手牌中除偏旁外的所有卡牌
    /// </summary>
    public void RemoveNowCardsExceptRadical()
    {
        for (int i = nowCards.Count - 1; i >= 0; i--)
        {
            RemoveCard(nowCards[i]);
        }
        nowCards.Clear();
        // 移除nowCapicity = 0，因为nowCapicity已废弃
    }
}