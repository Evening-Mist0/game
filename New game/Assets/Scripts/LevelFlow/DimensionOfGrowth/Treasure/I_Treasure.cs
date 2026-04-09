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
    public void OnDrawCard(BaseCard card);

    /// <summary>
    /// 卡牌打出时触发
    /// </summary>
    /// <param name="card"></param>
    public void OnPlay(BaseCard card);

    /// <summary>
    /// 放置阻挡物时触发
    /// </summary>
    /// <param name="card"></param>
    public void OnCreateDefTower(BaseCard card);

    /// <summary>
    /// 合成时触发
    /// </summary>
    /// <param name="card"></param>
    public void OnSynthesis(BaseCard card);
}
