using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor.Experimental.GraphView;
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

    [Tooltip("玩家最大血量")]
    public int maxHp;

    private int currentHp;

    //玩家实时持有的护甲
    public int nowDef;
    //玩家可治愈的回合数
    private int healLastCount;
    //玩家当局可以得到治愈的总量
    private int nowHealValue;

    public PlayerEffectControl effectControl;




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
        currentHp = maxHp;
        
    }

    private void Start()
    {
        //更新血条
        effectControl.bloodControl.UpdateBlood(currentHp);
    }


    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="value">受到的伤害值</param>
    /// <param name="isTrueDemage">是否为真伤</param>
    public void Hurt(int value,bool isTrueDemage = false)
    {
        effectControl.PlayerHurt();
        Debug.Log("玩家受到伤害" + value);
        if(isTrueDemage)
        {
            currentHp -= value;
            Debug.Log("[玩家受伤]玩家受到真伤");
        }
        else
        {
            //护甲抵挡
            int overDamage = value - nowDef;

            if (overDamage <= 0)
            {
                // 护甲足够，完全抵挡
                nowDef -= value;
                Debug.Log("[玩家受伤] 护甲完全抵挡伤害，剩余护甲：" + nowDef);
            }
            else
            {
                // 护甲被击穿，剩余伤害扣血
                nowDef = 0;
                currentHp -= overDamage;
                Debug.Log("[玩家受伤] 护甲被击穿，实际受到伤害：" + overDamage);
            }
        }
        //更新血条
        effectControl.bloodControl.UpdateBlood(currentHp);

        if (currentHp <= 0)
        {
            Debug.Log("[游戏结算]玩家游戏失败");
            effectControl.PlayDead();
        }

    }

    /// <summary>
    /// 玩家得到治愈
    /// </summary>
    /// <param name="value">治愈值</param>
    /// <param name="lastCount">治愈持续回合数</param>
    public void GetHeal(int value,int lastCount)
    {
        if(healLastCount <= lastCount)
            healLastCount = lastCount;

        nowHealValue = value;
        Debug.Log("玩家得到每回合治愈值" + nowHealValue);
    }

    /// <summary>
    /// 获得护甲
    /// </summary>
    /// <param name="value"></param>
    public void GetDef(int value)
    {
        if (value < 0)
            return;
        nowDef += value;
    }

    public void OnRound()
    {

        //回合进入回合更新状态，移除玩家的护甲
        nowDef = 0;
        Debug.Log("进入回合更新，可治愈回合数为" + healLastCount);
        if(healLastCount > 0)
        {
            currentHp += nowHealValue;
            if(currentHp > maxHp)
                currentHp = maxHp;
            healLastCount--;
            //治愈回合结束清空回复量
            if (healLastCount <= 0)
                nowHealValue = 0;

            //更新血条
            effectControl.bloodControl.UpdateBlood(currentHp);
        }

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
        if (nowCard.isPlaceCard)//检查是否为放置卡
        {
            for (int i = 0; i < cellslist.Count; i++)
            {
                LevelArchitect.Instance.PlaceDefTower(nowCard.MyDefTowerResName, cellslist[i]);
            }
        }
        else
        {
            if(nowCard.CardRangeType == E_CardRangeType.MySelf)//检查卡牌是否作用于自身
            {
                nowCard.AddEffectAt?.Invoke(null, cell);
            }
            else//卡牌作用于网格
            { //临时表,设置怪物是否能被赋予效果(不能被同一张牌赋予多次效果)
                List<BaseMonsterCore> tempCellsList = new List<BaseMonsterCore>();
                BaseMonsterCore monster = null;
                for (int i = 0; i < cellslist.Count; i++)
                {
                    monster = cellslist[i].nowObj as BaseMonsterCore;
                    if (monster != null)
                    {
                        if (monster.isAllowedEffected)
                        {
                            tempCellsList.Add(monster);
                            Debug.Log($"[赋予卡牌效果]对{monster.gameObject.name}造成了卡牌效果");
                            nowCard.AddEffectAt?.Invoke(monster, cell);
                            monster.isAllowedEffected = false;
                            monster.TakeDamage(nowCard.currentAtk, nowCard.elementType, nowCard.skill);
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
