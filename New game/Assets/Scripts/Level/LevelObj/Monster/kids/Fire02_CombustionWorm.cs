using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire02_CombustionWorm : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    [Tooltip("死亡时向前方造成的伤害")]
    public int deadAtk;
    protected override void OnDeadSpecial(MonsterOnDead evt)
    {
        base.OnDeadSpecial(evt);
        //如果在第零列，死亡时直接向玩家发起进攻
        if(currentPos.x <= 0)
        {
            GamePlayer.Instance.Hurt(deadAtk);
        }
        else//如果在其他列，尝试向前方发起攻击
        {
            GridPos nextPos = currentPos + new GridPos(-1,0);
            Debug.Log($"[爆燃虫]死亡尝试攻击的坐标{nextPos.x}{nextPos.y}");
            Cell nextCell;
            if (GridMgr.Instance.cellDic.ContainsKey(nextPos))
            {
                nextCell = GridMgr.Instance.cellDic[nextPos];
                if (nextCell.nowObj != null)
                {
                    Debug.Log($"[爆燃虫]死亡尝试攻击的坐标不为空，攻击的物体为{nextCell.nowObj.name}");

                    currentAtk = deadAtk;
                    combat.AttackTarget(nextCell.nowObj);
                }
            }
        }
    }

}
