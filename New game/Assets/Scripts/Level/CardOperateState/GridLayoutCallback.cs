
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(GridLayoutGroup))]
public class GridLayoutCallback : MonoBehaviour
{
    public UnityAction OnGridLayoutUpdated;
    private GridLayoutGroup _gridLayout;
    private bool _isLayoutDirty = false;

    private void Awake()
    {
        _gridLayout = GetComponent<GridLayoutGroup>();
        Canvas.willRenderCanvases += OnCanvasWillRender;
        StartCoroutine(MonitorChildChanges());
    }

    private System.Collections.IEnumerator MonitorChildChanges()
    {
        int lastChildCount = transform.childCount;
        while (true)
        {
            yield return null;
            bool childCountChanged = transform.childCount != lastChildCount;
            bool childActiveChanged = false;
            if (!childCountChanged)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf != transform.GetChild(i).gameObject.activeInHierarchy)
                    {
                        childActiveChanged = true;
                        break;
                    }
                }
            }

            if (childCountChanged || childActiveChanged)
            {
                MarkLayoutDirty();
                lastChildCount = transform.childCount;
            }
        }
    }

    public void MarkLayoutDirty()
    {
        _isLayoutDirty = true;
        LayoutRebuilder.MarkLayoutForRebuild(transform as RectTransform);
    }

    private void OnCanvasWillRender()
    {
        if (_isLayoutDirty)
        {
            Debug.Log($"GridLayoutCallback: 触发 OnGridLayoutUpdated，当前子物体数量: {transform.childCount}");
            // 布局更新后，同步所有子卡牌的SortingOrder
            SyncCardSortingOrder();

            OnGridLayoutUpdated?.Invoke();
            //RefreshAllCardPositions();

            _isLayoutDirty = false;
        }
        // 主动刷新所有子卡牌的原始位置
    }

    private void RefreshAllCardPositions()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var cardEffect = transform.GetChild(i).GetComponent<CardEffectControl>();
            if (cardEffect != null)
            {
                cardEffect.RefreshOriginalPos();
            }
        }
    }

    // 核心：按Grid子节点顺序，设置每个卡牌的初始SortingOrder
    private void SyncCardSortingOrder()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var card = transform.GetChild(i).GetComponent<CardHighlight>();
            if (card != null)
            {
                // 索引i越小，SortingOrder越小（保证先布局的卡牌在下层，后布局的在上层，和Grid显示一致）
                card.SetOriginalSortingOrder(i);
            }
        }
    }

    private void OnDestroy()
    {
        Canvas.willRenderCanvases -= OnCanvasWillRender;
        OnGridLayoutUpdated = null;
    }
}