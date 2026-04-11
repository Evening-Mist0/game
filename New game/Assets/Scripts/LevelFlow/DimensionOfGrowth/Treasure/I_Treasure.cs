using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 所有的奇物要继承这个行为接口
/// </summary>
public interface I_Treasure 
{
    /// <summary>
    /// 抽牌时触发
    /// </summary>
    /// <param name="card"></param>
    public abstract void OnDrawCard(BaseCard card);


    /// <summary>
    /// 卡牌打出时触发
    /// </summary>
    /// <param name="card"></param>
    public abstract void OnPlay(BaseCard card);

    /// <summary>
    /// 放置阻挡物时触发
    /// </summary>
    /// <param name="card"></param>
    public abstract void OnCreateDefTower(BasePlaceCard card);

    /// <summary>
    /// 合成时触发
    /// </summary>
    /// <param name="card"></param>
    public abstract void OnSynthesis(BaseCard card);

    /// <summary>
    /// 重置奇物的成员变量(点击“结束回合”按钮时)
    /// </summary>
    public abstract void ResetOnClickOverTurn();

    /// <summary>
    /// 重置奇物的成员变量(点击本局游戏结束时)
    /// </summary>
    public abstract void ResetOnLevelOver();


 
}
