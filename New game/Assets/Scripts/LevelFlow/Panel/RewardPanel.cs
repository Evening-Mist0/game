using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RewardPanel : BasePanel
{
    [Header("UI组件")]
    [SerializeField] private Transform rewardContainer;      // 奖励图标父物体
    [SerializeField] private GameObject rewardItemPrefab;    // 图标预制体（含Image和HoverDescription）
    [SerializeField] private TextMeshProUGUI titleText;      //无额外奖励

    private List<RelicConfig> relics = new List<RelicConfig>();
    private List<BookConfig> books = new List<BookConfig>();
    private System.Action onConfirmCallback;  // 确定后的回调

    protected override void Awake()
    {
        base.Awake();
        titleText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 初始化面板并显示
    /// </summary>
    /// <param name="relics">掉落的奇物列表</param>
    /// <param name="books">掉落的典籍列表</param>
    /// <param name="onConfirm">点击确定后的回调（可选）</param>
    public void ShowRewards(List<RelicConfig> relics, List<BookConfig> books, System.Action onConfirm = null)
    {
        this.relics = relics ?? new List<RelicConfig>();
        this.books = books ?? new List<BookConfig>();
        this.onConfirmCallback = onConfirm;

        // 清空旧内容
        foreach (Transform child in rewardContainer)
            Destroy(child.gameObject);

        // 生成奇物图标
        if(this.relics != null)
        {
            foreach (var relic in this.relics)
            {
                CreateRewardIcon(relic.relicIcon,relic.relicName,relic.relicDesc);
            }
        }
        
        // 生成典籍图标
        if(this.books != null)
        {    
            foreach (var book in this.books)
            {
                CreateRewardIcon(book.bookIcon,book.bookName,book.bookDesc);
            }
        }
            

        // 如果没有奖励，显示提示文字
        if (this.relics.Count == 0 && this.books.Count == 0)
        {
            titleText.gameObject.SetActive(true);
            titleText.text = "施主运气不佳，无奖励给予";
        }

        ShowMe();
    }

    private void CreateRewardIcon(Sprite icon, string name, string desc)
    {
        GameObject opt = Instantiate(rewardItemPrefab, rewardContainer);
        RewordOptionItem item = opt.GetComponent<RewordOptionItem>();
        item.Init(icon,name,desc);
    }


    void Update()
    {
        if (Input.anyKeyDown)
        {
            // 先关闭面板
            HideMe();
            // 执行回调（例如显示爬塔面板）
            onConfirmCallback?.Invoke();
        }
    }
}
