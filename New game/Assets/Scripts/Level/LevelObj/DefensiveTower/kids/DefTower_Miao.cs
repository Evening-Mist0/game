using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefTower_Miao : BaseDefTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;

    [Tooltip("水域每回合攻击伤害")]
    public int atk;
    [Tooltip("水域持续回合")]
    public int roundTime;
    //水域还剩多少回合
    private int nowRoundTime;

    public override void Hurt(BaseMonster monster)
    {
        
    }

    protected override void InitValue()
    {
        base.InitValue();
        nowRoundTime = roundTime;
    }

    public void Atk(BaseMonster monster)
    {
        monster.TakeDamage(atk, E_CardSkill.None);
    }

    protected override void Awake()
    {
        base.Awake();
        TypeSafeEventCenter.Instance.Register<OnEnterMonsterSettelEvent>(this, HandleEnterMonsterSettel);
    }

    /// <summary>
    /// 寻找自己处于的位置又没有怪物，如果有怪物就对怪物发起攻击
    /// </summary>
    private void HandleEnterMonsterSettel(OnEnterMonsterSettelEvent evt)
    {
        if((myCell.nowStateType == CellStateType.MonsterOccupied) || (myCell.nowStateType == CellStateType.GhostOccupied))
        {
            Debug.Log("[淼]检查测到当前格子被怪物占据，对怪物发起攻击");
            BaseMonster monster = myCell.nowObj as BaseMonster;
            if(monster != null)
            {
                monster.TakeDamage(atk,E_CardSkill.None);
            }
        }
        myCell.nowStateType = CellStateType.GhostOccupied;
        nowRoundTime--;
        if(nowRoundTime <= 0)
        {
            DestroyMe();
        }
    }

}
