using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefTower_Earth_Yi : BaseDefTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;

    //public override void Hurt(BaseMonsterCore monster)
    //{
    //    currentHP -= monster.currentAtk;

    //    effectControl.ShowDamageText(monster.currentAtk,this.transform.position);
    //    effectControl.UpdateBlood(currentHP, maxHP);
    //    effectControl.UpdateDef(nowDef);

    //    Debug.Log($"[·ÀÓùËş]·ÀÓùËşÊÜµ½ÉËº¦{monster.currentAtk},ÏÖÔÚÊ£ÓàÑªÁ¿{currentHP}");
    //    if (currentHP <= 0)
    //        DestroyMe();
    //}

}
