using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefTower_Mu : BaseDefTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;

    public override void Hurt(BaseMonsterCore monster)
    {
        currentHP -= monster.currentAtk;
        Debug.Log($"[·ÀÓùËş]·ÀÓùËşÊÜµ½ÉËº¦{monster.currentAtk},ÏÖÔÚÊ£ÓàÑªÁ¿{currentHP}");
        if (currentHP <= 0)
            DestroyMe();
    }
}
