using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 出牌阶段的状态，实现所有出牌阶段的相关逻辑  
/// </summary>
public class CardOperateState : BaseLevelState
{
    public override E_LevelState myStateType => E_LevelState.PlayerTurn_CardOperate;


    //当前玩家持有的卡牌
    public List<BaseCard> cardList = new List<BaseCard>();
    //记录现在玩家左键选中的牌，如果要作用于网格，就会读取改变量的卡牌信息，造成效果
    public BaseCard nowSelectedCard;
    //记录现在玩家右键键选中的牌，实现合成效果
    public List<BaseCard> CardCompositeList = new List<BaseCard>(2);
    //记录右键点击的次数
    public int rightMouseButtonClikCount;

    public override void EnterState()
    {
        Debug.Log("进入CardOperateState");
    }

    public override void ExitState()
    {
        Debug.Log("退出CardOperateState");
        //重置数据配置
        rightMouseButtonClikCount = 0;
        CardCompositeList.Clear();
        nowSelectedCard = null;
        //移除监听事件
        //CleanEvents();
    }

    public override void OnState()
    {
        //获得玩家点击的格子
        if (Input.GetMouseButtonDown(0))
        {
            if (nowSelectedCard == null)
            {
                return;
            }

            if (!nowSelectedCard.isLeftMouseButtonCliking || (nowSelectedCard.isRightMouseButtonCliking))
            {
                Debug.Log($"鼠标点击nowSelectedCard.isLeftMouseButtonCliking{nowSelectedCard.isLeftMouseButtonCliking},nowSelectedCard.isLeftMouseButtonCliking{nowSelectedCard.isLeftMouseButtonCliking}");
                return;
            }



            Debug.Log("此次点击判定为可以检测点击到的物体");
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitInfo = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hitInfo)
            {
                Debug.Log("检测到物体");
                if (hitInfo.collider.gameObject.CompareTag("LogicalGrid"))
                {
                    Cell plot = hitInfo.collider.gameObject.GetComponent<Cell>();
                    if (plot != null)
                    {
                        Debug.Log($"点击位置：{plot.logicalPos.x} {plot.logicalPos.y}");
                        ReleaseCard(this.nowSelectedCard, plot);
                    }
                    else
                    {
                        Debug.LogWarning("该格子没有Plot组件");
                    }
                }
            }
            else
            {
                Debug.Log("没有检测到点击");
            }
        }
    }

    #region 合成相关（修复版）
    /// <summary>
    /// 添加卡片到合成列表（增加重复/状态校验）
    /// </summary>
    /// <param name="card">要添加的卡片</param>
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

        // 标记卡片为右键选中状态（需确保BaseCard有该字段）
        card.isRightMouseButtonCliking = true;
        CardCompositeList.Add(card);
        Debug.Log($"添加卡片[{card.cardID}]到合成列表，当前数量：{CardCompositeList.Count}");

        // 数量达标时触发合成判断
        if (CardCompositeList.Count == 2)
        {
            CompositeCard();
        }
    }

    /// <summary>
    /// 从合成列表移除卡片（同步状态）
    /// </summary>
    /// <param name="card">要移除的卡片</param>
    public void RemoveCardInCompositeList(BaseCard card)
    {
        if (card == null || !CardCompositeList.Contains(card)) return;

        // 取消卡片的右键选中状态
        card.isRightMouseButtonCliking = false;
        CardCompositeList.Remove(card);
        Debug.Log($"移除卡片[{card.cardID}]，合成列表剩余：{CardCompositeList.Count}");
    }

    /// <summary>
    /// 清空合成列表（全量同步状态）
    /// </summary>
    public void RemoveAllCardInCompositeList()
    {
        foreach (var card in CardCompositeList)
        {
            if (card != null)
            {
                card.isRightMouseButtonCliking = false; // 批量取消选中
            }
        }
        CardCompositeList.Clear();
        rightMouseButtonClikCount = 0; // 重置右键计数
    }

    /// <summary>
    /// 合成卡片（修复逻辑矛盾+完善状态管理）
    /// </summary>
    public void CompositeCard()
    {
        Debug.Log($"开始合成判断，当前列表数量：{CardCompositeList.Count}");

        // 仅当列表有2张卡片时执行合成
        if (CardCompositeList.Count != 2)
        {
            Debug.Log("合成条件不满足（非2张卡片），终止合成");
            return;
        }

        // 尝试合成
        BaseCard newCard = TryCompositeCurrentCard();

        if (newCard != null) // 合成成功
        {
            Debug.Log($"合成成功，新卡片：{newCard.cardID}");

            //销毁旧卡片并清理状态
            foreach (var oldCard in CardCompositeList)
            {
                if (oldCard != null)
                {
                    oldCard.DestroyMe();
                    oldCard.isRightMouseButtonCliking = false;
                }
            }

            //添加新卡片到管理器
            CardMgr.Instance.AddCard(newCard);

            //触发UI刷新
            TypeSafeEventCenter.Instance.Trigger<CardCompositeSuccessEvent>(new CardCompositeSuccessEvent(newCard));
        }
        else // 合成失败
        {
            Debug.Log("合成失败：无匹配的合成公式");

            //取消选中状态并触发UI提示
            foreach (var card in CardCompositeList)
            {
                if (card != null)
                {
                    card.isRightMouseButtonCliking = false;
                    TypeSafeEventCenter.Instance.Trigger<CardCancelOhterRightSelectEvent>(new CardCancelOhterRightSelectEvent(card));
                }
            }

            //触发合成失败事件
            //TypeSafeEventCenter.Instance.Trigger<CardCompositeFailEvent>();
        }

        //清空合成列表
        RemoveAllCardInCompositeList();
    }

    /// <summary>
    /// 尝试合成当前卡片
    /// </summary>
    /// <returns>合成后的新卡片（失败返回null）</returns>
    private BaseCard TryCompositeCurrentCard()
    {
        try
        {
            string cardID0 = CardCompositeList[0].cardID;
            string cardID1 = CardCompositeList[1].cardID;
            Debug.Log($"校验合成公式：{cardID0} + {cardID1}");

            var tuple = CardSynthesisFormulaTable.Instance.GetSortedCardIdTuple(cardID0, cardID1);
            if (CardSynthesisFormulaTable.Instance.SynthesisDic.TryGetValue(tuple, out var formula))
            {
                return CardMgr.Instance.CreateAndAddCard(formula.resultResName);
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

    #region 合成相关
    ///// <summary>
    ///// 添加卡牌到合成列表
    ///// </summary>
    ///// <param name="card">要添加的卡牌</param>
    //public void AddCardInCompositeList(BaseCard card)
    //{
    //    Debug.Log("当前CardCompositeList.Count" + CardCompositeList.Count);

    //    if (CardCompositeList.Count > 2)
    //        return;


    //    CardCompositeList.Add(card);
    //    CompositeCard();

    //    Debug.Log("CardCompositeList.Count,添加卡片后Count" + CardCompositeList.Count);
    //}

    ///// <summary>
    ///// 将卡牌从合成列表移除
    ///// </summary>
    ///// <param name="card">要移除的卡牌</param>
    //public void RemoveCardInCompositeList(BaseCard card)
    //{
    //    CardCompositeList.Remove(card);
    //}
    ///// <summary>
    ///// 清空合成表
    ///// </summary>
    //public void RemoveAllCardInCompositeList()
    //{
    //    CardCompositeList.Clear();
    //}

    ///// <summary>
    ///// 合成卡牌
    ///// </summary>
    //public void CompositeCard()
    //{
    //    Debug.Log("开始进行合成判定");
            
    //    if (CardCompositeList.Count != 2)
    //        return;

    //    Debug.Log("合成失败");


    //    //判断是否能合成卡牌,合成成功卡牌为BaseCard，合成失败为null
    //    BaseCard newCard = TryCompositeCurrentCard();
    //    Debug.Log("尝试获取卡牌" + newCard);


    //    //int count = CardCompositeList.Count;
    //    //for (int i = 0; i < count; i++)
    //    //{
    //    //    Debug.Log("取消选择" + CardCompositeList[i].name);
    //    //    //UI层面的取消选择
    //    //    TypeSafeEventCenter.Instance.Trigger<CardCancelOhterRightSelectEvent>(new CardCancelOhterRightSelectEvent(CardCompositeList[i]));
    //    //}

    //    if (newCard != null)//合成成功
    //    {
    //        Debug.Log("合成成功");

    //        //清除旧的卡牌
    //        for (int i = CardCompositeList.Count - 1; i >= 0; i--)
    //        {
    //            CardCompositeList[i].DestroyMe();
    //        }
    //        CardCompositeList.Clear();


    //        //添加新的卡牌到手牌
    //        CardMgr.Instance.AddCard(newCard);
    //    }
    //    else//如果合成失败，两张卡牌放回原位
    //    {
    //        Debug.Log("合成失败");

    //        int count = CardCompositeList.Count;
    //        for (int i = 0; i < count; i++)
    //        {
    //            Debug.Log("取消选择" + CardCompositeList[i].name);
    //            //UI层面的取消选择
    //            TypeSafeEventCenter.Instance.Trigger<CardCancelOhterRightSelectEvent>(new CardCancelOhterRightSelectEvent(CardCompositeList[i]));
    //        }
    //    }
    //    //清空表
    //    CardCompositeList.Clear();
    //}


    ///// <summary>
    ///// 进行卡牌合成判断，合成成功返回卡牌，合成失败返回null
    ///// </summary>
    ///// <returns></returns>
    //private BaseCard TryCompositeCurrentCard()
    //{
    //    Debug.Log("尝试合成当前选中卡牌方法进行");
    //    //当前要合成的牌ID
    //    string cardID0 = CardCompositeList[0].cardID;
    //    //与该卡牌进行合成的卡牌ID
    //    string cardID1 = CardCompositeList[1].cardID;

    //    //获得当前ID字典键
    //    Tuple<string, string> tuple = CardSynthesisFormulaTable.Instance.GetSortedCardIdTuple(cardID0, cardID1);
    //    //判定该字典有没有此合成方案
    //    if (CardSynthesisFormulaTable.Instance.SynthesisDic.ContainsKey(tuple))
    //    {
    //        Debug.Log("合成判定成功");

    //        return CardMgr.Instance.CreateAndAddCard(CardSynthesisFormulaTable.Instance.SynthesisDic[tuple].resultResName);
    //    }

    //    Debug.Log("合成判定失败");
    //    return null;
    //}
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


        //取消画线
        DrawLineMgr.Instance.ExitDrawing();
        //播放卡牌打出特效
        nowCard.cardEffectControl.PlayReleaseAnimation();
        //创建检查范围
        GridMgr.Instance.CreatCheckRange(cell, nowCard);
        //删除卡牌实例
        nowCard.DestroyMe();
        nowCard = null;
    }
    #endregion
}
