
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 玩家类，游戏核心角色实现
/// </summary>
[RequireComponent(typeof(PlayerEffectControl))]
public class GamePlayer : BaseGameObject
{
    private static GamePlayer instance;

    public static GamePlayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GamePlayer>();
                if (instance == null)//如果不在创建对象
                {                   
                    //获取位置
                    Vector3 spawnPos = Vector3.zero;
                    if (LevelStepMgr.Instance != null && LevelStepMgrSO.Instance.playerPos != null)
                    {
                        Debug.Log("读取道玩家的位置为" + LevelStepMgrSO.Instance.playerPos);
                        spawnPos = LevelStepMgrSO.Instance.playerPos;
                        //instance.gameObject.transform.position = spawnPos;
                    }

                }
                else//如果在复用对象
                {
                    Debug.Log($"使用场景中已有的 {typeof(GamePlayer).Name}");
                }
            }
            return instance;
        }
    }


    public override E_GameObjectType gameObjectType => E_GameObjectType.Player;

    [Tooltip("最大生命值")]
    public int maxHp => GrowthMgr.Instance.growthData.playerMaxHp;

    [Tooltip("当前生命值")]
    public int currentHp => GrowthMgr.Instance.growthData.playerCurrentHp;

    // 玩家实时拥有的防御值 - 从GrowthMgr读取
    public int currentDef => GrowthMgr.Instance.growthData.playerCurrentArmor;
    //每回合额外获得的护甲（执照系统）
    public int extraDef = 0;

    //最大墨水值
    public int maxInkValue;
    //玩家每回合笔墨的增长数量
    public int inkGrowValue;
    //玩家当前拥有的笔墨数量
    public int currentInkValue;

    // 玩家治疗效果的持续回合数
    private int healLastCount;
    // 玩家每回合可获得的治疗数值
    private int nowHealValue;

    // 是否已经触发死亡逻辑
    public bool isDead;
    public PlayerEffectControl effectControl;
    public PlayerBag playerBag;

    public SpriteRenderer sr;

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
        // 如果已经有实例且不是自己，销毁自己
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);


        effectControl = GetComponent<PlayerEffectControl>();
        sr = GetComponent<SpriteRenderer>();
        playerBag = GetComponent<PlayerBag>();
    }

    private void Start()
    {
        // 更新防御/生命值UI
        effectControl.UpdateSpriteDef(currentDef);
        effectControl.UpdateSpriteBlood(currentHp, maxHp);
    }

    /// <summary>
    /// 玩家受到伤害
    /// </summary>
    /// <param name="value">受到的伤害值</param>
    /// <param name="isTrueDamage">是否为真实伤害</param>
    public void Hurt(int value, bool isTrueDamage = false)
    {
        Debug.Log("玩家受到伤害" + value);

        // 调用 GrowthMgr 处理伤害
        GrowthMgr.Instance.PlayerTakeDamage(value, isTrueDamage);

        // 更新防御/生命值UI（GrowthMgr内部已经触发事件，但为了保险再刷一次）
        effectControl.PlayerHurt(value, currentHp, maxHp, currentDef);

        if (currentHp <= 0 && (isDead == false))
        {
            isDead = true;
            Debug.Log("[游戏结束]玩家游戏失败");
            effectControl.PlayDead();
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.LevelLose);
        }
    }

    /// <summary>
    /// 玩家获得治疗效果（持续恢复）
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

        GrowthMgr.Instance.AddArmor(value);

        // 更新防御UI
        effectControl.UpdateSpriteDef(currentDef);
    }

    /// <summary>
    /// 出牌回合结束调用
    /// </summary>
    public void OnRound()
    {
        Debug.Log("玩家治疗结算，剩余治疗回合：" + healLastCount);
        if (healLastCount > 0)
        {
            // 调用 GrowthMgr 恢复血量
            GrowthMgr.Instance.PlayerRecoverHp(nowHealValue);

            healLastCount--;
            // 更新图标显示回合数
            effectControl.UpdateIconCount(E_BuffIconType.Heal, healLastCount);

            // 治疗回合结束，重置治疗效果
            if (healLastCount <= 0)
            {
                // 消除图标
                effectControl.RemoveBuffIcon(E_BuffIconType.Heal);
                nowHealValue = 0;
            }
            // 更新生命值UI
            effectControl.UpdateSpriteBlood(currentHp, maxHp);
        }

        //更新笔墨值
        AddInkWithGrowInk();
        
    }

    /// <summary>
    /// 清空防御值（回合结束时调用）
    /// </summary>
    public void ClearDef()
    {
        GrowthMgr.Instance.OnRoundEndClearArmor();
        // 更新防御UI
        effectControl.UpdateSpriteDef(currentDef);
    }

    /// <summary>
    /// 更新防御UI显示
    /// </summary>
    public void UpdateDef() => effectControl.UpdateSpriteDef(currentDef);

    public void UpdateBlood() => effectControl.UpdateSpriteBlood(currentHp,maxHp);

    /// <summary>
    /// 更新墨水UI显示（当前墨水值/最大墨水值）
    /// </summary>
    public void AddInk(int value)
    {
        //添加笔墨
        currentInkValue += value;
        //如果笔墨达到最大值，弹出兑换界面
        if (currentInkValue >= maxInkValue)
            UIMgr.Instance.ShowPanel<InkExchangePanel>();
        //更新UI
        effectControl.UpdateInkValue(currentInkValue, maxInkValue);
      
    }

    /// <summary>
    /// 更新每回合增长的墨水数量
    /// </summary>
    public void AddInkWithGrowInk()
    {
         AddInk(inkGrowValue);
         Debug.Log("每回合增长的墨水数量为" + inkGrowValue);

    }
    
    /// <summary>
    /// 重置笔墨值,每次战斗开始调用
    /// </summary>
    public void ResetInkValue()
    {
        currentInkValue = 0;
        effectControl.UpdateInkValue(currentInkValue, maxInkValue); 
    }


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
        if(!CardCompositeList.Contains(card))
        {
            Debug.Log("[合成bug检测]尝试移除卡牌" + card.cardID+"但是已经不存在合成表中");
        }
        if (card == null || !CardCompositeList.Contains(card)) return;

        card.isRightMouseButtonCliking = false;
        CardCompositeList.Remove(card);
        Debug.Log($"[合成bug检测]移除卡牌[{card.cardID}]，合成列表剩余：{CardCompositeList.Count}");
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
        //Debug.Log("[合成bug检测]清空合成表，当前的合成表容量为" + CardCompositeList.Count);
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

            //遍历玩家背包在卡牌合成成功时的效果
            playerBag.OnSynthesis(newCard);
            //进行连击判定
            ComboMgr.Instance.JudgementPlayCompositeCombo(newCard.comboData);

            List<BaseCard> tempOldCards = new List<BaseCard>(CardCompositeList);

            foreach (var oldCard in tempOldCards)
            {
                if (oldCard != null)
                {
                    oldCard.isRightMouseButtonCliking = false;
                    Dealer.Instance.RemoveCard(oldCard);
                }
            }

            RemoveAllCardInCompositeList();


            TypeSafeEventCenter.Instance.Trigger<CardCompositeSuccessEvent>(new CardCompositeSuccessEvent(newCard));
            var callback = UIMgr.Instance.GetPanel<CardPlayingPanel>().mainCallBack;
            if (callback != null) callback.MarkLayoutDirty();
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

        // 播放玩家攻击动作
        effectControl.PlayAtk();
        // 关闭卡牌绘制线效果
        DrawLineMgr.Instance.ExitDrawing();
        nowCard.cardEffectControl.PlayReleaseAnimation();

        //触发奇物打出卡牌的技能效果    
        GamePlayer.instance.playerBag.OnPlay(nowCard);
        //触发典籍效果
        GamePlayer.instance.playerBag.BookOnPlay(nowCard);
        // 生成卡牌作用范围
        List<Cell> cellslist = GridMgr.Instance.CreatCheckRange(cell, nowCard);
        //记录卡牌连击数据,更新combo显示
        ComboMgr.Instance.JudgementPlayCardCombo(nowCard.comboData);



        // 判断卡牌类型
        if (nowCard.cardPlayType == E_CardPlayType.Place)//放置类卡牌
        {
            for (int i = 0; i < cellslist.Count; i++)
            {
                BasePlaceCard placeCard = nowCard as BasePlaceCard;
                if (placeCard != null)
                {
                    //触发奇物对于放置物的效果
                    GamePlayer.instance.playerBag.OnCreateDefTower(placeCard);

                    EffectCreater.Instance.CreatEffect(placeCard.attackEffectType, cellslist[i]);
                    LevelArchitect.Instance.PlaceDefTower(placeCard.myDefTowerResName, cellslist[i], placeCard.currentExtraDefTowerHp);
                }
            }
        }
        else//效果类卡牌
        {
           

            if (nowCard.cardRangeType == E_CardRangeType.MySelf)//卡牌作用于自身
            {
                nowCard.AddEffectAt?.Invoke(null, cell);
            }
            else//卡牌作用于网格
            {
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
                                    for (int j = 0; j < nowCard.skills.Count; j++)
                                    {
                                        if (nowCard.skills[j].cardSkill == E_CardSkill.TrueDamage)
                                            coundTakeDamage = false;
                                    }

                                    if (coundTakeDamage)
                                        monster.TakeDamage(nowCard.currentAtk, nowCard.elementType, E_AtkType.CardAtk, false);
                                }
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

        //打出后前置弹回
        nowCard.cardEffectControl.ForceUnlockAndReturn();

        //重置肉鸽数据
        nowCard.ResetMe();

        //触发奇物效果
        GamePlayer.instance.playerBag.OnPlayFinish(nowCard);

        //移除卡牌
        if (nowCard.isUseDestroy)
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

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }


    /// <summary>
    /// 在关卡外隐藏自己
    /// </summary>
    public void HideMe()
    {
        sr.enabled = false;
        effectControl.bloodControl.gameObject.SetActive(false);
        effectControl.buffControl.gameObject.SetActive(false); 
    }

    public void ShowMe()
    {
        sr.enabled = true;
        effectControl.bloodControl.gameObject.SetActive(true);
        effectControl.buffControl.gameObject.SetActive(true);

    }

    /// <summary>
    /// 重置状态
    /// </summary>
    public void ResetMe()
    {
        effectControl.RestAnimator();
        isDead = false;
    }
}