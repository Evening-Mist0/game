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
        statusText.text = $"当前轮次：{roundDesc}\n本轮赌注：{currentBet}铜钱\n下轮可赢得\n累计赢得：{accumulatedWin}铜钱";

                // 显示下一轮预期收益（如果还有下一轮）
        if (currentRound < 3)
        {
            int nextWin = 0;
            int nextNet = 0;
            if (currentRound == 0) { nextWin = 50; nextNet = 20; }
            else if (currentRound == 1) { nextWin = 100; nextNet = 50; }
            else if (currentRound == 2) { nextWin = 500; nextNet = 500 - currentBet; }
            statusText.text += $"\n下一轮若成功将获得{nextWin}铜钱（净赚{nextNet}）";
        }
        else
        {
            statusText.text += "\n这是最后一轮！";
        }


        gambleBtn.interactable = true;
        quitBtn.interactable = true;
    }
    
    private void OnGamble()
    {
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
            EventCenter.Instance.EventTrigger(E_EventType.UI_PlayerMoneyUpdate,GrowthMgr.Instance.growthData.copperCoins);
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
                EventCenter.Instance.EventTrigger(E_EventType.UI_PlayerMoneyUpdate,GrowthMgr.Instance.growthData.copperCoins);
                accumulatedWin = reward;
            }
            else if (currentRound == 3) // 最后一轮梭哈
            {
                // 梭哈成功：获得500铜钱
                GrowthMgr.Instance.AddCopperCoins(reward);
                accumulatedWin += reward; // 实际上此时累计赢得会很大，但后续不会再有轮次
                ShowTip($"梭哈成功！获得{reward}铜钱！");
                EventCenter.Instance.EventTrigger(E_EventType.UI_PlayerMoneyUpdate,GrowthMgr.Instance.growthData.copperCoins);
                Finish(true, true);
                return;
            }
            else
            {
                int netGain = reward - currentBet;
                accumulatedWin += netGain;
                // 实际铜钱增加净赚部分
                GrowthMgr.Instance.AddCopperCoins(netGain);
                ShowTip($"第{currentRound+1}轮成功！获得{reward}铜钱，净赚{netGain}铜钱！");
                EventCenter.Instance.EventTrigger(E_EventType.UI_PlayerMoneyUpdate,GrowthMgr.Instance.growthData.copperCoins);
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
                // 所有轮次结束
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
                EventCenter.Instance.EventTrigger(E_EventType.UI_PlayerMoneyUpdate,GrowthMgr.Instance.growthData.copperCoins);
            }
            else
            {
                // 后续轮次失败：失去所有累计赢得的铜钱（之前赢得的已经加到背包，需要扣除）
                // 注意：之前每轮成功后已经将净赚加到背包，现在失败需要将累计赢得的全部扣除（因为赌注来自这些赢得）
                if (accumulatedWin > 0)
                {
                    GrowthMgr.Instance.SpendCopperCoins(accumulatedWin);
                    ShowTip($"第{currentRound+1}轮失败！损失{accumulatedWin}铜钱，前功尽弃！");
                    EventCenter.Instance.EventTrigger(E_EventType.UI_PlayerMoneyUpdate,GrowthMgr.Instance.growthData.copperCoins);
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
            case 0: return 30;   // 第一轮成功后获得30铜钱
            case 1: return 50;   // 第二轮成功后获得50铜钱
            case 2: return 100;  // 第三轮成功后获得100铜钱
            case 3: return 500;  // 梭哈成功获得500
            default: return 0;
        }
    }
    
    private void OnQuit()
    {

        ShowTip($"你带着{accumulatedWin}铜钱（净利）离开了赌桌。");
        EventCenter.Instance.EventTrigger(E_EventType.UI_PlayerMoneyUpdate,GrowthMgr.Instance.growthData.copperCoins);
        Finish(true, true);
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
