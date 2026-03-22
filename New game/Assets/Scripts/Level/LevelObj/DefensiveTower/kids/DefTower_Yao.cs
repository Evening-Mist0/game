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

    public override void Hurt(BaseMonsterCore monster)
    {
        if(monster.couldDestoryDefAndAhead)//如果可以直接摧毁阻挡物并前进,不需要再进行伤害计算
           currentHP -= maxHP;
        else
            currentHP -= monster.currentAtk;

        Debug.Log($"[防御塔]防御塔受到伤害,现在剩余血量{currentHP}");
        //如果被摧毁，对怪物施加5伤害并禁锢
        if (currentHP <= 0)
        {
            monster.TakeDamage(reflectAtk,E_Element.Earth, E_CardSkill.None);
            monster.GetImprison(imprisionCount);
            DestroyMe();
        }
    }
}
