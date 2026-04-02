using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCreatState : BaseLevelState
{
    public override E_LevelState myStateType => E_LevelState.MonsterTurn_CreatMonster;

    private bool isMonsterCreting;

    public override void EnterState()
    {
       

        if (LevelStepMgr.Instance.monsterCounts <= 0)
        {
            Debug.Log("关卡怪物创建的总数量额度完成,不再创建");
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_DrawCard);
        }
        else
        {
            //创建这次要随机生成的数量
            int roundCount = CreatCurrentRoundCount();
            //如果数量大于关卡剩余怪物数量，直接用关卡剩余数量
            if (roundCount > LevelStepMgr.Instance.monsterCounts)
                roundCount = LevelStepMgr.Instance.monsterCounts;

            //获得真正创建成功的怪物数量
            //int realRoundCount = MonsterCreater.Instance.CreateMonster(DataCenter.Instance.monsterResNameData.GetRandomMonsterName(), roundCount);
            int realRoundCount = MonsterCreater.Instance.CreateMonster(DataCenter.Instance.monsterResNameData.Monster_Water01_WaterWisp, roundCount);
            LevelStepMgr.Instance.monsterCounts -= realRoundCount;
            if (LevelStepMgr.Instance.monsterCounts < 0)
                LevelStepMgr.Instance.monsterCounts = 0;
            isMonsterCreting = false;
        }          
    }

    public override void ExitState()
    {
        isMonsterCreting = true;
    }

    public override void OnState()
    {
        if (!isMonsterCreting)
        {
            Debug.Log("怪物创建状态，进入打牌状态");
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_DrawCard);

        }
    }

    /// <summary>
    /// 随机本局怪物生成数量
    /// </summary>
    /// <returns></returns>
    private int CreatCurrentRoundCount()
    {
        //return Random.Range(1, 5);
        Debug.LogWarning("测试调用，随机创建数固定为1");
        return 1;
    }



}
