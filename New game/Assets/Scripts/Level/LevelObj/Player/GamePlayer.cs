using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 玩家类，游戏核心角色实现
/// </summary>
[RequireComponent(typeof(PlayerEffectControl))]
public class GamePlayer : BaseGameObject
{
    private static GamePlayer instance;

    public static GamePlayer Instance => instance;

    public override E_GameObjectType gameObjectType => E_GameObjectType.Player;

    [Tooltip("最大生命值")]
    public int maxHp;

    public int currentHp;

    //玩家实时拥有的防御值
    public int nowDef;
    //玩家治疗效果的持续回合数
    private int healLastCount;
    //玩家每回合可获得的治疗数值
    private int nowHealValue;
    //是否已经触发死亡逻辑
    private bool isDead;
    public PlayerEffectControl effectControl;

    // 卡牌操作相关字段，原 CardOperateState 字段
    /// <summary>
    /// 玩家可操作的卡牌列表
    /// </summary>
    public List<BaseCard> cardList = new List<BaseCard>();
    /// <summary>
    /// 当前选中的卡牌
    /// </summary>
    public BaseCard nowSelectedCard;
    /// <summary>
    /// 玩家准备合成的卡牌列表
    /// </summary>
    public List<BaseCard> CardCompositeList = new List<BaseCard>(2);
    public int rightMouseButtonClikCount;
    /// <summary>
    /// 玩家当前选中的格子
    /// </summary>
    public Cell preSlectedCell;
    /// <summary>
    /// 预选中的格子列表（根据卡牌释放范围）
    /// </summary>
    public List<Cell> preSlectedCellList = new List<Cell>();
    /// <summary>
    /// 是否允许格子高亮显示
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
        //更新防御/生命值UI
        effectControl.UpdateSpriteDef(nowDef);
        effectControl.UpdateSpriteBlood(currentHp, maxHp);
    }

    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="value">受到的伤害值</param>
    /// <param name="isTrueDemage">是否为真实伤害</param>
    public void Hurt(int value, bool isTrueDemage = false)
    {
        Debug.Log("玩家受到伤害" + value);
        if (isTrueDemage)
        {
            currentHp -= value;
            Debug.Log("[真实伤害]玩家受到伤害");
        }
        else
        {
            //计算溢出伤害
            int overDamage = value - nowDef;
            Debug.Log($"[普通伤害]伤害值{value}-防御值{nowDef}");
            if (overDamage <= 0)
            {
                // 防御足够，完全抵消伤害
                nowDef -= value;
                Debug.Log("[普通伤害] 防御完全抵消伤害，剩余防御：" + nowDef);
            }
            else
            {
                // 防御被击破，剩余伤害扣除生命值
                nowDef = 0;
                currentHp -= overDamage;
                Debug.Log("[普通伤害] 防御被击破，实际受到伤害：" + overDamage);
            }
        }

        //更新防御/生命值UI
        effectControl.PlayerHurt(value, currentHp, maxHp, nowDef);

        if (currentHp <= 0 && (isDead == false))
        {
            isDead = true;
            Debug.Log("[游戏结束]玩家游戏失败");
            effectControl.PlayDead();
        }
    }

    /// <summary>
    /// 玩家获得治疗效果
    /// </summary>
    /// <param name="value">每回合治疗值</param>
    /// <param name="lastCount">治疗持续回合数</param>
    public void GetHeal(int value, int lastCount)
    {
        if (healLastCount <= lastCount)
            healLastCount = lastCount;

        nowHealValue = value;
        Debug.Log("玩家获得每回合治疗值" + nowHealValue);
    }

    /// <summary>
    /// 玩家获得防御
    /// </summary>
    /// <param name="value">防御值</param>
    public void GetDef(int value)
    {
        if (value < 0)
            return;
        nowDef += value;

        //更新防御UI
        effectControl.UpdateSpriteDef(nowDef);
    }

    public void OnRound()
    {


        Debug.Log("玩家治疗结算，剩余治疗回合：" + healLastCount);
        if (healLastCount > 0)
        {
            currentHp += nowHealValue;
            if (currentHp > maxHp)
                currentHp = maxHp;
            healLastCount--;
            //更新图标显示回合数

            effectControl.UpdateIconCount(E_BuffIconType.Heal, healLastCount);

            //治疗回合结束，重置治疗效果
            if (healLastCount <= 0)
            {
                //消除图标
                effectControl.RemoveBuffIcon(E_BuffIconType.Heal);
                nowHealValue = 0;
            }
            //更新生命值
            effectControl.UpdateSpriteBlood(currentHp, maxHp);
        }
    }

    /// <summary>
    /// 清空防御值（回合结束时调用）
    /// 当前逻辑：每回合开始清空玩家防御
    /// </summary>
    public void ClearDef()
    {
        //回合结束清空玩家的防御
        nowDef = 0;
        //更新防御UI
        effectControl.UpdateSpriteDef(nowDef);
    }

    /// <summary>
    /// 更新防御UI显示
    /// </summary>
    public void UpdateDef() => effectControl.UpdateSpriteDef(nowDef);

    #region 卡牌合成
    /// <summary>
    /// 添加卡牌到合成列表
    /// </summary>
    public void AddCardInCompositeList(BaseCard card)
    {
        if (card == null || CardCompositeList.Contains(card))
        {
            Debug.LogWarning("卡牌为空或已在合成列表中，无法添加");
            return;
        }

        if (CardCompositeList.Count >= 2)
        {
            Debug.LogWarning("合成列表已满（2张），无法添加");
            return;
        }

        card.isRightMouseButtonCliking = true;
        CardCompositeList.Add(card);
        Debug.Log($"添加卡牌[{card.cardID}]到合成列表，当前数量：{CardCompositeList.Count}");

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
    /// 获取合成中另一张卡牌的位置
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
            Debug.LogError("选中的卡牌竟然不在合成列表里，出现异常");
            return 0;
        }
        else
        {
            Debug.LogError("该方法获取无效，合成列表数量不为2，请检查调用逻辑");
            return 0;
        }
    }

    /// <summary>
    /// 从合成列表移除卡牌
    /// </summary>
    public void RemoveCardInCompositeList(BaseCard card)
    {
        if (card == null || !CardCompositeList.Contains(card)) return;

        card.isRightMouseButtonCliking = false;
        CardCompositeList.Remove(card);
        Debug.Log($"移除卡牌[{card.cardID}]，合成列表剩余：{CardCompositeList.Count}");
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
    /// 合成卡牌
    /// </summary>
    public void CompositeCard(int newCardPos)
    {
        Debug.Log($"开始合成检测，当前列表数量：{CardCompositeList.Count}");

        if (CardCompositeList.Count != 2)
        {
            Debug.Log("合成条件不足（需2张卡牌），停止合成");
            return;
        }

        BaseCard newCard = TryCompositeCurrentCard(newCardPos);

        if (newCard != null)
        {
            Debug.Log($"合成成功，生成卡牌：{newCard.cardID}");

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
            var callback = UIMgr.Instance.GetPanel<CardPlayingPanel>().mainCallBack;
            if (callback != null) callback.MarkLayoutDirty(); // 强制触发布局更新
        }
        else
        {
            Debug.Log("合成失败，无匹配的合成公式");

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
    /// 尝试合成当前卡牌
    /// </summary>
    private BaseCard TryCompositeCurrentCard(int newCardPos)
    {
        try
        {
            string cardID0 = CardCompositeList[0].cardID;
            string cardID1 = CardCompositeList[1].cardID;
            Debug.Log($"验证合成公式：{cardID0} + {cardID1}");

            var tuple = CardSynthesisFormulaTable.Instance.GetSortedCardIdTuple(cardID0, cardID1);
            if (CardSynthesisFormulaTable.Instance.SynthesisDic.TryGetValue(tuple, out var formula))
            {
                return Dealer.Instance.CreateAndAddCard(formula.resultResName, newCardPos, UIMgr.Instance.GetPanel<CardPlayingPanel>().originMainPos);
            }
            return null;
        }
        catch (Exception e)
        {
            Debug.LogError($"合成公式验证异常：{e.Message}");
            return null;
        }
    }
    #endregion

    #region 卡牌释放
    /// <summary>
    /// 释放卡牌
    /// </summary>
    public void ReleaseCard(BaseCard nowCard, Cell cell)
    {
        if (nowCard == null)
            return;

        if ((!nowCard.isRightMouseButtonCliking) && nowCard.isLeftMouseButtonCliking)
            Debug.Log("卡牌使用");

        //播放玩家攻击动作
        effectControl.PlayAtk();
        //关闭卡牌绘制线效果
        DrawLineMgr.Instance.ExitDrawing();
        nowCard.cardEffectControl.PlayReleaseAnimation();
        //生成卡牌作用范围
        List<Cell> cellslist = GridMgr.Instance.CreatCheckRange(cell, nowCard);
        //判断卡牌类型
        if (nowCard.cardPlayType == E_CardPlayType.Place)//是否为放置类卡牌
        {
            for (int i = 0; i < cellslist.Count; i++)
            {
                BasePlaceCard placeCard = nowCard as BasePlaceCard;
                if (placeCard != null)
                {
                    EffectCreater.Instance.CreatEffect(placeCard.attackEffectType, cellslist[i]);
                    LevelArchitect.Instance.PlaceDefTower(placeCard.myDefTowerResName, cellslist[i], placeCard.extraDefTowerHp);
                }
            }
        }
        else
        {
            if (nowCard.cardRangeType == E_CardRangeType.MySelf)//卡牌范围是否为自身
            {
                nowCard.AddEffectAt?.Invoke(null, cell);
            }
            else//范围作用目标
            {
                //临时列表，防止同一卡牌重复施加效果
                List<BaseMonsterCore> tempCellsList = new List<BaseMonsterCore>();
                BaseGameObject obj = null;
                for (int i = 0; i < cellslist.Count; i++)
                {
                    EffectCreater.Instance.CreatEffect(nowCard.attackEffectType, cellslist[i]);
                    obj = cellslist[i].nowObj;

                    if (obj == null)
                        continue;

                    Debug.Log("检测到目标对象为空，当前对象类型为" + obj.gameObjectType);

                    switch (obj.gameObjectType)
                    {
                        case E_GameObjectType.Cell:
                        case E_GameObjectType.Player:
                            break;

                        case E_GameObjectType.Monster:
                            BaseMonsterCore monster = obj as BaseMonsterCore;
                            if (monster != null)
                            {
                                if (monster.isAllowedEffected)
                                {
                                    tempCellsList.Add(monster);
                                    Debug.Log($"[卡牌效果]对{monster.gameObject.name}施加效果");
                                    nowCard.AddEffectAt?.Invoke(monster, cell);
                                    monster.isAllowedEffected = false;

                                    bool coundTakeDamage = true;
                                    for (int j = 0; j < nowCard.skills.Count; j++)//遍历技能，判断是否造成伤害
                                    {
                                        if (nowCard.skills[j].cardSkill == E_CardSkill.TrueDamage)
                                            coundTakeDamage = false;
                                    }

                                    if (coundTakeDamage)
                                        monster.TakeDamage(nowCard.currentAtk, nowCard.elementType, E_AtkType.CardAtk, false);
                                }
                                //恢复目标效果施加状态
                                for (int k = 0; k < tempCellsList.Count; k++)
                                {
                                    monster = tempCellsList[k];
                                    if (monster != null)
                                    {
                                        monster.isAllowedEffected = true;
                                    }
                                }
                            }
                            break;
                        case E_GameObjectType.DefTower:
                            Debug.Log("检测到目标对象为防御塔");
                            BaseMonsterCore monster2 = obj as BaseMonsterCore;
                            nowCard.AddEffectAt?.Invoke(monster2, cell);
                            break;
                    }
                }
            }
        }

        Dealer.Instance.RemoveCard(nowCard);
        nowSelectedCard = null;
    }
    #endregion

    #region 格子选中
    /// <summary>
    /// 更新预选中格子列表
    /// </summary>
    public void UpdatePreSlectedCellList(Cell cell)
    {
        if (nowSelectedCard == null) return;
        if (!nowSelectedCard.isLeftMouseButtonCliking) return;

        preSlectedCellList = GridMgr.Instance.CreatCheckRange(cell, nowSelectedCard);
    }

    /// <summary>
    /// 清空预选中格子和列表
    /// </summary>
    public void ClearPreSlectedCellAndList()
    {
        preSlectedCellList.Clear();
        preSlectedCell = null;
    }
    #endregion

    #region 操作重置
    /// <summary>
    /// 重置卡牌操作状态（取消操作时调用）
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