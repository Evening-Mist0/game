using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCreatState : BaseLevelState
{
    public override E_LevelState myStateType => E_LevelState.MonsterTurn_CreatMonster;

    private bool isMonsterCreting;

    /// <summary>
    /// 本次关卡怪物生成的总数量
    /// </summary>
    public int MonsterCounts
    {
        get => LevelStepMgr.Instance.monsterCounts;
        set => LevelStepMgr.Instance.monsterCounts = value;
    }

    /// <summary>
    /// 当前的生成波次
    /// </summary>
    public int CurrentWave
    {
        get => LevelStepMgr.Instance.currentWave;
        set => LevelStepMgr.Instance.currentWave = value;
    }

    /// <summary>
    /// 到第几波开始刷精英怪
    /// </summary>
    public int EliteMonsterAppearWaveCount
    {
        get => LevelStepMgr.Instance.eliteMonsterAppearWaveCount;
        set => LevelStepMgr.Instance.eliteMonsterAppearWaveCount = value;
    }

    /// <summary>
    /// 出现精英怪的初始概率
    /// </summary>
    public int EliteMonsterAppearProbability
    {
        get => LevelStepMgr.Instance.eliteMonsterAppearProbability;
        set => LevelStepMgr.Instance.eliteMonsterAppearProbability = value;
    }

    /// <summary>
    /// 出现精英怪每回合增长的概率（从下回合开始，100%则满）
    /// </summary>
    public int EliteAppearGrowthProbability
    {
        get => LevelStepMgr.Instance.eliteAppearGrowthProbability;
        set => LevelStepMgr.Instance.eliteAppearGrowthProbability = value;
    }

    /// <summary>
    /// 到第几波出现boss(直接为100%刷新)
    /// </summary>
    public int BossMonsterAppearWaveCount
    {
        get => LevelStepMgr.Instance.bossMonsterAppearWaveCount;
        set => LevelStepMgr.Instance.bossMonsterAppearWaveCount = value;
    }

    /// <summary>
    /// 当前怪物还存在的数量
    /// </summary>
    public int MonsterAliveCount
    {
        get => LevelStepMgr.Instance.monsterAliveCount;
        set => LevelStepMgr.Instance.monsterAliveCount = value;
    }

    /// <summary>
    /// 当前生成的精英怪数量
    /// </summary>
    public int CurrentEliteCount
    {
        get => LevelStepMgr.Instance.currentEliteCount;
        set => LevelStepMgr.Instance.currentEliteCount = value;
    }

    /// <summary>
    /// 最大精英怪生成数量
    /// </summary>
    public int MaxEliteCount
    {
        get => LevelStepMgr.Instance.maxEliteCount;
        set => LevelStepMgr.Instance.maxEliteCount = value;
    }
    /// <summary>
    /// 当前Boss生成数量
    /// </summary>
    public int CurrentBossCount
    {
        get => LevelStepMgr.Instance.currentBossCount;
        set => LevelStepMgr.Instance.currentBossCount = value;
    }

    /// <summary>
    /// 最大Boss生成数量
    /// </summary>
    public int MaxBossCount
    {
        get => LevelStepMgr.Instance.maxBossCount;
        set => LevelStepMgr.Instance.maxBossCount = value;
    } 

    public override void EnterState()
    {

        Debug.Log("进入MonsterCreatState");
        //增加怪物波次
        CurrentWave++;
        if (MonsterCounts <= 0)
        {
            Debug.Log("关卡怪物创建的总数量额度完成,不再创建");
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_DrawCard);
        }
        else
        {
            //创建这次要随机生成的数量
            int roundCount = CreatCurrentRoundCount();
            //如果数量大于关卡剩余怪物数量，直接用关卡剩余数量
            if (roundCount > MonsterCounts)
                roundCount = MonsterCounts;

            //获得真正创建成功的怪物数量
            //int realRoundCount = CreateMonsterAccordingWave(CurrentWave, roundCount);
            int realRoundCount = MonsterCreater.Instance.CreateMonster(DataCenter.Instance.monsterResNameData.Monster_Water01_WaterWisp, roundCount);

            //更细还需生成的怪物数量
            MonsterCounts -= realRoundCount;
            if (MonsterCounts < 0)
                MonsterCounts = 0;
            isMonsterCreting = false;
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

  /// <summary>
  /// 根据波数生成怪物
  /// </summary>
  /// <param name="currentWave">当前的波数</param>
  /// <param name="roundCount">该波次需要生成的怪物总量</param>
  /// <returns>正真成功生成的怪物总量</returns>
    public int CreateMonsterAccordingWave(int currentWave,int roundCount)
    {
        int realRoundCount = 0;
        string pathName;

        for (int i = 0; i < roundCount; i++)
        {
            
            if (currentWave == BossMonsterAppearWaveCount)//创建boss
            {
                bool canCreateBoss = CurrentBossCount < MaxBossCount;
                if (canCreateBoss)
                {
                    //生成Boss
                    pathName = DataCenter.Instance.monsterResNameData.Monster_None01_GodofAllElementalArts;
                    CurrentBossCount++;
                }
                else
                {
                    //Boss满了,直接随机普通怪
                    pathName = DataCenter.Instance.monsterResNameData.GetRandomBasicMonsterName();
                }
               
            }
            else if (currentWave < EliteMonsterAppearWaveCount)//刷新普通怪
            {
                pathName = DataCenter.Instance.monsterResNameData.GetRandomBasicMonsterName();
            }
            else//等于刷新精英怪的波次，开始刷精英怪
            {
                bool canCreateElite = CurrentEliteCount < MaxEliteCount;

                if (canCreateElite && Random.Range(0, 100) < EliteMonsterAppearProbability)
                {
                    // 生成精英
                    pathName = DataCenter.Instance.monsterResNameData.GetRandomEliteMonsterName();
                    CurrentEliteCount++;
                }
                else
                {
                    // 精英满了,直接随机普通怪
                    pathName = DataCenter.Instance.monsterResNameData.GetRandomBasicMonsterName();
                }
                //概率叠加
                EliteMonsterAppearProbability += EliteAppearGrowthProbability;
            }
            realRoundCount += MonsterCreater.Instance.CreateMonster(pathName, 1);
        }
        return realRoundCount;
    }

}
