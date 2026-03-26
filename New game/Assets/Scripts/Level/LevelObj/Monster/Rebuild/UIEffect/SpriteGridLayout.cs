using UnityEngine;
using System.Collections.Generic;
using System.Collections;


#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[DisallowMultipleComponent]
public class SpriteGridLayout : MonoBehaviour
{
    [Header("网格约束")]
    public Constraint constraint = Constraint.Flexible;
    public int constraintCount = 2;

    [Header("单元格大小")]
    public float cellWidth = 100f;
    public float cellHeight = 100f;

    [Header("间距设置")]
    public Vector2 spacing = new Vector2(10f, 10f);
    public RectOffset padding = new RectOffset();

    [Header("排列方向")]
    public Axis startAxis = Axis.Horizontal;

    [Header("对齐方式")]
    public TextAnchor childAlignment = TextAnchor.UpperLeft;

    private List<SpriteRenderer> cachedSprites = new List<SpriteRenderer>();
    private bool needsRefresh = true;

    public enum Constraint { Flexible, FixedColumnCount, FixedRowCount }
    public enum Axis { Horizontal, Vertical }

    void OnEnable()
    {
        needsRefresh = true;
        RefreshLayout();
        // 注册子对象状态监听（仅运行时）
        if (Application.isPlaying)
        {
            StartCoroutine(MonitorChildActiveState());
        }
    }

    void OnDisable()
    {
        // 取消协程监听
        StopAllCoroutines();
    }

    void Update()
    {
        if (needsRefresh) RefreshLayout();
    }

    [ContextMenu("刷新布局")]
    public void RefreshLayout()
    {
        cachedSprites.Clear();

        // 遍历子物体，仅收集【激活】且包含SpriteRenderer的有效子对象
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTransform = transform.GetChild(i);
            // 关键：仅保留激活状态的子对象（失活的直接跳过）
            if (!childTransform.gameObject.activeInHierarchy) continue;

            SpriteRenderer sprite = childTransform.GetComponent<SpriteRenderer>();
            if (sprite != null)
            {
                cachedSprites.Add(sprite);
            }
        }

        // 移除自身的SpriteRenderer（如果有）
        SpriteRenderer selfSprite = GetComponent<SpriteRenderer>();
        if (selfSprite != null && cachedSprites.Contains(selfSprite))
            cachedSprites.Remove(selfSprite);

        if (cachedSprites.Count == 0)
        {
            needsRefresh = false;
            return;
        }

        int columns, rows;
        CalculateGridDimensions(cachedSprites.Count, out columns, out rows);
        if (columns <= 0 || rows <= 0)
        {
            needsRefresh = false;
            return;
        }

        // 计算网格总尺寸（含间距）
        float totalGridWidth = columns * cellWidth + (columns - 1) * spacing.x;
        float totalGridHeight = rows * cellHeight + (rows - 1) * spacing.y;

        // 根据对齐方式计算偏移
        Vector2 alignmentOffset = GetAlignmentOffset(totalGridWidth, totalGridHeight);

        // 起始偏移 = 内边距 + 对齐偏移
        Vector2 startOffset = new Vector2(padding.left, -padding.top) + alignmentOffset;

        // 重新排列所有有效子对象（按顺序填充空缺）
        for (int i = 0; i < cachedSprites.Count; i++)
        {
            SpriteRenderer sprite = cachedSprites[i];
            if (sprite == null) continue;

            int xIndex, yIndex;
            if (startAxis == Axis.Horizontal)
            {
                xIndex = i % columns;
                yIndex = i / columns;
            }
            else
            {
                xIndex = i / rows;
                yIndex = i % rows;
            }

            float posX = startOffset.x + xIndex * (cellWidth + spacing.x);
            float posY = startOffset.y - yIndex * (cellHeight + spacing.y);
            sprite.transform.localPosition = new Vector3(posX, posY, 0);
        }

