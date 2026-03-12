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

    #region 合成相关
    /// <summary>
    /// 添加卡牌到合成列表
    /// </summary>
    /// <param name="card">要添加的卡牌</param>
    public void AddCardInCompositeList(BaseCard card)
    {
        Debug.Log("当前CardCompositeList.Count" + CardCompositeList.Count);

        if (CardCompositeList.Count > 2)
            return;


        CardCompositeList.Add(card);
        CompositeCard();

        Debug.Log("CardCompositeList.Count,添加卡片后Count" + CardCompositeList.Count);
    }

    /// <summary>
    /// 将卡牌从合成列表移除
    /// </summary>
    /// <param name="card">要移除的卡牌</param>
    public void RemoveCardInCompositeList(BaseCard card)
    {
        CardCompositeList.Remove(card);
    }
    /// <summary>
    /// 清空合成表
    /// </summary>
    public void RemoveAllCardInCompositeList()
    {
        CardCompositeList.Clear();
    }

    /// <summary>
    /// 合成卡牌
    /// </summary>
    public void CompositeCard()
    {
        Debug.Log("开始进行合成判定");
            
        if (CardCompositeList.Count != 2)
            return;

        Debug.Log("合成失败");


        //判断是否能合成卡牌,合成成功卡牌为BaseCard，合成失败为null
        BaseCard newCard = TryCompositeCurrentCard();
        Debug.Log("尝试获取卡牌" + newCard);


        //int count = CardCompositeList.Count;
        //for (int i = 0; i < count; i++)
        //{
        //    Debug.Log("取消选择" + CardCompositeList[i].name);
        //    //UI层面的取消选择
        //    TypeSafeEventCenter.Instance.Trigger<CardCancelOhterRightSelectEvent>(new CardCancelOhterRightSelectEvent(CardCompositeList[i]));
        //}

        if (newCard != null)//合成成功
        {
            Debug.Log("合成成功");

            //清除旧的卡牌
            for (int i = CardCompositeList.Count - 1; i >= 0; i--)
            {
                CardCompositeList[i].DestroyMe();
            }
            CardCompositeList.Clear();


            //添加新的卡牌到手牌
            CardMgr.Instance.AddCard(newCard);
        }
        else//如果合成失败，两张卡牌放回原位
        {
            Debug.Log("合成失败");

            int count = CardCompositeList.Count;
            for (int i = 0; i < count; i++)
            {
                Debug.Log("取消选择" + CardCompositeList[i].name);
                //UI层面的取消选择
                TypeSafeEventCenter.Instance.Trigger<CardCancelOhterRightSelectEvent>(new CardCancelOhterRightSelectEvent(CardCompositeList[i]));
            }
        }
        //清空表
        CardCompositeList.Clear();
    }


    /// <summary>
    /// 进行卡牌合成判断，合成成功返回卡牌，合成失败返回null
    /// </summary>
    /// <returns></returns>
    private BaseCard TryCompositeCurrentCard()
    {
        Debug.Log("尝试合成当前选中卡牌方法进行");
        //当前要合成的牌ID
        string cardID0 = CardCompositeList[0].cardID;
        //与该卡牌进行合成的卡牌ID
        string cardID1 = CardCompositeList[1].cardID;

        //获得当前ID字典键
        Tuple<string, string> tuple = CardSynthesisFormulaTable.Instance.GetSortedCardIdTuple(cardID0, cardID1);
        //判定该字典有没有此合成方案
        if (CardSynthesisFormulaTable.Instance.SynthesisDic.ContainsKey(tuple))
        {
            Debug.Log("合成判定成功");

            return CardMgr.Instance.CreateAndAddCard(CardSynthesisFormulaTable.Instance.SynthesisDic[tuple].resultResName);
        }

        Debug.Log("合成判定失败");
        return null;
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
