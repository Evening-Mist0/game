using UnityEngine;
using UnityEngine.UI;

public class CardHighlight : MonoBehaviour
{
    private Canvas _tempCanvas;
    private int _originalSortingOrder;

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

    // 设置到指定层级（悬停或选中）
    public void SetTop(int order)
    {
        _tempCanvas.sortingLayerID = SortingLayer.NameToID("Map");
        _tempCanvas.sortingOrder = order;
        Debug.Log($"卡牌{gameObject.name}设置层级 Order={order}");
    }

    // 悬停专用（建议 50）
    public void SetTopAsHover()
    {
        SetTop(50);
    }

    // 选中专用（建议 100）
    public void SetTopAsSelected()
    {
        SetTop(100);
    }

    // 恢复原始层级
    public void ResetTop()
    {
        _tempCanvas.sortingLayerID = SortingLayer.NameToID("Default");
        _tempCanvas.sortingOrder = _originalSortingOrder;
        Debug.Log($"卡牌{gameObject.name}重置层级 Order={_originalSortingOrder}");
    }

    public void SetOriginalSortingOrder(int order)
    {
        _originalSortingOrder = order;
        if (_tempCanvas != null)
            _tempCanvas.sortingOrder = order;
    }
}