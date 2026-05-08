using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth03_StoneGiant : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    [Tooltip("怪物进场治愈土属性怪物的治愈量")]
    public int healValue;
    [Tooltip("怪物每回合获得的临时护甲")]
    public int tempDef;

    protected override void OnEnterSpecial(MonsterOnEnter evt)
    {
        base.OnEnterSpecial(evt);
        nowDef += tempDef;
        effectControl.UpdateDef(nowDef);
        List<BaseMonsterCore> list = MonsterCreater.Instance.GetMonstersInColumn(evt.currentPos.x);
        for(int i = 0;i < list.Count; i++)
        {
            if (list[i].element == MonsterElement.Earth)
            {
                list[i].AddHp(healValue);
            }
        }
    }

    protected override void OnRoundSpecial(MonsterOnRound evt)
    {
        base.OnRoundSpecial(evt);
        nowDef = 0;
        nowDef += tempDef;
        effectControl.UpdateDef(nowDef);

    }

   

}
