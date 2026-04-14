using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DefTower_Ke : BaseDefTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;

    [Tooltip("反弹的伤害值")]
    public int reflectAtk = 1;


    public override void OnHurt(OnDefTowerHurtByMonsterEvents evt)
    {
        //反弹伤害给怪物
        evt.monster.TakeDamage(reflectAtk, E_Element.Earth,E_AtkType.DefAtk,false);
        Debug.Log($"[防御塔]防御塔受到伤害{evt.monster.currentAtk},现在剩余血量{currentHP}");

    }
}
