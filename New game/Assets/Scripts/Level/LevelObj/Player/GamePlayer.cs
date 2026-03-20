using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 单例模式，玩家游戏实体
/// </summary>
[RequireComponent(typeof(PlayerEffectControl))]
public class GamePlayer : BaseGameObject
{
    private static GamePlayer instance;
    public static GamePlayer Instance => instance;

    public override E_GameObjectType gameObjectType => E_GameObjectType.Player;

    public int hp = 1;

    private PlayerEffectControl effectControl;




    // 卡牌操作相关字段（原 CardOperateState 字段）
    /// <summary>
    /// 持有卡牌的卡牌列表
    /// </summary>
    public List<BaseCard> cardList = new List<BaseCard>();
    /// <summary>
    /// 当前选中的卡牌
    /// </summary>
    public BaseCard nowSelectedCard;
    /// <summary>
    /// 被右键选中的卡牌列表
    /// </summary>
    public List<BaseCard> CardCompositeList = new List<BaseCard>(2);
    public int rightMouseButtonClikCount;
    /// <summary>
    /// 玩家当前选中的格子
    /// </summary>
    public Cell preSlectedCell;
    /// <summary>
    /// 玩家选中的格子根据卡牌范围辐射的所有格子
    /// </summary>
    public List<Cell> preSlectedCellList = new List<Cell>();
    /// <summary>
    /// 是否允许格子高亮
    /// </summary>
    public bool isAllowedCellHighLight;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        effectControl = GetComponent<PlayerEffectControl>();
    }


    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    public void Hurt(int value)
    {
        effectControl.PlayerHurt();
        Debug.Log("玩家受到伤害" + value);
        hp -= value;
        if (hp <= 0)
        {
            Debug.Log("[游戏结算]玩家游戏失败");
            effectControl.PlayDead();
        }
    }

    /// <summary>
    /// 玩家得到治愈
    /// </summary>
    public void GetHeal(int value)
    {
        hp += value;
        Debug.Log("玩家得到治愈效果" + value);
    }


    #region 合成相关
    /// <summary>
    /// 添加卡片到合成列表
    /// </summary>
    public void AddCardInCompositeList(BaseCard card)
    {
        if (card == null || CardCompositeList.Contains(card))
        {
            Debug.LogWarning("卡片为空或已在合成列表中，跳过添加");
            return;
        }

        if (CardCompositeList.Count >= 2)
        {
            Debug.LogWarning("合成列表已达上限（2张），无法添加");
            return;
        }

        card.isRightMouseButtonCliking = true;
        CardCompositeList.Add(card);
        Debug.Log($"添加卡片[{card.cardID}]到合成列表，当前数量：{CardCompositeList.Count}");

        if (CardCompositeList.Count == 2)
        {
            int newCardPos;
            if (card.cardType == E_CardType.Radical)
                newCardPos = GetOtherCompositeCardIndex(card);
            else
                newCardPos = card.transform.GetSiblingIndex();

            CompositeCard(newCardPos);
        }
    }

    /// <summary>
    /// 得到合成表的另一张卡牌索引
    /// </summary>
    private int GetOtherCompositeCardIndex(BaseCard nowSlectedCard)
    {
        if (CardCompositeList.Count == 2)
        {
            for (int i = 0; i < CardCompositeList.Count; i++)
            {
                if (CardCompositeList[i] != nowSlectedCard)
                    return CardCompositeList[i].transform.GetSiblingIndex();
            }
            Debug.LogError("传进来的卡牌居然都等于合成表的卡牌？什么鬼玩意");
            return 0;
        }
        else
        {
            Debug.LogError("该此坐标获取无效，合成表数的count不为2，照理来说不会出现这种情况，请仔细检查！");
            return 0;
        }
    }

    /// <summary>
    /// 从合成列表移除卡片
    /// </summary>
    public void RemoveCardInCompositeList(BaseCard card)
    {
        if (card == null || !CardCompositeList.Contains(card)) return;

        card.isRightMouseButtonCliking = false;
        CardCompositeList.Remove(card);
        Debug.Log($"移除卡片[{card.cardID}]，合成列表剩余：{CardCompositeList.Count}");
    }

    /// <summary>
    /// 清空合成列表
    /// </summary>
    public void RemoveAllCardInCompositeList()
    {
        foreach (var card in CardCompositeList)
        {
            if (card != null)
            {
                card.isRightMouseButtonCliking = false;
            }
        }
        CardCompositeList.Clear();
        rightMouseButtonClikCount = 0;
    }

    /// <summary>
    /// 合成卡片
    /// </summary>
    public void CompositeCard(int newCardPos)
    {
        Debug.Log($"开始合成判断，当前列表数量{CardCompositeList.Count}");

        if (CardCompositeList.Count != 2)
        {
            Debug.Log("合成条件不满足（非2张卡牌），终止合成");
            return;
        }

        BaseCard newCard = TryCompositeCurrentCard(newCardPos);

        if (newCard != null)
        {
            Debug.Log($"合成成功，新卡牌：{newCard.cardID}");

            List<BaseCard> tempOldCards = new List<BaseCard>(CardCompositeList);
            RemoveAllCardInCompositeList();

            foreach (var oldCard in tempOldCards)
            {
                if (oldCard != null)
                {
                    oldCard.isRightMouseButtonCliking = false;
                    Dealer.Instance.RemoveCard(oldCard);
                }
            }

            TypeSafeEventCenter.Instance.Trigger<CardCompositeSuccessEvent>(new CardCompositeSuccessEvent(newCard));
        }
        else
        {
            Debug.Log("合成失败，无匹配的合成方式");

            foreach (var card in CardCompositeList)
            {
                if (card != null)
                {
                    card.isRightMouseButtonCliking = false;
                    TypeSafeEventCenter.Instance.Trigger<CardCancelOhterRightSelectEvent>(new CardCancelOhterRightSelectEvent(card));
                }
            }

            RemoveAllCardInCompositeList();
        }
    }

    /// <summary>
    /// 尝试合成当前卡片
    /// </summary>
    private BaseCard TryCompositeCurrentCard(int newCardPos)
    {
        try
        {
            string cardID0 = CardCompositeList[0].cardID;
            string cardID1 = CardCompositeList[1].cardID;
            Debug.Log($"校验合成公式：{cardID0} + {cardID1}");

            var tuple = CardSynthesisFormulaTable.Instance.GetSortedCardIdTuple(cardID0, cardID1);
            if (CardSynthesisFormulaTable.Instance.SynthesisDic.TryGetValue(tuple, out var formula))
            {
                return Dealer.Instance.CreateAndAddCard(formula.resultResName, newCardPos, UIMgr.Instance.GetPanel<CardPlayingPanel>().originMainPos);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"合成公式校验异常：{e.Message}");
            return null;
        }
    }
    #endregion

    #region 出牌相关
    /// <summary>
    /// 打出卡牌
    /// </summary>
    public void ReleaseCard(BaseCard nowCard, Cell cell)
    {
        if (nowCard == null)
            return;

        if ((!nowCard.isRightMouseButtonCliking) && nowCard.isLeftMouseButtonCliking)
            Debug.Log("卡牌打出");

        //播放玩家攻击动画
        effectControl.PlayAtk();
        //播放卡牌释放效果
        DrawLineMgr.Instance.ExitDrawing();
        nowCard.cardEffectControl.PlayReleaseAnimation();
        //创建网格生成范围
        List<Cell> cellslist = GridMgr.Instance.CreatCheckRange(cell, nowCard);
        //判定卡牌的类型
        if (nowCard.isPlaceCard)
        {
            for (int i = 0; i < cellslist.Count; i++)
            {
                LevelArchitect.Instance.PlaceDefTower(nowCard.MyDefTowerResName, cellslist[i]);
            }
        }
        else
        {
            //临时表,设置怪物是否能被赋予效果(不能被同一张牌赋予多次效果)
            List<BaseMonster> tempCellsList = new List<BaseMonster>();
            BaseMonster monster = null;
            for (int i = 0; i < cellslist.Count; i++)
            {
                monster = cellslist[i].nowObj as BaseMonster;
                if (monster != null)
                {
                    if (monster.isAllowedEffected)
                    {
                        tempCellsList.Add(monster);
                        Debug.Log($"[赋予卡牌效果]对{monster.gameObject.name}造成了卡牌效果");
                        nowCard.AddEffectAt?.Invoke(monster, cell);
                        monster.isAllowedEffected = false;
                        monster.TakeDamage(nowCard.currentAtk, nowCard.skill);
                    }
                }
            }
            
            //重置受到效果状态
            for (int i = 0; i < tempCellsList.Count; i++)
            {
                monster = tempCellsList[i];
                if (monster != null)
                {
                    monster.isAllowedEffected = true;
                }
            }
        }

        Dealer.Instance.RemoveCard(nowCard);
        nowSelectedCard = null;
    }
    #endregion

    #region 格子相关
    /// <summary>
    /// 更新预选中组块
    /// </summary>
    public void UpdatePreSlectedCellList(Cell cell)
    {
        if (nowSelectedCard == null) return;
        if (!nowSelectedCard.isLeftMouseButtonCliking) return;

        preSlectedCellList = GridMgr.Instance.CreatCheckRange(cell, nowSelectedCard);
    }

    /// <summary>
    /// 清空预选组块表
    /// </summary>
    public void ClearPreSlectedCellAndList()
    {
        preSlectedCellList.Clear();
        preSlectedCell = null;
    }
    #endregion

    #region 重置操作
    /// <summary>
    /// 重置卡牌操作状态（用于退出状态时）
    /// </summary>
    public void ResetCardOperation()
    {
        if (nowSelectedCard != null)
            nowSelectedCard.cardEffectControl.ForceUnlockAndReturn();
        for (int i = 0; i < CardCompositeList.Count; i++)
        {
            if (CardCompositeList[i] != null)
                CardCompositeList[i].cardEffectControl.ForceUnlockAndReturn();
        }
        rightMouseButtonClikCount = 0;
        CardCompositeList.Clear();
        nowSelectedCard = null;
        ClearPreSlectedCellAndList();
    }
    #endregion
}
