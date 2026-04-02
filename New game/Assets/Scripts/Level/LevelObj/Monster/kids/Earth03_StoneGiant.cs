using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth03_StoneGiant : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    [Tooltip("怪物进场治愈土属性怪物的治愈量")]
    public int healValue;
    [Tooltip("怪物受伤反弹的真伤值")]
    public int reflect;

    protected override void OnEnterSpecial(MonsterOnEnter evt)
    {
        base.OnEnterSpecial(evt);

        //effectControl.AddBuffIcon(E_BuffIconType.Reflect);
        //effectControl.AddBuffIcon(E_BuffIconType.AddBloodToMonster);
        List<BaseMonsterCore> list = MonsterCreater.Instance.GetMonstersInColumn(evt.currentPos.x);
        for(int i = 0;i < list.Count; i++)
        {
            if (list[i].element == MonsterElement.Earth)
            {
                list[i].AddHp(healValue);
            }
        }
    }

    protected override void OnHurtSpecial(MonsterOnHurt evt)
    {
        base.OnHurtSpecial(evt);
        GamePlayer.Instance.Hurt(reflect, true);
    }

}
