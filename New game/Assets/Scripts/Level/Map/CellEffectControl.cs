using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Cell)), RequireComponent(typeof(Image)), RequireComponent(typeof(CellEventTrigger))]
public class CellEffectControl : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    //UI图片，用于表现效果
    Image image;
    //是否允许高亮（只有在左键选择卡牌时才能高亮）
    public bool isAllowedHighLight;
    //自身Cell脚本
    Cell myCell;

    private void Awake()
    {
        image = GetComponent<Image>();
        myCell = GetComponent<Cell>();
    }

    /// <summary>
    /// 自身进入高亮
    /// </summary>
    public void EnterHighLight()
    {
        image.color = Color.green;
    }

    /// <summary>
    /// 自身及辐射范围进入高亮
    /// </summary>
    /// <param name="cellList">需要进入高亮的单元格表</param>
    public void EnterHighLight(List<Cell> cellList)
    {
        if (cellList == null) return;
        for(int i = 0; i < cellList.Count;i++)
        {
            cellList[i].myUIControl.EnterHighLight();
        }
    }
    

    /// <summary>
    /// 自身退出高亮
    /// </summary>
    public void ExitHightLight()
    {
        image.color = Color.white;
    }

    /// <summary>
    /// 自身及辐射范围退出高亮
    /// </summary>
    public void ExitHightLight(List<Cell> cellList)
    {
        if (cellList == null) return;
        for (int i = 0; i < cellList.Count; i++)
        {
            cellList[i].myUIControl.ExitHightLight();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!LevelStepMgr.Instance.ComfirNowStateType(E_LevelState.PlayerTurn_CardOperate))
            return;


        CardOperateState state = LevelStepMgr.Instance.ReturnNowState() as CardOperateState;
        Debug.Log("判断为OperateCardState状态，进行获取当前是否允许高亮" + state.isAllowedCellHighLight);
        state.preSlectedCell = myCell;
        state.UpdatePreSlectedCellList(myCell);
        if (state.isAllowedCellHighLight)
        {
            //范围高亮
            EnterHighLight(state.preSlectedCellList);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CardOperateState state = LevelStepMgr.Instance.ReturnNowState() as CardOperateState;

        //范围退出高亮
        ExitHightLight(state.preSlectedCellList);
        state.ClearPreSlectedCellAndList();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        CardOperateState state = LevelStepMgr.Instance.ReturnNowState() as CardOperateState;
        Debug.Log("取消选择高亮");
        ExitHightLight(state.preSlectedCellList);
        state.ClearPreSlectedCellAndList();
    }


}
