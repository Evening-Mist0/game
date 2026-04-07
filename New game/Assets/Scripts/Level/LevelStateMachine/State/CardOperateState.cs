using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 出牌阶段的状态，现仅作为入口调用 PlayerTest 的方法
/// </summary>
public class CardOperateState : BaseLevelState
{
    public override E_LevelState myStateType => E_LevelState.PlayerTurn_CardOperate;

    //属性转发至 PlayerTest 的字段，保持外部兼容性
    public List<BaseCard> cardList => GamePlayer.Instance.cardList;
    public BaseCard nowSelectedCard
    {
        get => GamePlayer.Instance.nowSelectedCard;
        set => GamePlayer.Instance.nowSelectedCard = value;
    }
    public List<BaseCard> CardCompositeList => GamePlayer.Instance.CardCompositeList;
    public int rightMouseButtonClikCount
    {
        get => GamePlayer.Instance.rightMouseButtonClikCount;
        set => GamePlayer.Instance.rightMouseButtonClikCount = value;
    }
    public Cell preSlectedCell
    {
        get => GamePlayer.Instance.preSlectedCell;
        set => GamePlayer.Instance.preSlectedCell = value;
    }
    public List<Cell> preSlectedCellList => GamePlayer.Instance.preSlectedCellList;
    public bool isAllowedCellHighLight
    {
        get => GamePlayer.Instance.isAllowedCellHighLight;
        set => GamePlayer.Instance.isAllowedCellHighLight = value;
    }

    // UI 射线检测缓存（状态内保留）
    private static List<RaycastResult> reusableResults = new List<RaycastResult>();

    public override void EnterState()
    {
        Debug.Log("进入CardOperateState");
        UIMgr.Instance.GetPanel<CardPlayingPanel>().ExitAsh();
    }

    public override void ExitState()
    {
        Debug.Log("退出CardOperateState");
        TypeSafeEventCenter.Instance.Trigger<OnExitCardOperateStateEvent>(new OnExitCardOperateStateEvent());
        UIMgr.Instance.GetPanel<CardPlayingPanel>().EnterAsh();
        DrawLineMgr.Instance.ExitDrawing();          // 画线由状态直接控制
        GamePlayer.Instance.ResetCardOperation();    // 重置玩家卡牌操作
    }

    public override void OnState()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var selectedCard = GamePlayer.Instance.nowSelectedCard;
            if (selectedCard == null)
                return;

            if (!selectedCard.isLeftMouseButtonCliking || selectedCard.isRightMouseButtonCliking)
            {
                Debug.Log($"鼠标点击条件不满足：isLeftMouseButtonCliking={selectedCard.isLeftMouseButtonCliking}, isRightMouseButtonCliking={selectedCard.isRightMouseButtonCliking}");
                return;
            }

            Debug.Log("左键点击可检测物体");
            if (EventSystem.current.IsPointerOverGameObject())
            {
                GameObject hoverObj = GetFirstHoveredUI();
                if (hoverObj != null && hoverObj.CompareTag("LogicalGrid"))
                {
                    Cell nowClickCell = hoverObj.GetComponent<Cell>();
                    if (nowClickCell != null)
                    {
                        Debug.Log($"点击格子：{nowClickCell.logicalPos.x},{nowClickCell.logicalPos.y}");
                        GamePlayer.Instance.ReleaseCard(selectedCard, nowClickCell);
                    }
                    else
                    {
                        Debug.LogWarning("格子没有Cell组件");
                    }
                }
            }
            else
            {
                Debug.Log("没有点击到UI");
            }
        }
    }

    #region 合成相关（转发至 PlayerTest）
    public void AddCardInCompositeList(BaseCard card)
    {
        GamePlayer.Instance.AddCardInCompositeList(card);
    }

    public void RemoveCardInCompositeList(BaseCard card)
    {
        GamePlayer.Instance.RemoveCardInCompositeList(card);
    }

    public void RemoveAllCardInCompositeList()
    {
        GamePlayer.Instance.RemoveAllCardInCompositeList();
    }

    //public void CompositeCard(int newCardPos)
    //{
    //    GamePlayer.Instance.CompositeCard(newCardPos);
    //}

    public void CompositeCard(int newCardPos)
    {
        GamePlayer.Instance.CompositeCard(newCardPos);
    }
    #endregion

    #region 出牌相关（转发至 PlayerTest）
    public void ReleaseCard(BaseCard nowCard, Cell cell)
    {
        GamePlayer.Instance.ReleaseCard(nowCard, cell);
    }
    #endregion

    #region 格子相关（转发至 PlayerTest）
    public void UpdatePreSlectedCellList(Cell cell)
    {
        GamePlayer.Instance.UpdatePreSlectedCellList(cell);
    }

    public void ClearPreSlectedCellAndList()
    {
        GamePlayer.Instance.ClearPreSlectedCellAndList();
    }
    #endregion

    #region UI 辅助
    private GameObject GetFirstHoveredUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;

        reusableResults.Clear();
        EventSystem.current.RaycastAll(eventData, reusableResults);

        return reusableResults.Count > 0 ? reusableResults[0].gameObject : null;
    }
    #endregion
}