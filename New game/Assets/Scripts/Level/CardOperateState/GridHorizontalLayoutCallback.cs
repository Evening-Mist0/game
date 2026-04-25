using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(HorizontalLayoutGroup))]  // 改为 HorizontalLayoutGroup
public class GridHorizontalLayoutCallback : MonoBehaviour
{
    public UnityAction OnHorizontalLayoutUpdated;  // 事件名称改为更贴合的水平布局
    private HorizontalLayoutGroup _horizontalLayout; // 类型改为 HorizontalLayoutGroup
    private bool _isLayoutDirty = false;

    private void Awake()
    {
        _horizontalLayout = GetComponent<HorizontalLayoutGroup>(); // 获取对应组件
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
            Debug.Log($"HorizontalLayoutCallback: 触发 OnHorizontalLayoutUpdated，当前子物体数量: {transform.childCount}");
            // 布局更新后，同步所有子卡牌的SortingOrder
            SyncCardSortingOrder();

            OnHorizontalLayoutUpdated?.Invoke();
            // 如需刷新位置可取消下一行注释
            // RefreshAllCardPositions();

            _isLayoutDirty = false;
        }
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

    // 按子物体顺序设置每个卡牌的初始SortingOrder
    private void SyncCardSortingOrder()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            var card = transform.GetChild(i).GetComponent<CardHighlight>();
            if (card != null)
            {
                // 索引i越小，SortingOrder越小（保证先布局的卡牌在下层，后布局的在上层，与水平布局显示顺序一致）
                card.SetOriginalSortingOrder(0);
            }
        }
    }

    private void OnDestroy()
    {
        Canvas.willRenderCanvases -= OnCanvasWillRender;
        OnHorizontalLayoutUpdated = null;
    }
}