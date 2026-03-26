using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物BUFF处理组件
/// 负责：燃烧、禁锢、加速等效果的施加、持续、移除
/// </summary>
public class MonsterBuffHandler : MonoBehaviour
{
    private BaseMonsterCore owner;
    private MonsterEffectControl effectControl;

    [Header("BUFF持续回合")]
    public int burnLastCount;      // 燃烧剩余回合

    public int imprisonLastCount;  // 禁锢剩余回合
    public bool isImprison;//真正确认禁锢标识，持续回合会延迟结算禁锢效果保证移动正确


    public int speedUpLastCount;   // 加速剩余回合

    // 当前是否被禁锢


    // 所有生效中的BUFF列表
    private List<E_MonsterBuffType> activeBuffs = new List<E_MonsterBuffType>();

    public void Init(BaseMonsterCore monster, MonsterEffectControl effect)
    {
        owner = monster;
        effectControl = effect;
    }

    /// <summary>
    /// 施加BUFF效果
    /// </summary>
    /// <param name="type">BUFF类型</param>
    /// <param name="duration">持续回合</param>
    public void ApplyBuff(E_MonsterBuffType type, int duration)
    {
        if (!owner.isAllowedEffected) return;

        MonsterOnGetDeBuff evt = new MonsterOnGetDeBuff();
        owner.TriggerOnGetDeBuff(evt);

        switch (type)
        {
            case E_MonsterBuffType.Burn:
                if (evt.isImmunityBurn)
                    return;
                burnLastCount = duration;
                if (!activeBuffs.Contains(type))
                {
                    activeBuffs.Add(type);
                    //实例化图标
                    effectControl.AddBuffIcon(E_BuffIconType.Burn);
                    //更新回合持续时间
                    effectControl.UpdateIconCount(E_BuffIconType.Burn, burnLastCount);
                }
                break;

            case E_MonsterBuffType.Imprison:
                if (evt.isImmunityImprison)
                    return;
                imprisonLastCount = duration;
                isImprison = true;
                if (!activeBuffs.Contains(type))
                {
                    activeBuffs.Add(type);
                    //实例化图标
                    effectControl.AddBuffIcon(E_BuffIconType.Imprison);
                    //更新回合持续时间
                    effectControl.UpdateIconCount(E_BuffIconType.Imprison, imprisonLastCount);
                }
                break;

            case E_MonsterBuffType.SpeedUp:
                speedUpLastCount = duration;
                if (!activeBuffs.Contains(type))
                {
                    activeBuffs.Add(type);
                    //实例化图标
                    effectControl.AddBuffIcon(E_BuffIconType.SpeedUp);
                    //更新回合持续时间
                    effectControl.UpdateIconCount(E_BuffIconType.SpeedUp, speedUpLastCount);
                }
                break;
        }
    }

    /// <summary>
    /// 每回合更新，BUFF倒计时
    /// </summary>
    /// 
    public void OnRoundUpdate()
    {
        // 燃烧效果：每回合造成伤害
        if (burnLastCount > 0)
        {
            burnLastCount--;
            //更新图标显示回合数
            effectControl.UpdateIconCount(E_BuffIconType.Burn, burnLastCount);
            //持续回合为0清除图标
            if (burnLastCount <= 0)
                RemoveBuff(E_MonsterBuffType.Burn);
            owner.TakeDamage(BaseCard.burnAtk, E_Element.Fire, E_CardSkill.Burn, E_AtkType.Skill);
            if (GridMgr.Instance.cellDic.ContainsKey(owner.currentPos))
                effectControl.PlayBurnEffect(GridMgr.Instance.cellDic[owner.currentPos]);
            else
                Debug.LogError("怪物竟然处于GridMgr没有记录到的格子");

            Debug.Log($"{owner.name}受到燃烧伤害，剩余回合：{burnLastCount}");
        }
  

        // 禁锢效果
        if (imprisonLastCount > 0)
        {
            imprisonLastCount--;
            //更新图标显示回合数
            effectControl.UpdateIconCount(E_BuffIconType.Imprison, imprisonLastCount);
            //持续回合为0清除图标
            if (imprisonLastCount <= 0)
                RemoveBuff(E_MonsterBuffType.Imprison);
            if (GridMgr.Instance.cellDic.ContainsKey(owner.currentPos))
                effectControl.PlayImprisonEffect(GridMgr.Instance.cellDic[owner.currentPos]);
            else
                Debug.LogError("怪物竟然处于GridMgr没有记录到的格子");

            Debug.Log($"{owner.name}受到燃烧伤害，剩余回合：{burnLastCount}");
        }
        else if (activeBuffs.Contains(E_MonsterBuffType.Imprison))
        {
            RemoveBuff(E_MonsterBuffType.Imprison);
        }

        // 加速效果
        if (speedUpLastCount > 0)
        {
            speedUpLastCount--;
            effectControl.UpdateIconCount(E_BuffIconType.SpeedUp, speedUpLastCount);
            if (GridMgr.Instance.cellDic.ContainsKey(owner.currentPos))
                effectControl.PlaySpeedUpEffect(GridMgr.Instance.cellDic[owner.currentPos]);
            else
                Debug.LogError("怪物竟然处于GridMgr没有记录到的格子");

            Debug.Log($"{owner.name}受到燃烧伤害，剩余回合：{burnLastCount}");
        }
        else if (activeBuffs.Contains(E_MonsterBuffType.SpeedUp))
        {
            RemoveBuff(E_MonsterBuffType.SpeedUp);
        }
    }
  

    /// <summary>
    /// 移除指定BUFF
    /// </summary>
    private void RemoveBuff(E_MonsterBuffType type)
    {
        if (activeBuffs.Contains(type))
            activeBuffs.Remove(type);

        switch (type)
        {
            case E_MonsterBuffType.Burn:
                effectControl.RemoveBuffIcon(E_BuffIconType.Burn);
                break;
            case E_MonsterBuffType.Imprison:
                effectControl.RemoveBuffIcon(E_BuffIconType.Imprison);
                break;
            case E_MonsterBuffType.SpeedUp:
                effectControl.RemoveBuffIcon(E_BuffIconType.SpeedUp);
                break;
        }

        Debug.Log($"{owner.name}的{type}效果结束");
    }

    /// <summary>
    /// 清空所有BUFF
    /// </summary>
    public void ClearAllBuffs()
    {
        activeBuffs.Clear();
        burnLastCount = 0;
        imprisonLastCount = 0;
        speedUpLastCount = 0;
    }
}