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

        Debug.Log("进入MonsterCreatState");
        //增加怪物波次
        CurrentWave++;
        Debug.Log("当前怪物的总量" + info.monsterCounts);
        if (info.monsterCounts <= 0)
        {
            Debug.Log("关卡怪物创建的总数量额度完成,不再创建");
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_DrawCard);
        }
        else
        {
            //创建这次要随机生成的数量
            int roundCount = CreatCurrentRoundCount();
            //如果数量大于关卡剩余怪物数量，直接用关卡剩余数量
            if (roundCount > info.monsterCounts)
                roundCount = info.monsterCounts;

            //获得真正创建成功的怪物数量
            int realRoundCount = CreateMonsterAccordingWave(CurrentWave, roundCount);
            //int realRoundCount = MonsterCreater.Instance.CreateMonster(DataCenter.Instance.monsterResNameData.Monster_Earth01_StoneSprite, roundCount);

            //更新还需生成的怪物数量
            info.monsterCounts -= realRoundCount;
            if (info.monsterCounts < 0)
                info.monsterCounts = 0;
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
        Debug.LogWarning("测试调用，随机创建数固定为1");
        return Random.Range(1,3);
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
            
            if (currentWave == info.bossMonsterAppearWaveCount)//创建boss
            {
                bool canCreateBoss = CurrentBossCount < info.maxBossCount;
                if (canCreateBoss)
                {
                    //生成Boss
                    Debug.Log("[LevelStepMgr]生成Boss");

                    pathName = DataCenter.Instance.monsterResNameData.Monster_None01_GodofAllElementalArts;
                    CurrentBossCount++;
                }
                else
                {
                    //Boss满了,直接随机普通怪
                    Debug.Log("[LevelStepMgr]生成Boss波次，但是boss数量满了，生成普通怪");

                    pathName = DataCenter.Instance.monsterResNameData.GetRandomBasicMonsterName();
                }
               
            }
            else if (currentWave < info.eliteMonsterAppearWaveCount)//刷新普通怪
            {
                pathName = DataCenter.Instance.monsterResNameData.GetRandomBasicMonsterName();
            }
            else//等于刷新精英怪的波次，开始刷精英怪
            {
                bool canCreateElite = CurrentEliteCount < info.maxEliteCount;

                if (canCreateElite && Random.Range(0, 100) < info.eliteMonsterAppearProb)
                {
                    // 生成精英
                    Debug.Log("[LevelStepMgr]生成精英怪");

                    pathName = DataCenter.Instance.monsterResNameData.GetRandomEliteMonsterName();
                    CurrentEliteCount++;
                }
                else
                {
                    // 精英概率没随机到,直接随机普通怪
                    Debug.Log("[LevelStepMgr]生成精英怪成功率未达到，生成普通怪");

                    pathName = DataCenter.Instance.monsterResNameData.GetRandomBasicMonsterName();
                }
                //概率叠加
                info.eliteMonsterAppearProb += info.eliteAppearGrowthProb;
            }
            realRoundCount += MonsterCreater.Instance.CreateMonster(pathName, 1);
        }
        return realRoundCount;
    }

}
