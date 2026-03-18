using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefTower_Yao : BaseDefTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;

    private int reflectAtk = 5;
    /// <summary>
    /// 禁锢的回合数
    /// </summary>
    private int imprisionCount = 1;

    public override void Hurt(BaseMonster monster)
    {
        currentHP -= monster.attack;
        Debug.Log($"[防御塔]防御塔受到伤害{monster.attack},现在剩余血量{currentHP}");
        //如果被摧毁，对怪物施加5伤害并禁锢
        if (currentHP <= 0)
        {
            monster.TakeDamage(reflectAtk, E_CardSkill.None);
            monster.GetImprison(imprisionCount);
            DestroyMe();
        }
    }
}
