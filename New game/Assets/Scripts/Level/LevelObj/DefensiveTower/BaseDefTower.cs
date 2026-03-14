using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDefTower : BaseLevelObject
{
  
   
    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="value">具体的伤害值</param>
    public void Hurt(int value)
    {
        Debug.Log($"[防御塔]防御塔受到伤害{value}");
    }
}
