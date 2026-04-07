using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "LevelUpOptionConfig", menuName = "游戏配置/成长/升级选项配置")]
public class LevelUpOptionConfigSO : ScriptableObject
{
    [Header("执照满级等级")]
    public int maxLevel = 5;
    [Header("每级所需经验")]
    public int expPerLevel = 2;
    [Header("升级选项池")]
    public List<LevelUpOptionConfig> optionPool = new List<LevelUpOptionConfig>();
}

[System.Serializable]
public class LevelUpOptionConfig
{
    [Header("选项类型")]
    public E_LevelUpOptionType optionType;
    [Header("选项名称")]
    public string optionName;
    [Header("选项描述")]
    public string optionDesc;
    [Header("是否为强力选项(避免重复)")]
    public bool isPowerfulOption;
}
