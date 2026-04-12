using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InkExchangePanel : BasePanel
{
    // 按钮自身的位置
    public RectTransform radicalPosXi;
    public RectTransform radicalPosYe;
    public RectTransform radicalPosKe;
    public RectTransform radicalPosPi;

    public int currentInkValue => GamePlayer.Instance.currentInkValue;
    public int maxInkValue => GamePlayer.Instance.maxInkValue;


    public override void ShowMe()
    {
        base.ShowMe();
        Time.timeScale = 0f; // 暂停游戏
    }

    public override void HideMe()
    {
        base.HideMe();
        Time.timeScale = 1f; // 恢复游戏
    }
    private void HandleAddXi()
    {
        GamePlayer.Instance.AddInk(-maxInkValue);
        Debug.Log($"[按钮添加部首牌]{DataCenter.Instance.cardResNameData.radical_xi}");
        CreateCardAndMoveUI(DataCenter.Instance.cardResNameData.radical_xi, UIMgr.Instance.GetPanel<CardPlayingPanel>().radicalPosXi);
        if (currentInkValue < maxInkValue)
            HideMe();
    }

    private void HandleAddYe()
    {
        GamePlayer.Instance.AddInk(-maxInkValue);
        Debug.Log($"[按钮添加部首牌]{DataCenter.Instance.cardResNameData.radical_ye}");
        CreateCardAndMoveUI(DataCenter.Instance.cardResNameData.radical_ye, UIMgr.Instance.GetPanel<CardPlayingPanel>().radicalPosYe);
        if (currentInkValue < maxInkValue)
            HideMe();
    }

    private void HandleAddKe()
    {
        GamePlayer.Instance.AddInk(-maxInkValue);
        Debug.Log($"[按钮添加部首牌]{DataCenter.Instance.cardResNameData.radical_ke}");
        CreateCardAndMoveUI(DataCenter.Instance.cardResNameData.radical_ke, UIMgr.Instance.GetPanel<CardPlayingPanel>().radicalPosKe);
        if (currentInkValue < maxInkValue)
            HideMe();
    }

    private void HandleAddPi()
    {
        GamePlayer.Instance.AddInk(-maxInkValue);
        Debug.Log($"[按钮添加部首牌]{DataCenter.Instance.cardResNameData.radical_pi}");
        CreateCardAndMoveUI(DataCenter.Instance.cardResNameData.radical_pi, UIMgr.Instance.GetPanel<CardPlayingPanel>().radicalPosPi);
        if (currentInkValue < maxInkValue)
            HideMe();
    }

    /// <summary>
    /// 【通用】创建卡牌并从当前按钮位置 平滑移动到目标UI位置
    /// </summary>
    private void CreateCardAndMoveUI(string cardResName, RectTransform targetUiPos)
    {
        Debug.Log($"创建卡牌 {cardResName} 并移动到目标UI位置 {targetUiPos.name}");

        // 1. 创建卡牌（父物体用当前按钮的Transform，位置正确）
        BaseCard card = Dealer.Instance.CreateAndAddCard(cardResName, 0, transform);
        card.cardEffectControl.enabled = false;

        if (card.TryGetComponent<RectTransform>(out var cardRect))
        {
            // 获取画布
            Canvas canvas = GetComponentInParent<Canvas>();
            cardRect.SetParent(canvas.transform, false);

            // 启动平滑移动
            MonoMgr.Instance.StartCoroutine(MoveCardToTargetUICoroutine(cardRect, targetUiPos));
        }
        else
        {
            Debug.LogError("卡牌没有RectTransform！");
        }
    }

    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch (name)
        {
            case "btnAddXi":
                HandleAddXi();
                break;
            case "btnAddYe":
                HandleAddYe();
                break;
            case "btnAddKe":
                HandleAddKe();
                break;
            case "btnAddPi":
                HandleAddPi();
                break;
        }
    }


    private IEnumerator MoveCardToTargetUICoroutine(RectTransform cardRect, RectTransform targetRect)
    {
        if (cardRect == null || targetRect == null) yield break;

        float moveDuration = 0.6f;
        float elapsedTime = 0f;

        // 正确用法：使用 position（世界坐标），永远不偏移
        Vector3 startPos = cardRect.position;
        Vector3 endPos = targetRect.position;

        AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

        while (elapsedTime < moveDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = curve.Evaluate(elapsedTime / moveDuration);
            cardRect.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        //最终对齐世界坐标
        cardRect.position = endPos;

        if (cardRect.TryGetComponent<BaseCard>(out var baseCard))
        {
            Dealer.Instance.RemoveCard(baseCard);
        }
    }

}