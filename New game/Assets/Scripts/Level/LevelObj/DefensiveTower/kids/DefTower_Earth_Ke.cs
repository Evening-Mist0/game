using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DefTower_Earth_Ke : BaseEntityTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;

    private EntityTowerSkillPair reflectSkill;

    protected override void InitValue()
    {
        base.InitValue();
        for(int i = 0; i < skills.Count; i++)
        {
            if (skills[i].towerSkill == E_EntityTowerSkill.Reflect)
                reflectSkill = skills[i];
        }
    }
    public override void OnHurt(OnDefTowerHurtByMonsterEvents evt)
    {
        //·´µ¯ÉËº¦¸ø¹ÖÎï
        evt.monster.TakeDamage(reflectSkill.effectValue, E_Element.Earth,E_AtkType.DefAtk,false);
        Debug.Log($"[·ÀÓùËþ]·ÀÓùËþÊÜµ½ÉËº¦{evt.monster.currentAtk},ÏÖÔÚÊ£ÓàÑªÁ¿{currentHP}");

    }
}
