using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DefTower_Yao : BaseDefTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;

    private int reflectAtk = 5;
    /// <summary>
    /// 禁锢的回合数
    /// </summary>
    private int imprisionCount = 1;


    public override void OnHurt(OnDefTowerHurtByMonsterEvents evt)
    {
        
        //如果被摧毁，对怪物施加5伤害并禁锢
        if (currentHP <= 0)
        {
            evt.monster.TakeDamage(reflectAtk, E_Element.Earth, E_AtkType.DefAtk,false);
            evt.monster.GetImprison(imprisionCount);
            DestroyMe();
        }
    }
}
