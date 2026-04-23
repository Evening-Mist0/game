using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardShowBubble : MonoBehaviour
{
    [Header("描述内容")]
    public string description;
    [Header("描述气泡偏移位置")]
    public Vector3 tipOffsetPos;

    private GameObject currentBubble;
    private Canvas targetCanvas;
    private CardEffectUIControl cardEffectUIControl;
    public void ShowPrevCompositeBubble(string cardID_First,string cardID_Second)
    {
        if (currentBubble != null) HideBubble();

        targetCanvas = UIMgr.Instance.canvas;
        if (targetCanvas == null) { Debug.LogError("没有找到 Canvas，无法显示气泡！"); return; }

        currentBubble = PoolMgr.Instance.GetObj("UI/DescriptionBubbleUI");
        if (currentBubble == null) return;

        DescriptionBubbleUI bubble = currentBubble.GetComponent<DescriptionBubbleUI>();
        if (bubble == null) { Debug.LogError("DescriptionBubbleUI 组件未找到！"); return; }

        cardEffectUIControl = this.gameObject.GetComponentInParent<CardEffectUIControl>();
        if (cardEffectUIControl == null)
            Debug.LogError("cardEffectUIControl组件未找到！");

        // 确保气泡一开始不可见（但依然参与布局计算）
        CanvasGroup cg = bubble.GetComponent<CanvasGroup>();
        if (cg == null) cg = bubble.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        bubble.transform.SetParent(UIMgr.Instance.canvas.transform, false);

        //更新卡牌描述

        bubble.UpdateDescibe(TryCompositeCurrentCard(cardID_First, cardID_Second));

        StartCoroutine(SetBubblePositionAfterLayout(bubble, cg));
    }

    private IEnumerator SetBubblePositionAfterLayout(DescriptionBubbleUI bubble, CanvasGroup cg)
    {
        yield return null;
        yield return null; // 等待两帧确保布局完成

        if (currentBubble == null || bubble == null) yield break;

        Vector3 pos = this.transform.position + new Vector3(bubble.GetLeftEdgeToCenterDistance(), 0, 0) + tipOffsetPos;
        bubble.transform.position = pos;

        // 位置设置完成，恢复可见
        cg.alpha = 1f;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void HideBubble()
    {
        if (currentBubble != null)
        {
            PoolMgr.Instance.PushObj(currentBubble.gameObject);
            currentBubble = null;
        }
    }

    private string TryCompositeCurrentCard(string cardID_First, string cardID_Second)
    {
        try
        {
            Debug.Log($"验证合成公式：{cardID_First} + {cardID_Second}");
            var tuple = CardSynthesisFormulaTable.Instance.GetSortedCardIdTuple(cardID_First, cardID_Second);
            if (CardSynthesisFormulaTable.Instance.SynthesisDic.TryGetValue(tuple, out var formula))
            {

                BaseCardScriptableData originalData = Resources.Load<BaseCardScriptableData>(formula.resultDataResName);

                // 创建一个临时副本（深拷贝）
                BaseCardScriptableData tempData = Instantiate(originalData);
                //结算典籍对卡牌的加成
                GamePlayer.Instance.playerBag.BookOnPrevSlected(tempData);
                //结算奇物对卡牌的加成
                GamePlayer.Instance.playerBag.OnPrevSlected(tempData);
       
             
                if(tempData.isFirstActive)
                {
                    string newStr = string.Format(tempData.desPrevComposite, tempData.baseAtk, tempData.baseRecRangeWide, tempData.baseRecRangeHigh);
                    Debug.Log("卡牌可以合成，合成描述为" + newStr);
                    return newStr;
                }
                else//可以合成，但是是为典籍卡牌，且没得到典籍
                {
                    return "未解锁当前典籍";
                }
                    
            }
                return "无法合成";
        }
        catch (Exception e)
        {
            Debug.LogError($"合成公式验证异常：{e.Message}");
            return null;
        }
    }
}
