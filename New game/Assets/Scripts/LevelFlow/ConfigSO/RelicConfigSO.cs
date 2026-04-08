using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RelicConfig", menuName = "游戏配置/成长/奇物配置")]
public class RelicConfigSO : ScriptableObject
{
    public List<RelicConfig> relicConfigs = new List<RelicConfig>();
}

[System.Serializable]
public class RelicConfig
{
    [Header("奇物唯一ID")]
    public string relicId;
    [Header("奇物品级")]
    public E_RelicQuality quality;
    [Header("奇物名称")]
    public string relicName;
    [Header("奇物描述")]
    public string relicDesc;
    [Header("奇物图标")]
    public Sprite relicIcon;
    [Header("掉落权重")]
    public int dropWeight;
}
