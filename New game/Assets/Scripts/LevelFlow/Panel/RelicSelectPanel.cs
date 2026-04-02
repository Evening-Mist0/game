using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class RelicSelectPanel : BasePanel
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Transform contentRoot;
    [SerializeField] private GameObject relicOptionPrefab;
    [SerializeField] private Button closeBtn;

    private Action<RelicConfig> onConfirm;

    protected override void Awake()
    {
        base.Awake();
        if (closeBtn != null) closeBtn.onClick.AddListener(CloseWithoutSelect);
    }

    /// <summary>
    /// 初始化面板
    /// </summary>
    /// <param name="mode">模式（影响标题和提示）</param>
    /// <param name="relicList">奇物列表（已拥有的）</param>
    /// <param name="onConfirm">选中后的回调</param>
    public void Init(E_RelicSelectMode mode, List<RelicConfig> relicList, Action<RelicConfig> onConfirm)
    {
        this.onConfirm = onConfirm;

        switch (mode)
        {
            case E_RelicSelectMode.Sell:
                titleText.text = "出售奇物（白色=1经验，绿色=2，蓝色=3）";
                break;
            case E_RelicSelectMode.Recover:
                titleText.text = "消耗奇物恢复血量（白色=3，绿色=6，蓝色=9）";
                break;
            default:
                titleText.text = "选择奇物";
                break;
        }

        // 清空旧选项
        foreach (Transform child in contentRoot)
            Destroy(child.gameObject);

        // 生成选项
        foreach (var relic in relicList)
        {
            GameObject opt = Instantiate(relicOptionPrefab, contentRoot);
            RelicOptionItem item = opt.GetComponent<RelicOptionItem>();
            item.Init(relic, () => OnRelicSelected(relic));
        }
    }

    private void OnRelicSelected(RelicConfig relic)
    {
        onConfirm?.Invoke(relic);
        ClosePanel();
    }

    private void CloseWithoutSelect()
    {
        ClosePanel();
    }

    private void ClosePanel()
    {
        HideMe();
    }
}


