using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public  class BaseTreasure : BaseGrowthObj, I_Treasure
{
    // 提供默认实现，不标记 abstract
    public virtual void OnDrawCard(BaseCard card) { Debug.Log("父类奇物触发效果"); }
    public virtual void OnPlay(BaseCard card) { }

    public virtual void OnSynthesis(BaseCard card) { }

    public void ResetOnClickOverTurn()
    {
    }

    public void ResetOnEnterClimbPanel()
    {
    }

    public void ResetOnLevelOver()
    {

    }

    public void OnCreateDefTower(BasePlaceCard card)
    {
        
    }

    public void OnCreateCardRange(BaseCard card)
    {
     
    }


}
