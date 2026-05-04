using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class AltarUpgradePanel : BasePanel
{
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Transform bookContainer;
    [SerializeField] private GameObject bookOptionPrefab;
    [SerializeField] private Button confirmBtn;
    [SerializeField] private Button cancelBtn;

    private string nodeId;
    private E_BookType selectedBookType;
    private BookConfig selectedBookConfig;

    public void Init(string nodeId)
    {
        this.nodeId = nodeId;
        descText.text = "你误入了一座祭坛，上面刻画着符文，请选择一本可升级的典籍：";

        // 获取可升级典籍（等级0或1）
        var ownedBooks = GrowthMgr.Instance.growthData.ownedBooks;
        var upgradable = ownedBooks
            .Where(bookType => BookUpgradeMgr.Instance.CanUpgrade(bookType))
            .ToList();

        if (upgradable.Count == 0)
        {
            ShowTip("没有可升级的典籍，祭坛毫无反应");
            Finish();
            return;
        }

        foreach (var bookType in upgradable)
        {
            var cfg = GrowthMgr.Instance.GetBookConfig(bookType);
            if (cfg == null) continue;
            int level = BookUpgradeMgr.Instance.GetUpgradeLevel(bookType);
            GameObject obj = Instantiate(bookOptionPrefab, bookContainer);
            var btn = obj.GetComponent<Button>();
            var text = obj.GetComponentInChildren<TextMeshProUGUI>();
            text.text = $"{cfg.bookName} (等级{level})  消耗2点血量上限  50%概率升级";
            btn.onClick.AddListener(() => OnBookSelected(bookType, cfg));
        }

        confirmBtn.onClick.AddListener(OnConfirm);
        cancelBtn.onClick.AddListener(OnCancel);
        confirmBtn.interactable = false;
    }

    private void OnBookSelected(E_BookType bookType, BookConfig config)
    {
        selectedBookType = bookType;
        selectedBookConfig = config;
        confirmBtn.interactable = true;
        // 可选：高亮选中的选项
    }

    private void OnConfirm()
    {
        if (selectedBookConfig == null) return;

        // 检查血量上限是否足够扣除2点
        if (GrowthMgr.Instance.growthData.playerMaxHp <= 2)
        {
            ShowTip("血量上限不足2点，无法进行仪式");
            Finish();
            return;
        }

        // 扣除2点血量上限
        GrowthMgr.Instance.AddPlayerMaxHp(-2);

        // 50%概率升级
        bool success = Random.Range(0, 100) < 50;
        if (success)
        {
            BookUpgradeMgr.Instance.UpgradeBook(selectedBookType);
            ShowTip($"仪式成功！《{selectedBookConfig.bookName}》升级了！");
        }
        else
        {
            ShowTip($"仪式失败，典籍没有变化，但血量上限已减少。");
        }

        Finish();
    }

    private void OnCancel()
    {
        Finish();
    }

    private void Finish()
    {
        LevelFlowMgr.Instance.CompleteNode(nodeId);
        UIMgr.Instance.HidePanel<AltarUpgradePanel>();
        UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
    }

    private void ShowTip(string msg) 
    {
        UIMgr.Instance.ShowPanel<TipPanel>(E_UILayerType.bottom,(panel) =>
        {
            panel.Init(msg);
        });
    }
}
