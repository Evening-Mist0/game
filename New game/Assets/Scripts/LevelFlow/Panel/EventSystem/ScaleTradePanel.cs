using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ScaleTradePanel : BasePanel
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private Button tradeRelicBtn;   // 交易奇物
    [SerializeField] private Button tradeBookBtn;    // 交易典籍
    [SerializeField] private Button closeBtn;        // 放弃交易

    private string nodeId;

    public void Init(string nodeId)
    {
        this.nodeId = nodeId;
        descText.text = "你掉进了一个密室，里面有一件天平奇物，似乎可以和他进行以物易物？\n请选择交易类型：";
        
        tradeRelicBtn.onClick.AddListener(OnTradeRelic);
        tradeBookBtn.onClick.AddListener(OnTradeBook);
        if (closeBtn != null) closeBtn.onClick.AddListener(OnClose);
    }

    /// <summary>
    /// 交易奇物：消耗一件已拥有的奇物，随机获得一件同品阶未拥有的奇物
    /// </summary>
    private void OnTradeRelic()
    {
        // 1. 获取玩家拥有的奇物列表（配置列表）
        var ownedRelics = GetOwnedRelicConfigs();
        if (ownedRelics.Count == 0)
        {
            ShowTip("你没有任何奇物可以交易");
            Finish();
            return;
        }

        // 打开奇物选择面板，让玩家选择要消耗的奇物
        UIMgr.Instance.ShowPanel<RelicSelectPanel>(E_UILayerType.middle, (relicSelectPanel) =>
        {
            relicSelectPanel.Init(E_RelicSelectMode.Sell, ownedRelics, (selectedRelic) =>
           {
               // 2. 根据选中奇物的品质，查找同品质未拥有的奇物
               var quality = selectedRelic.quality;
               var ownedIds = GrowthMgr.Instance.growthData.ownedRelicIds;
               var candidates = GrowthMgr.Instance.relicConfig.relicConfigs
                   .Where(r => r.quality == quality && !ownedIds.Contains(r.relicId))
                   .ToList();

               if (candidates.Count == 0)
               {
                   ShowTip($"没有找到{quality}品质的未拥有奇物，交易失败");
                   // 交易失败，但已消耗奇物？不应该消耗，此处不消耗奇物，直接结束事件
                   Finish();
                  return;
               }
   
               // 3. 随机获得一件奇物
               var newRelic = candidates[Random.Range(0, candidates.Count)];
               // 移除旧奇物
               GrowthMgr.Instance.RemoveRelic(selectedRelic.relicId);
               // 添加新奇物
               GrowthMgr.Instance.AddRelic(newRelic.relicId);
               ShowTip($"消耗《{selectedRelic.relicName}》，获得《{newRelic.relicName}》");
               Finish();
           });
        });
    }

    /// <summary>
    /// 交易典籍：消耗一本已拥有的典籍，然后从两本未拥有的典籍中选择一本获得
    /// </summary>
    private void OnTradeBook()
    {
        // 1. 获取玩家拥有的典籍配置列表
        var ownedBooks = GrowthMgr.Instance.GetOwnedBookConfigs();
        if (ownedBooks.Count == 0)
        {
            ShowTip("你没有任何典籍可以交易");
            Finish();
            return;
        }

        // 打开典籍选择面板，让玩家选择要消耗的典籍
        UIMgr.Instance.ShowPanel<BookChoicePanel>(E_UILayerType.middle);
        var bookSelectPanel = UIMgr.Instance.GetPanel<BookChoicePanel>();
        // 使用单选模式（出售模式）
        bookSelectPanel.Init("请选择要消耗的典籍",ownedBooks, (selectedBook) =>
        {
            if (selectedBook == null)
            {
                Finish();
                return;
            }

            // 2. 获取所有未拥有的典籍（至少需要2本才能让玩家二选一）
            var unownedBooks = GrowthMgr.Instance.GetRandomUnownedBooks(2);
            if (unownedBooks.Count < 2)
            {
                ShowTip("未拥有的典籍不足2本，交易失败");
                Finish();
                return;
            }

            // 3. 打开二选一面板，让玩家选择获得哪一本
            UIMgr.Instance.ShowPanel<BookSelectPanel>(E_UILayerType.middle, (selectPanel) =>
            {
                selectPanel.Init("请选择想要的典籍",unownedBooks, (selectedNewBook) =>
            {
                if (selectedNewBook == null)
                {
                    Finish();
                    return;
                }
                // 消耗旧典籍，获得新典籍
                GrowthMgr.Instance.RemoveBook(selectedBook.bookId);
                GrowthMgr.Instance.AddBook(selectedNewBook.bookId);
                ShowTip($"消耗典籍《{selectedBook.bookName}》，获得《{selectedNewBook.bookName}》");
                Finish();
            });
            });
            
        });
    }

    private void OnClose()
    {
        Finish();
    }

    private void Finish()
    {
        LevelFlowMgr.Instance.CompleteNode(nodeId);
        UIMgr.Instance.HidePanel<ScaleTradePanel>();
        UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
    }

    private void ShowTip(string msg) 
    {
        UIMgr.Instance.ShowPanel<TipPanel>(E_UILayerType.bottom,(panel) =>
        {
            panel.Init(msg);
        });
    }

    private List<RelicConfig> GetOwnedRelicConfigs()
    {
        List<RelicConfig> list = new List<RelicConfig>();
        foreach (var relicId in GrowthMgr.Instance.growthData.ownedRelicIds)
        {
            var cfg = GrowthMgr.Instance.GetRelicConfig(relicId);
            if (cfg != null) list.Add(cfg);
        }
        return list;
    }
}
