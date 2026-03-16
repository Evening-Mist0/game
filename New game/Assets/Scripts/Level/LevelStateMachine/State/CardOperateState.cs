using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

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
    //鼠标预选中的格子
    public Cell preSlectedCell;
    //鼠标选中格子所辐射到的其他格子
    public List<Cell> preSlectedCellList = new List<Cell>();
    //是否允许鼠标悬停在格子上高亮
    public bool isAllowedCellHighLight;
    //全局表，节约性能,检测UI点击
    private static List<RaycastResult> reusableResults = new List<RaycastResult>();



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
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("检测到物体");
                GameObject hoverObj = GetFirstHoveredUI();
                if (hoverObj!=null && hoverObj.CompareTag("LogicalGrid"))
                {
                    Cell nowClickCell = hoverObj.GetComponent<Cell>();
                    if (nowClickCell != null)
                    {
                        Debug.Log($"点击位置：{nowClickCell.logicalPos.x} {nowClickCell.logicalPos.y}");
                        ReleaseCard(this.nowSelectedCard, nowClickCell);
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

        //// 取消卡片的右键选中状态
        //TypeSafeEventCenter.Instance.Trigger<CardRightSelectEvent>(new CardRightSelectEvent(card, false));
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
   /// 合成卡片
   /// </summary>
    public void CompositeCard()
    {
        Debug.Log($"开始合成判断，当前列表数量{CardCompositeList.Count}");

        if (CardCompositeList.Count != 2)
        {
            Debug.Log("合成条件不满足（非2张卡牌），终止合成");
            return;
        }

        BaseCard newCard = TryCompositeCurrentCard();

        if (newCard != null) // 合成成功
        {
            Debug.Log($"合成成功，新卡牌：{newCard.cardID}");

            // 1. 先缓存旧卡牌列表，避免遍历中修改原集合
            List<BaseCard> tempOldCards = new List<BaseCard>(CardCompositeList);
            // 2. 先清空合成列表（修改集合操作提前完成）
            RemoveAllCardInCompositeList();

            // 3. 销毁旧卡牌（此时合成列表已清空，无遍历冲突）
            foreach (var oldCard in tempOldCards)
            {
                if (oldCard != null)
                {
                    oldCard.isRightMouseButtonCliking = false; // 先取消状态
                    oldCard.DestroyMe(); // 销毁卡牌（避免事件触发时集合未稳定）
                }
            }

            // 4. 最后触发事件（所有集合/对象修改完成后）
            Dealer.Instance.AddCard(newCard);
            TypeSafeEventCenter.Instance.Trigger<CardCompositeSuccessEvent>(new CardCompositeSuccessEvent(newCard));
        }
        else // 合成失败
        {
            Debug.Log("合成失败，无匹配的合成方式");

            foreach (var card in CardCompositeList)
            {
                if (card != null)
                {
                    card.isRightMouseButtonCliking = false;
                    //先记录再批量触发
                    TypeSafeEventCenter.Instance.Trigger<CardCancelOhterRightSelectEvent>(new CardCancelOhterRightSelectEvent(card));
                }
            }

            RemoveAllCardInCompositeList(); // 统一移到最后修改集合
        }
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
                return Dealer.Instance.CreateAndAddCard(formula.resultResName,UIMgr.Instance.GetPanel<CardPlayingPanel>().originMainPos);
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
    /// <param name="nowCard">当前选择的卡牌</param>
    /// <param name="cell">当前选中的单元格</param>
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
        List<Cell> cellslist = GridMgr.Instance.CreatCheckRange(cell, nowCard);
        //创建临时表，用于恢复怪物isEffected的状态
        List<BaseMonster> tempCellsList = new List<BaseMonster>();
        //遍历范围的格子，检查格子上是否有怪物，如果有对怪物进行攻击
        BaseMonster monster = null;
        for (int i = 0; i < cellslist.Count;i++)
        {
             monster = cellslist[i].nowObj as BaseMonster;
            //Debug.Log($"[检查范围表格子个数]当前格子的坐标为{cellslist[i].logicalPos.x}{cellslist[i].logicalPos.y}");
            if (monster != null)
            {
                if(monster.isAllowedEffected)
                {
                    tempCellsList.Add(monster);
                    //赋予怪物该卡牌效果
                    Debug.Log($"[赋予卡牌效果]对{monster.gameObject.name}造成了卡牌效果");
                    nowCard.AddEffectAt?.Invoke(monster,cell);
                    monster.isAllowedEffected = false;
                    //怪物受到伤害
                    monster.TakeDamage(nowCard.currentAtk);
                }           
            }        
        }
        //将怪物设置为可以附着效果的状态
        for (int i = 0; i < tempCellsList.Count; i++)
        {
            monster = tempCellsList[i];
            if (monster != null)
            {
                monster.isAllowedEffected = true;
                
            }
        }

            //删除卡牌实例
            nowCard.DestroyMe();
        nowCard = null;
    }
    #endregion


    #region 格子相关
    
    /// <summary>
    /// 鼠标点击获得当前点击到的第一个UI
    /// </summary>
    private GameObject GetFirstHoveredUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        reusableResults.Clear(); // 每个脚本自己清空自己的
        EventSystem.current.RaycastAll(eventData, reusableResults);

        return reusableResults.Count > 0 ? reusableResults[0].gameObject : null;
    }

    /// <summary>
    /// 更新预选中组块
    /// </summary>
    /// <param name="cell">当前组块的核心单元格(基于该单元格进行辐射获取其他单元格)</param>
    public void UpdatePreSlectedCellList(Cell cell)
    {
        //检查当前卡牌是否为空
        if (nowSelectedCard == null) return;
        //检查当前卡牌是否被左键点击
        if (!nowSelectedCard.isLeftMouseButtonCliking) return;

        preSlectedCellList = GridMgr.Instance.CreatCheckRange(cell, nowSelectedCard);
    }

    /// <summary>
    /// 清空预选组块表,置空预选单元格
    /// </summary>
    public void ClearPreSlectedCellAndList()
    {
        preSlectedCellList.Clear();
        preSlectedCell = null;
    }
    #endregion
}