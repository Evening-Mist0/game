using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Water02_TideSoldier : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    [Tooltip("检测到同列有水属性怪物的临时移动速度")]
    public int tempMoveStepHorizontal;
    [Tooltip("检测到同列有水属性怪物增加速度的回合")]
    public int speedUpLastCount;
    [Tooltip("正常移动速度")]
    public int normalMoveStepHorizontal;

    protected override void OnRoundSpecial(MonsterOnRound evt)
    {
        base.OnRoundSpecial(evt);
        if (speedUpLastCount <= 0)
            buffHandler.RemoveBuff(E_MonsterBuffType.SpeedUp);
    }

    protected override void OnEnterSpecial(MonsterOnEnter evt)
    {
        base.OnEnterSpecial(evt);
        baseMoveStepHorizontal = normalMoveStepHorizontal;
        List<BaseMonsterCore> list = MonsterCreater.Instance.GetMonstersInColumn(evt.currentPos.x);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].element == MonsterElement.Water && (list[i].currentPos.y != evt.currentPos.y))
            {
                if (list[i].monsterID == monsterID)
                {

                    list[i].baseMoveStepHorizontal = tempMoveStepHorizontal;
                    list[i].buffHandler.ApplyBuff(E_MonsterBuffType.SpeedUp, speedUpLastCount);
                    list[i].effectControl.AddBuffIcon(E_BuffIconType.SpeedUp);
                }
                baseMoveStepHorizontal = tempMoveStepHorizontal;
                buffHandler.ApplyBuff(E_MonsterBuffType.SpeedUp, speedUpLastCount);
                effectControl.AddBuffIcon(E_BuffIconType.SpeedUp);
            }
        }
    }
    protected override void OnMoveOverSpecial(MonsterOnMoveOver evt)
    {
        base.OnMoveOverSpecial(evt);
        baseMoveStepHorizontal = normalMoveStepHorizontal;
        List<BaseMonsterCore> list = MonsterCreater.Instance.GetMonstersInColumn(evt.currentPos.x);
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].element == MonsterElement.Water && (list[i].currentPos.y != evt.currentPos.y))
            {
                if (list[i].monsterID == monsterID)
                {
                    
                    list[i].baseMoveStepHorizontal = tempMoveStepHorizontal;
                    list[i].buffHandler.ApplyBuff(E_MonsterBuffType.SpeedUp, speedUpLastCount);
                    list[i].effectControl.AddBuffIcon(E_BuffIconType.SpeedUp);
                }
                baseMoveStepHorizontal = tempMoveStepHorizontal;
                buffHandler.ApplyBuff(E_MonsterBuffType.SpeedUp, speedUpLastCount);
                effectControl.AddBuffIcon(E_BuffIconType.SpeedUp);
            }
        }
    }
}
