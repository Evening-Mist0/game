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

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void EnterHighLight()
    {
        image.color = Color.green;
    }

    public void ExitHightLight()
    {
        image.color = Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!LevelStepMgr.Instance.ComfirNowStateType(E_LevelState.PlayerTurn_CardOperate))
            return;


        CardOperateState state = LevelStepMgr.Instance.ReturnNowState() as CardOperateState;
        Debug.Log("判断为OperateCardState状态，进行获取当前是否允许高亮" + state.isAllowedCellHighLight);

        if (state.isAllowedCellHighLight)    
            EnterHighLight();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ExitHightLight();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("取消选择高亮");
        ExitHightLight();
    }


}
