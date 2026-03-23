using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water02_TideSoldier : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    [Tooltip("检测到同列有水属性怪物的临时移动速度")]
    public int tempMoveStepHorizontal;
    [Tooltip("正常移动速度")]
    public int normalMoveStepHorizontal;


    protected override void OnRoundSpecial(MonsterOnRound evt)
    {
        base.OnRoundSpecial(evt);
        baseMoveStepHorizontal = normalMoveStepHorizontal;
        List<BaseMonsterCore> list = MonsterCreater.Instance.GetMonstersInColumn(evt.currentPos.x);
        for(int i = 0; i < list.Count; i++)
        {
            if (list[i].element == MonsterElement.Water && (list[i].currentPos.y != evt.currentPos.y))
            {
                baseMoveStepHorizontal = tempMoveStepHorizontal;
                return;
            }
        }
    }
}
