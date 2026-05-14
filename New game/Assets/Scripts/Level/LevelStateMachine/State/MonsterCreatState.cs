using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCreatState : BaseLevelState
{
    public override E_LevelState myStateType => E_LevelState.MonsterTurn_CreatMonster;

    private bool isMonsterCreting;

    public BattleInfo info
    {
        get => LevelStepMgr.Instance.currentBattleInfo;
        set => LevelStepMgr.Instance.currentBattleInfo = value;
    }

    /// <summary>
    /// 当前的生成波次
    /// </summary>
    public int CurrentWave
    {
        get => LevelStepMgr.Instance.currentWave;
        set => LevelStepMgr.Instance.currentWave = value;
    }

    public int MonsterAliveCount
    {
        get => LevelStepMgr.Instance.monsterAliveCount;
        set => LevelStepMgr.Instance.monsterAliveCount = value;
    }

    public int CurrentEliteCount
    {
        get => LevelStepMgr.Instance.currentEliteCount;
        set => LevelStepMgr.Instance.currentEliteCount = value;
    }
    public int CurrentBossCount
    {
        get => LevelStepMgr.Instance.currentBossCount;
        set => LevelStepMgr.Instance.currentBossCount = value;
    }
    public override void EnterState()
    {

        Debug.Log("进入MonsterCreatState,怪物总量剩余" + info.monsterCounts);
        //增加怪物波次
        CurrentWave++;
        if(info != null)
        {
            if (info.monsterCounts <= 0)
            {
                Debug.Log("关卡怪物创建的总数量额度完成,不再创建");
                if (LevelStepMgr.Instance.isTeach)
                    LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_Teach);
                else
                {
                    Debug.Log("怪物创建状态，进入打牌状态");
                    LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_DrawCard);
                }
            }
            else
            {
                //创建这次要生成的数量
                int roundCount = CreatCurrentRoundCount();
                //如果数量大于关卡剩余怪物数量，直接用关卡剩余数量
                if (roundCount > info.monsterCounts)
                    roundCount = info.monsterCounts;

                int realRoundCount;
                //获得真正创建成功的怪物数量
                if (LevelStepMgr.Instance.isTeach && CurrentWave == 1)
                    realRoundCount = MonsterCreater.Instance.CreateOneMonsterAt(DataCenter.Instance.monsterResNameData.Monster_Water01_WaterWisp, GridMgr.Instance.gridWideCount - 1, roundCount);
                else if(LevelStepMgr.Instance.isTeach)
                    realRoundCount = MonsterCreater.Instance.CreateOneMonsterAt(DataCenter.Instance.monsterResNameData.GetRandomBasicMonsterName(), GridMgr.Instance.gridWideCount - 1, roundCount);
                else

                {
                    realRoundCount = MonsterCreater.Instance.CreateOneMonsterAt(DataCenter.Instance.monsterResNameData.Monster_Wood01_WoodSpirit, GridMgr.Instance.gridWideCount - 1, roundCount);
                    //realRoundCount = CreateMonsterAccordingWave(CurrentWave, roundCount);


                }


                //更新还需生成的怪物数量
                info.monsterCounts -= realRoundCount;
                if (info.monsterCounts < 0)
                    info.monsterCounts = 0;
                isMonsterCreting = false;
            }
        }
      
    }

    public override void ExitState()
    {
        isMonsterCreting = true;
        Debug.Log("退出MonsterCreatState");

    }

    public override void OnState()
    {
        if (!isMonsterCreting)
        {
            Debug.Log("初始状态关是否进入教学关卡" + LevelStepMgr.Instance.isTeach);
            if (LevelStepMgr.Instance.isTeach)
                LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_Teach);
            else
            {
                Debug.Log("怪物创建状态，进入打牌状态");
                LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_DrawCard);
            }
                

        }
    }

    /// <summary>
    /// 随机本局怪物生成数量
    /// </summary>
    /// <returns></returns>
    private int CreatCurrentRoundCount()
    {
        Debug.LogWarning("测试调用，随机创建数固定为1");
        return Random.Range(1,3);
    }

  /// <summary>
  /// 根据波数生成怪物
  /// </summary>
  /// <param name="currentWave">当前的波数</param>
  /// <param name="roundCount">该波次需要生成的怪物总量</param>
  /// <returns>正真成功生成的怪物总量</returns>
    public int CreateMonsterAccordingWave(int currentWave, int roundCount)
    {
        int realRoundCount = 0;

        for (int i = 0; i < roundCount; i++)
        {
            // 每次循环都重置路径，确保每个怪物独立随机
            string pathName = string.Empty;

            // ====================== BOSS 生成 ======================
            if (currentWave >= info.bossMonsterAppearWaveCount)
            {
                bool canCreateBoss = CurrentBossCount < info.maxBossCount;
                if (canCreateBoss)
                {
                    Debug.Log("[LevelStepMgr]生成Boss");
                    pathName = DataCenter.Instance.monsterResNameData.Monster_None01_GodofAllElementalArts;
                    CurrentBossCount++;
                }
            }

            // ====================== 已有BOSS，直接生成 ======================
            if (!string.IsNullOrEmpty(pathName))
            {
                realRoundCount += MonsterCreater.Instance.CreateMonster(pathName, 1);
                continue; //不 return，而是继续下一轮循环
            }

            // ====================== 普通怪 / 精英怪 ======================
            if (currentWave < info.eliteMonsterAppearWaveCount)
            {
                // 普通怪
                pathName = DataCenter.Instance.monsterResNameData.GetRandomBasicMonsterName();
            }
            else
            {
                // 精英怪概率
                bool canCreateElite = CurrentEliteCount < info.maxEliteCount;
                if (canCreateElite && Random.Range(0, 100) < info.eliteMonsterAppearProb)
                {
                    Debug.Log("[LevelStepMgr]生成精英怪");
                    pathName = DataCenter.Instance.monsterResNameData.GetRandomEliteMonsterName();
                    CurrentEliteCount++;
                }
                else
                {
                    Debug.Log("[LevelStepMgr]生成普通怪");
                    pathName = DataCenter.Instance.monsterResNameData.GetRandomBasicMonsterName();
                }

                // 概率增加
                info.eliteMonsterAppearProb += info.eliteAppearGrowthProb;
            }

            // 生成当前怪物
            realRoundCount += MonsterCreater.Instance.CreateMonster(pathName, 1);
        }

        return realRoundCount;
    }

}
