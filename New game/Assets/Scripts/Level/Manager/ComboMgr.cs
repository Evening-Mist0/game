using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ComboData
{
    public E_Element cardElement;
    public E_CardRadical cardRadical;
}

/// <summary>
/// 检查卡牌打出的关联性，记录连击数量
/// </summary>
public class ComboMgr : BaseMonoMgr<ComboMgr>
{
    public int comboCount = 0;
    public ComboData prevComboData;    // 上一张卡牌数据
    public ComboData currentComboData; // 当前卡牌数据

    public int comboRewardInk = 1; // 连击奖励的墨水数量

    /// <summary>
    /// 正确记录上一张和当前卡牌数据
    /// </summary>
    private void RecordCardComboData(ComboData data)
    {
        // 旧的当前 → 变成上一张
        prevComboData = currentComboData;
        // 新数据 → 当前
        currentComboData = data;
    }

    /// <summary>
    /// 判断打出卡牌时候是否触发连击
    /// </summary>
    public bool JudgementPlayCardCombo(ComboData data)
    {
        // 记录数据
        RecordCardComboData(data);

        //获得面板
        CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();

        // 第一张牌：没有上一张，不触发连击
        if (prevComboData == null)
        {
            comboCount = 1;
            //UI更新
            if (panel != null)
                panel.comboViewControl.UpdateComboView(comboCount, currentComboData);
            return false;
        }

        
        //元素判断
        bool isElementMatch = currentComboData.cardElement == prevComboData.cardElement;

        //部首判断
        bool isRadicalValid = currentComboData.cardRadical != E_CardRadical.none;
        bool isRadicalMatch = currentComboData.cardRadical == prevComboData.cardRadical;

        // 最终判断
        bool isCombo = isElementMatch || (isRadicalValid && isRadicalMatch);
  

        // 更新连击数
        if (isCombo)
        {
            comboCount++;
        }
        else
        {
            if(comboCount > 1)
            {
                //断连增加笔墨
                GamePlayer.Instance.AddInk(comboCount);
                //给予增加笔墨的数量提示
                panel.comboViewControl.PlayReWardAnim(comboCount);
            }
            comboCount = 1; // 断连重置为1
        }

        //UI更新
        if (panel != null)
            panel.comboViewControl.UpdateComboView(comboCount,currentComboData);
       

        return isCombo;
    }

    public bool JudgementPlayCompositeCombo(ComboData data)
    {
        // 记录数据
        RecordCardComboData(data);

        //获得面板
        CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();


        // 第一张牌：没有上一张，不触发连击
        if (prevComboData == null)
        {
            comboCount = 1;
            //UI更新
            if (panel != null)
                panel.comboViewControl.UpdateComboView(comboCount, currentComboData);
            return false;
        }

        //元素或部首相同
        bool isCombo = currentComboData.cardElement == prevComboData.cardElement;

        // 更新连击数
        if (isCombo)
        {
            comboCount++;
        }
        else
        {
            //断连增加笔墨
            GamePlayer.Instance.AddInk(comboCount);
            //给予增加笔墨的数量提示
            panel.comboViewControl.PlayReWardAnim(comboCount);
            // 断连重置为1
            comboCount = 1; 
        }

        //UI更新
        if (panel != null)
            panel.comboViewControl.UpdateComboView(comboCount, currentComboData);


        return isCombo;
    }

    /// <summary>
    /// 清空连击（重新开始游戏时调用）
    /// </summary>
    public void ClearCombo()
    {

        //获得面板
        CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();
        if (comboCount > 1)
        {
            // 断连增加笔墨
            GamePlayer.Instance.AddInk(comboCount);
            panel.comboViewControl.PlayReWardAnim(comboCount);
        }

        comboCount = 0;
        prevComboData = null;
        currentComboData = null;

        //UI更新
        if (panel != null)
            panel.comboViewControl.UpdateComboView(comboCount, currentComboData);
    }
}