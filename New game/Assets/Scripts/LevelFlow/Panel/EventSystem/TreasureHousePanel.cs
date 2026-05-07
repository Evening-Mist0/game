using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;


public class TreasureHousePanel : BasePanel
{
    [SerializeField] private Button bookBtn;      // 拿走典籍
    [SerializeField] private Button relicBtn;     // 拿走奇物
    [SerializeField] private Button supplyBtn;    // 拿走物资
    [SerializeField] private Button copperBtn;    // 拿走铜钱
    [SerializeField] private Button allBtn;       // 全拿走
    [SerializeField] private Button bowBtn;       // 鞠躬
    private string nodeId;

    public void Init(string nodeId) 
    { 
        this.nodeId = nodeId; 
        bookBtn.onClick.AddListener(OnBook);
        relicBtn.onClick.AddListener(OnRelic);
        supplyBtn.onClick.AddListener(OnSupply);
        copperBtn.onClick.AddListener(OnCopper);
        allBtn.onClick.AddListener(OnAll);
        bowBtn.onClick.AddListener(OnBow);

    }

    private void OnBook() 
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        if (Random.Range(0, 100) < 60)
        {
            // 拿走一本未拥有的典籍
            var unownedBooks = GrowthMgr.Instance.GetRandomUnownedBooks(1);
            if (unownedBooks.Count == 0)
            {
                ShowTip("没有可获得的典籍");
                return;
            }
            var book = unownedBooks[0];
            GrowthMgr.Instance.AddBook(book.bookId);
            ShowTip($"获得典籍《{book.bookName}》");
            Finish(); 
        }
        else
        {
            ShowTip("触碰机关，损失5点生命值");
            // 失败扣除5血，需要血量检测
            GrowthMgr.Instance.PlayerTakeDamage(5);
            Finish();
        }


    }
    private void OnRelic() 
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        if (Random.Range(0, 100) < 60)
        {
            // 拿走一件奇物
            var ownedIds = GrowthMgr.Instance.growthData.ownedRelicIds;
            var candidates = GrowthMgr.Instance.relicConfig.relicConfigs
                .Where(r => !ownedIds.Contains(r.relicId))
                .ToList();
            if (candidates.Count == 0)
            {
                ShowTip("没有可获得的奇物");
                return;
            }
            var relic = candidates[Random.Range(0, candidates.Count)];
            GrowthMgr.Instance.AddRelic(relic.relicId);
            ShowTip($"获得奇物《{relic.relicName}》");
            Finish();
        }
        else
        {
            ShowTip("触碰机关，损失5点生命值");
            // 失败扣除5血，需要血量检测
            GrowthMgr.Instance.PlayerTakeDamage(5);
            Finish();
        }
        
        
     
    }
    private void OnSupply() 
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        if (Random.Range(0, 100) < 60)
        {
            GrowthMgr.Instance.PlayerRecoverHp(10);
            ShowTip("恢复10点生命值");
            Finish(); 
        }
        else
        {
            ShowTip("触碰机关，损失5点生命值");
            // 失败扣除5血，需要血量检测
            GrowthMgr.Instance.PlayerTakeDamage(5);
            Finish();
        }
        

    }
    private void OnCopper()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        // 拿走铜钱：60%获得70铜，40%扣除5血
        if (Random.Range(0, 100) < 60)
        {
            GrowthMgr.Instance.AddCopperCoins(70);
            ShowTip("获得30铜钱");
            Finish();
        }
        else
        {
            ShowTip("触碰机关，损失5点生命值");
            // 失败扣除5血，需要血量检测
            GrowthMgr.Instance.PlayerTakeDamage(5);
            Finish();
        }
    }
    private void OnAll()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        // 全拿走：20%获得所有奖励，80%扣除25血
        if (Random.Range(0, 100) < 20)
        {
            // 成功：获得典籍、奇物、30铜、恢复10血
            // 典籍
            var unownedBooks = GrowthMgr.Instance.GetRandomUnownedBooks(1);
            if (unownedBooks.Count > 0)
                GrowthMgr.Instance.AddBook(unownedBooks[0].bookId);
            // 奇物
            var ownedIds = GrowthMgr.Instance.growthData.ownedRelicIds;
            var candidates = GrowthMgr.Instance.relicConfig.relicConfigs
                .Where(r => !ownedIds.Contains(r.relicId))
                .ToList();
            if (candidates.Count > 0)
            {
                var relic = candidates[UnityEngine.Random.Range(0, candidates.Count)];
                GrowthMgr.Instance.AddRelic(relic.relicId);
            }
            // 铜钱和恢复
            GrowthMgr.Instance.AddCopperCoins(30);
            GrowthMgr.Instance.PlayerRecoverHp(10);
            ShowTip("你成功搬空了宝库！获得典籍、奇物、30铜钱，并恢复了10点生命值");
            Finish();
        }
        else
        {      
            ShowTip("触发了机关，损失25点生命值，什么都没拿到");
            GrowthMgr.Instance.PlayerTakeDamage(25);
            Finish();
        }
    }

    private void OnBow()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        GrowthMgr.Instance.AddLicenseExp(1);
        ShowTip("你向石碑鞠躬，获得了一点感悟（+1执照经验）");
        Finish();
    }
    private void Finish()
    {
        UIMgr.Instance.HidePanel<TreasureHousePanel>();
        LevelFlowMgr.Instance.CompleteNode(nodeId);
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
