using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefTower_Ke : BaseDefTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;

    private int reflectAtk = 1;

    public override void Hurt(BaseMonsterCore monster)
    {
        //反弹伤害给怪物
        monster.TakeDamage(reflectAtk,E_Element.Earth, E_CardSkill.None);
        currentHP -= monster.currentAtk;
        Debug.Log($"[防御塔]防御塔受到伤害{monster.currentAtk},现在剩余血量{currentHP}");
        if (currentHP <= 0)
            DestroyMe();
    }
}
