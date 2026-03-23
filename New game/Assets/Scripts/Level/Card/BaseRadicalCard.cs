using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseRadicalCard : BaseCard
{
    public abstract E_RadicalCardType radicalCardType { get; }

    /// <summary>
    /// 卡牌位移持续事件(值越小移动越快)
    /// </summary>
    public float duration = 0.6f;

    /// <summary>
    /// 持有该卡牌的数量
    /// </summary>
    /// 
    public int myCardCount = 0;


    [Tooltip("挂载该脚本的对象是卡槽还是场景实例")]
    public bool isSlot;

    private void OnEnable()
    {

    }

    /// <summary>
    /// 该类型卡牌数量+1
    /// </summary>
    public void AddCardCount()
    {
        if(!isSlot)
        {
            Debug.LogError("是卡牌实例却在当卡槽用");
            return;
        }
       
        myCardCount++;
        //更新UI界面
        UIMgr.Instance.GetPanel<CardPlayingPanel>().UpdateRadicalCount(radicalCardType, myCardCount);
    }

    /// <summary>
    /// 该类型卡牌数量-1
    /// </summary>
    public void ReduceCardCount()
    {
        if (!isSlot)
        {
            Debug.LogError("是卡牌实例却在当卡槽用");
            return;
        }
        myCardCount--;
        //更新UI界面
        UIMgr.Instance.GetPanel<CardPlayingPanel>().UpdateRadicalCount(radicalCardType, myCardCount);
    }

    /// <summary>
    /// 该类型卡牌数量清零(回合结束时使用)
    /// </summary>
    public void CardCountTurnZero()
    {
        if (!isSlot)
        {
            Debug.LogError("是卡牌实例却在当卡槽用");
            return;
        }

        myCardCount = 0;
        //更新UI界面
        UIMgr.Instance.GetPanel<CardPlayingPanel>().UpdateRadicalCount(radicalCardType, myCardCount);
    }


    /// <summary>
    /// 将卡牌从当前位置移动到指定位置（部首牌调用）
    /// </summary>
    /// <param name="target">父对象坐标，先设置为该对象的</param>
    public void MoveAt(RectTransform target)
    {
        if (isSlot)
        {
            Debug.LogError("是卡槽却在当卡牌实例用");
            return;
        }

        StartCoroutine(MoveAtCoroutine(target));
    }

    private IEnumerator MoveAtCoroutine(RectTransform target)
    {
        // 终点：将UI的RectTransform位置转换为世界坐标
        Vector3 endPos = target.position;

        // 起点：自身世界坐标,并与终点的Z轴堆成
        Vector3 startPos = this.transform.position;
        startPos.z = endPos.z;
       

       
        float time = 0;

        // 平滑移动到目标位置
        while (time < duration)
        {
            float t = time / duration;
            // 使用平滑曲线
            t = Mathf.SmoothStep(0, 1, t);

            // 在世界空间中进行插值
            transform.position = Vector3.Lerp(startPos, endPos, t);

            time += Time.deltaTime;
            yield return null;
        }

        // 确保最终位置精确
        transform.position = endPos;

        // 移动完成后的逻辑
        Debug.Log("物体移动到UI位置完成");

        //使用这个方法的根本是不是一张卡，直接放回对象池即可
        Dealer.Instance.RemoveCard(this);
    }
}

