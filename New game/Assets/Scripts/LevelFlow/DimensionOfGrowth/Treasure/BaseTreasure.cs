using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public  class BaseTreasure : BaseGrowthObj
{
    /// <summary>
    /// 抽牌时触发
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnDrawCard(BaseCard card)
    {

    }

    /// <summary>
    /// 打卡牌打出没有被消耗
    /// </summary>
    public virtual void OnPlayCardNotDestory()
    {

    }

    /// <summary>
    /// 卡牌打出时触发
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnPlay(BaseCard card)
    {

    }

    /// <summary>
    /// 放置阻挡物时触发
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnCreateDefTower(BasePlaceCard card)
    {

    }

    /// <summary>
    /// 合成时触发
    /// </summary>
    /// <param name="card"></param>
    public virtual void OnSynthesis(BaseCard card)
    {

    }

    /// <summary>
    /// 重置奇物的成员变量(点击“结束回合”按钮时)
    /// </summary>
    public virtual void ResetOnClickOverTurn()
    {

    }

    /// <summary>
    /// 重置奇物的成员变量(点击本局游戏结束时)
    /// </summary>
    public virtual void ResetOnLevelOver()
    {

    }

    /// <summary>
    /// 卡牌打出后，卡牌效果结算完成并移除时触发
    /// </summary>
    public virtual void OnPlayFinish(BaseCard card)
    {

    }



}
