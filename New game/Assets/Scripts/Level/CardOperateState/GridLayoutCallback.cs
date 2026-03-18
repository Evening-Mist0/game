using UnityEngine;
using UnityEngine.UI;

public class GridLayoutCallback : MonoBehaviour
{
    private GridLayoutGroup gridLayout;
    private Vector2 lastCellSize;
    private Vector2 lastSpacing;

    public System.Action OnGridLayoutUpdated;

    void Awake()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        if (gridLayout == null)
        {
            Debug.LogError($"[{gameObject.name}] 未找到 GridLayoutGroup 组件！");
            return;
        }

        lastCellSize = gridLayout.cellSize;
        lastSpacing = gridLayout.spacing;
    }

    void Update()
    {
        if (gridLayout == null) return;

        // 只要 cellSize 或 spacing 变了，就说明 GridLayout 更新了
        if (gridLayout.cellSize != lastCellSize || gridLayout.spacing != lastSpacing)
        {
            lastCellSize = gridLayout.cellSize;
            lastSpacing = gridLayout.spacing;

            OnGridLayoutUpdated?.Invoke();
        }
    }

    void OnDestroy()
    {
        OnGridLayoutUpdated = null;
    }
}