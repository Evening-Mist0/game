using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物数据 ScriptableObject，用于配置怪物的基础属性和行为参数
/// </summary>
[CreateAssetMenu(fileName = "NewMonsterData", menuName = "Game/Monster/BaseMonsterData")]
public class BaseMonsterScriptableData : ScriptableObject
{
    #region 基础配置
    [Header("怪物基础配置")]
    [Tooltip("怪物唯一ID")]
    public string monsterID;

    [Tooltip("怪物显示名称")]
    public string monsterName;

    [Tooltip("最大生命值")]
    public int maxHp;

    [Tooltip("基础攻击力")]
    public int baseAtk;

    [Tooltip("基础防御力")]
    public int baseDef;

    [Tooltip("元素属性")]
    public MonsterElement element;

    [Tooltip("怪物身份（普通/精英/Boss）")]
    public MonsterIdentity identity;

    [Tooltip("怪物资源路径（预制体或模型资源名）")]
    public string MonsterResName;   
    #endregion

    #region 移动行为配置
    [Header("移动行为设置")]
    [Tooltip("基础横向移动步数/每回合")]
    public int baseMoveStepHorizontal = 1;

    [Tooltip("基础纵向移动步数/每回合，没有移动能力填 -1")]
    public int baseMoveStepVertical = 1;

    [Tooltip("移动间隔回合（1=每回合移动，2=每2回合移动）")]
    public int moveInterval = 1;

    [Tooltip("是否可以直接摧毁前方障碍物")]
    public bool couldDestroyDefAndAhead;
    #endregion
}