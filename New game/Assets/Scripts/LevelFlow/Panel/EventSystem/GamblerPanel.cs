using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamblerPanel : BasePanel
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI descText;
    [SerializeField] private TextMeshProUGUI statusText;     // 显示当前轮次、赌注、累计收益
    [SerializeField] private Button gambleBtn;               // 继续赌博
    [SerializeField] private Button quitBtn;                 // 离开（领取当前收益）
    [SerializeField] private Button closeBtn;                // 关闭面板（放弃所有）
    
    private string nodeId;
    private bool isFinished = false;
    
    // 赌局状态
    private int currentRound = 0;          // 0=第一轮，1=第二轮，2=第三轮，3=最后一轮
    private int currentBet = 0;             // 本轮需要支付的铜钱（第一轮固定20，后续轮次为上一轮赢得的奖励）
    private int accumulatedWin = 0;         // 当前累计赢得的铜钱（不含已投入本金）
    private bool isGambling = false;
    
    public void Init(string nodeId)
    {
        this.nodeId = nodeId;
        descText.text = "你遇见了一个千门高手，他邀请你参加一场赌局。";
        gambleBtn.onClick.AddListener(OnGamble);
        quitBtn.onClick.AddListener(OnQuit);
        if (closeBtn != null) closeBtn.onClick.AddListener(OnClose);
        
        // 开始第一轮
        StartRound();
    }
    
    private void StartRound()
    {
        currentRound = 0;
        accumulatedWin = 0;
        currentBet = 20;
        isGambling = true;
        UpdateUI();
    }
    
    private void UpdateUI()
    {
        string roundDesc = "";
        switch (currentRound)
        {
            case 0: roundDesc = "第一轮 (90%成功率)"; break;
            case 1: roundDesc = "第二轮 (50%成功率)"; break;
            case 2: roundDesc = "第三轮 (20%成功率)"; break;
            case 3: roundDesc = "终极梭哈 (1%成功率)"; break;
        }
        statusText.text = $"当前轮次：{roundDesc}\n本轮赌注：{currentBet}铜钱\n累计赢得：{accumulatedWin}铜钱";
        gambleBtn.interactable = true;
        quitBtn.interactable = true;
    }
    
    private void OnGamble()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        if (!isGambling) return;


        // 检测铜钱是否足够
        if (currentRound == 0)
        {
            // 第一轮要求背包有20铜钱（不是从赢得中扣，而是额外支出）
            if (GrowthMgr.Instance.GetCopperCoins() < currentBet)
            {
                ShowTip("铜钱不足，无法开始赌局！");
                Finish(false, false);
                return;
            }
            // 扣除20铜钱
            GrowthMgr.Instance.SpendCopperCoins(currentBet);
        }
        else
        {
            // 后续轮次赌注来自之前赢得的铜钱（已经存在 accumulatedWin 中，不需要额外扣除）
            if (accumulatedWin < currentBet)
            {
                Debug.LogError("赌注异常：累计赢得少于赌注");
                Finish(false, false);
                return;
            }
            // 注意：这里不额外扣钱，因为赌注是“将赚到的铜钱再赌进去”，表示如果失败会失去这些铜钱（不从背包额外扣）
            // 逻辑在失败处理中：将 accumulatedWin 清零，相当于赌注打了水漂。
        }
        
        // 计算成功率
        int successRate = GetSuccessRate(currentRound);
        bool success = Random.Range(0, 100) < successRate;
        
        if (success)
        {
            // 成功：获得奖励
            int reward = GetReward(currentRound);
            if (currentRound == 0)
            {
                // 第一轮成功：净赚30铜钱（已扣除20，所以实际增加30铜钱到背包，同时累计赢得记为30）
                GrowthMgr.Instance.AddCopperCoins(reward);
                accumulatedWin = reward;
            }
            else if (currentRound == 3) // 最后一轮梭哈
            {
                // 梭哈成功：获得500铜钱，但注意这里赌注是“包括背包内所有铜币”，我们简化处理：直接增加500铜钱，不受已有铜钱影响
                GrowthMgr.Instance.AddCopperCoins(reward);
                accumulatedWin += reward; // 实际上此时累计赢得会很大，但后续不会再有轮次
                ShowTip($"梭哈成功！获得{reward}铜钱！");
                Finish(true, true);
                return;
            }
            else
            {
                // 中间轮次成功：在原有累计赢得基础上增加奖励（但奖励是额外给的，赌注本金还在？）
                // 根据规则：“将赚到的30铜钱再赌进去成功获得50铜钱”，意思是：你投入30，获得50，净赚20。
                // 但前后逻辑：第一轮结束后累计赢得30，第二轮将30赌进去，成功后获得50，那么累计赢得变为50（比之前多了20）。
                // 实际上，我们应该用奖励减去赌注作为净赚。
                int netGain = reward - currentBet;
                accumulatedWin += netGain;
                // 实际铜钱增加净赚部分
                GrowthMgr.Instance.AddCopperCoins(netGain);
                ShowTip($"第{currentRound+1}轮成功！获得{reward}铜钱，净赚{netGain}铜钱！");
            }
            
            // 进入下一轮
            currentRound++;
            if (currentRound < 4)
            {
                // 更新赌注：下一轮的赌注就是本轮赢得的奖金（累计赢得的当前值）
                currentBet = accumulatedWin;
                UpdateUI();
            }
            else
            {
                // 所有轮次结束（理论上第四轮成功后已经返回了，这里防御）
                Finish(true, true);
            }
        }
        else
        {
            // 失败：本轮赌注消失，之前累计赢得的铜钱归零（但背包里的基础铜钱不受影响？第一轮已扣20，后续轮次赌注来源于赢得，未实际从背包扣，所以只需要清零累计赢得，并提示）
            if (currentRound == 0)
            {
                // 第一轮失败：已扣20，无额外损失
                ShowTip("第一轮失败，损失20铜钱！");
            }
            else
            {
                // 后续轮次失败：失去所有累计赢得的铜钱（之前赢得的已经加到背包，需要扣除）
                // 注意：之前每轮成功后已经将净赚加到背包，现在失败需要将累计赢得的全部扣除（因为赌注来自这些赢得）
                if (accumulatedWin > 0)
                {
                    GrowthMgr.Instance.SpendCopperCoins(accumulatedWin);
                    ShowTip($"第{currentRound+1}轮失败！损失{accumulatedWin}铜钱，前功尽弃！");
                }
                else
                {
                    ShowTip($"第{currentRound+1}轮失败！");
                }
            }
            Finish(false, false);
        }
    }
    
    private int GetSuccessRate(int round)
    {
        switch (round)
        {
            case 0: return 90;
            case 1: return 50;
            case 2: return 20;
            case 3: return 1;
            default: return 0;
        }
    }
    
    private int GetReward(int round)
    {
        switch (round)
        {
            case 0: return 30;   // 第一轮成功后获得30铜钱（净赚30）
            case 1: return 50;   // 第二轮成功后获得50铜钱（净赚20）
            case 2: return 100;  // 第三轮成功后获得100铜钱（净赚? 取决于赌注，但这里固定奖励）
            case 3: return 500;  // 梭哈成功获得500
            default: return 0;
        }
    }
    
    private void OnQuit()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        // 离开：带走当前累计赢得的铜钱（已经每次成功时实时加到了铜钱里，所以不需要额外加）
        // 但注意：第一轮成功后已经加过30，第二轮成功后加净赚部分，所以此时背包中的铜钱已经包含了所有净赚。
        // 因此只需要显示提示，结束事件。
        ShowTip($"你带着{accumulatedWin}铜钱（净利）离开了赌桌。");
        Finish(true, true);
    }
    
    private void OnClose()
    {
        AudioMgr.Instance.PlaySFX("按钮点击");

        // 关闭面板：视为放弃所有赢得，但已扣除的第一轮20不退。
        ShowTip("你飞速逃跑，离开了赌桌。放弃了发财机会");
        Finish(false, false);
    }
    
    private void Finish(bool hasWin, bool success)
    {
        if (isFinished) return;
        isFinished = true;
        LevelFlowMgr.Instance.CompleteNode(nodeId);
        UIMgr.Instance.HidePanel<GamblerPanel>();
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
