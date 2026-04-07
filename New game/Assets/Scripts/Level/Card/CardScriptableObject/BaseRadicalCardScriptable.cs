using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseCardScriptable/部首卡槽SO")]

public class BaseRadicalCardScriptable : BaseCardScriptableData
{
    public override string MyResName => myResName;


    [Header("基础配置")]
    [Tooltip("卡牌资源路径")]
    public string myResName;
    [Tooltip("是否为卡槽（打牌面板记录卡牌获得个数那个）")]
    public bool isSlot;
    [Tooltip("卡牌位移速度（非卡槽）")]
    public float duration = 0.6f;


}