        needsRefresh = false;
    }

    private void CalculateGridDimensions(int totalCount, out int columns, out int rows)
    {
        columns = 1; rows = 1;
        switch (constraint)
        {
            case Constraint.FixedColumnCount:
                columns = Mathf.Max(1, constraintCount);
                rows = Mathf.CeilToInt((float)totalCount / columns);
                break;
            case Constraint.FixedRowCount:
                rows = Mathf.Max(1, constraintCount);
                columns = Mathf.CeilToInt((float)totalCount / rows);
                break;
            case Constraint.Flexible:
                columns = Mathf.CeilToInt(Mathf.Sqrt(totalCount));
                rows = Mathf.CeilToInt((float)totalCount / columns);
                break;
        }
    }

    private Vector2 GetAlignmentOffset(float totalWidth, float totalHeight)
    {
        Vector2 offset = Vector2.zero;
        switch (childAlignment)
        {
            case TextAnchor.UpperLeft: break;
            case TextAnchor.UpperCenter: offset.x = -totalWidth / 2f; break;
            case TextAnchor.UpperRight: offset.x = -totalWidth; break;
            case TextAnchor.MiddleLeft: offset.y = totalHeight / 2f; break;
            case TextAnchor.MiddleCenter: offset.x = -totalWidth / 2f; offset.y = totalHeight / 2f; break;
            case TextAnchor.MiddleRight: offset.x = -totalWidth; offset.y = totalHeight / 2f; break;
            case TextAnchor.LowerLeft: offset.y = totalHeight; break;
            case TextAnchor.LowerCenter: offset.x = -totalWidth / 2f; offset.y = totalHeight; break;
            case TextAnchor.LowerRight: offset.x = -totalWidth; offset.y = totalHeight; break;
        }
        return offset;
    }

    private void OnValidate()
    {
        cellWidth = Mathf.Max(0.01f, cellWidth);
        cellHeight = Mathf.Max(0.01f, cellHeight);
        spacing.x = Mathf.Max(0f, spacing.x);
        spacing.y = Mathf.Max(0f, spacing.y);
        padding.left = Mathf.Max(0, padding.left);
        padding.right = Mathf.Max(0, padding.right);
        padding.top = Mathf.Max(0, padding.top);
        padding.bottom = Mathf.Max(0, padding.bottom);
        if (constraintCount < 1) constraintCount = 1;
        needsRefresh = true;
#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorApplication.delayCall += () => { if (this != null) RefreshLayout(); };
#endif
    }

    // 子物体数量/层级变更时触发刷新
    void OnTransformChildrenChanged()
    {
        needsRefresh = true;
        if (Application.isPlaying)
            RefreshLayout();
    }

    // 监听子对象激活状态变化（运行时）
    private IEnumerator MonitorChildActiveState()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            // 检查是否有子对象激活状态变化
            bool isChildStateChanged = false;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
                if (sr == null) continue;

                // 缓存中存在但当前失活 → 状态变化
                if (cachedSprites.Contains(sr) && !child.gameObject.activeInHierarchy)
                {
                    isChildStateChanged = true;
                    break;
                }
                // 缓存中不存在但当前激活 → 状态变化
                if (!cachedSprites.Contains(sr) && child.gameObject.activeInHierarchy)
                {
                    isChildStateChanged = true;
                    break;
                }
            }

            if (isChildStateChanged)
            {
                needsRefresh = true;
                RefreshLayout();
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (cachedSprites.Count == 0) return;

        int columns, rows;
        CalculateGridDimensions(cachedSprites.Count, out columns, out rows);
        float totalGridWidth = columns * cellWidth + (columns - 1) * spacing.x;
        float totalGridHeight = rows * cellHeight + (rows - 1) * spacing.y;

        Vector2 alignOffset = GetAlignmentOffset(totalGridWidth, totalGridHeight);
        Vector2 startOffset = new Vector2(padding.left, -padding.top) + alignOffset;
        Vector2 endOffset = startOffset + new Vector2(totalGridWidth, -totalGridHeight);

        Gizmos.color = Color.green;
        Vector3 center = transform.position + new Vector3((startOffset.x + endOffset.x) / 2f, (startOffset.y + endOffset.y) / 2f, 0);
        Vector3 size = new Vector3(Mathf.Abs(endOffset.x - startOffset.x), Mathf.Abs(endOffset.y - startOffset.y), 0);
        Gizmos.DrawWireCube(center, size);
    }
#endif
}