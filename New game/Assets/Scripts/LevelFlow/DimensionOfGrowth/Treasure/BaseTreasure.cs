using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_TreasureType
{
    HuoRong,
    Paperweight,
    Stone,
    WaterDrop,
    WoodLeaf,

    ClosedBook,
    FireMask,
    Inkstone,
    MagicBrush,

    BlazingSunPearl,
    EchoConch,
    GuiyuanCompass,
    PenEdge,
}

[System.Serializable]

public abstract class BaseTreasure : BaseGrowthObj
{
    public abstract E_TreasureType type { get; }
    public abstract int round { get; }
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
    public virtual void OnSynthesisSuccessed(BaseCard card)
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

    /// <summary>
    /// 当卡牌预选时触发（触发了弹出动画）
    /// </summary>
    public virtual void OnPrevSlected(BaseCardScriptableData data)
    {
        
    }

    /// <summary>
    /// 当卡牌取消预选时触发（触发弹回动画）
    /// </summary>
    public virtual void OnCancelPrevSlected(BaseCardScriptableData data)
    {

    }


}
