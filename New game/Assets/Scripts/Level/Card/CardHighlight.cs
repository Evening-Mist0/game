using UnityEngine;
using UnityEngine.UI;

public class CardHighlight : MonoBehaviour
{
    private Canvas _tempCanvas;
    private int _originalSortingOrder;

    //记录的当前位置的order
    private int currentOrder;
  

    void Awake()
    {
        _tempCanvas = gameObject.AddComponent<Canvas>();
        gameObject.AddComponent<GraphicRaycaster>();
    }

    void Start()
    {
        _tempCanvas.overrideSorting = true;
        _tempCanvas.sortingOrder = _originalSortingOrder;
    }

   

   

    // 恢复原始层级
    public void ResetTop()
    {
        _tempCanvas.sortingOrder = _originalSortingOrder;
        Debug.Log($"卡牌{gameObject.name}重置层级 Order={_originalSortingOrder}");
    }

    public void SetTop()
    {
        _tempCanvas.sortingOrder = 100;
    }

    public void SetOriginalSortingOrder(int order)
    {
        _originalSortingOrder = order;
        Debug.Log("设置卡牌层级为" + order);
        if (_tempCanvas != null)
            _tempCanvas.sortingOrder = order;
    }
}