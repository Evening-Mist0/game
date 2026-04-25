using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DefTower_Earth_Yao : BaseEntityTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;


    public EntityTowerSkillPair skillImprison;

    public EntityTowerSkillPair reflectSkill;

    protected override void InitValue()
    {
        base.InitValue();
        for (int i = 0; i < skills.Count; i++)
        {
            if (skills[i].towerSkill == E_EntityTowerSkill.Reflect)
                reflectSkill = skills[i];
            else if(skills[i].towerSkill == E_EntityTowerSkill.Imprison)
                skillImprison = skills[i];
        }

        Debug.Log($"[防御塔垚]显示时更新自身描述控件效果值{reflectSkill.effectValue}回合数{skillImprison.roundValue}");
        StartCoroutine(DoAfterOneFrame());

    }

    IEnumerator DoAfterOneFrame()
    {
        yield return null;  // 等待一帧
        effectControl.UpdateDesIcon(reflectSkill.effectValue, skillImprison.roundValue);

    }


    public override void OnDestory(OnDefTowerDestoryByMonsterEvents evt)
    {
        base.OnDestory(evt);
        evt.monster.TakeDamage(reflectSkill.effectValue, E_Element.Earth, E_AtkType.DefAtk, false);
        evt.monster.GetImprison(skillImprison.roundValue);
    }
}
