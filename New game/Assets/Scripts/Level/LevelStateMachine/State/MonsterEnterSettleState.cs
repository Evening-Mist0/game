using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEnterSettleState : BaseLevelState
{
    /// <summary>
    /// 状态运行标识
    /// </summary>
    private bool isMonsterEnterSettling = true;

    public override E_LevelState myStateType => E_LevelState.MonsterTurn_EnterSettle;

    public override void EnterState()
    {
        //清空部首牌
        Dealer.Instance.RemoveAllRadicalCard();
        //发起trigger,让幽灵防御塔攻击怪物
        TypeSafeEventCenter.Instance.Trigger<OnEnterMonsterSettelEvent>(new OnEnterMonsterSettelEvent());
        //更新玩家的状态
        GamePlayer.Instance.OnRound();
        //更新怪物的位置和状态
        UpdateMonstersState();
        isMonsterEnterSettling = false;
    }

    public override void ExitState()
    {
        isMonsterEnterSettling = true;
    }

    public override void OnState()
    {
        if (!isMonsterEnterSettling)
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_Move);
    }

    public void UpdateMonstersState()
    {
       List<BaseMonsterCore> monsterList = MonsterCreater.Instance.GetAllAliveMonsters();
        for(int i = 0; i < monsterList.Count; i++)
        {
            monsterList[i].OnRoundUpdate();
        }
    }
}
