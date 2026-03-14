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
        // 监听子物体变化（自动触发布局更新）
        StartCoroutine(MonitorChildChanges());
    }

    // 自动监听Grid子物体变化（新增/删除/激活状态），无需外部调用MarkLayoutDirty
    private System.Collections.IEnumerator MonitorChildChanges()
    {
        int lastChildCount = transform.childCount;
        while (true)
        {
            yield return null;
            // 子物体数量变化 或 子物体激活状态变化
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
            Debug.Log("布局已重建完成，触发回调");
            OnGridLayoutUpdated?.Invoke();
            _isLayoutDirty = false;
        }
    }

    private void OnDestroy()
    {
        Canvas.willRenderCanvases -= OnCanvasWillRender;
        OnGridLayoutUpdated = null;
    }
}