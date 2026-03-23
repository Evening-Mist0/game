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
                    activeBuffs.Add(type);
                break;

            case E_MonsterBuffType.Imprison:
                if (evt.isImmunityImprison)
                    return;
                imprisonLastCount = duration;
                isImprison = true;
                if (!activeBuffs.Contains(type))
                    activeBuffs.Add(type);
                break;

            case E_MonsterBuffType.SpeedUp:
                speedUpLastCount = duration;
                if (!activeBuffs.Contains(type))
                    activeBuffs.Add(type);
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
            owner.TakeDamage(BaseCard.burnAtk, E_Element.Fire, E_CardSkill.Burn, E_AtkType.Skill);
            Debug.Log($"{owner.name}受到燃烧伤害，剩余回合：{burnLastCount}");
        }
        else if (activeBuffs.Contains(E_MonsterBuffType.Burn))
        {
            RemoveBuff(E_MonsterBuffType.Burn);
        }

        // 禁锢效果
        if (imprisonLastCount > 0)
        {
            imprisonLastCount--;
        }
        else if (activeBuffs.Contains(E_MonsterBuffType.Imprison))
        {
            RemoveBuff(E_MonsterBuffType.Imprison);
        }

        // 加速效果
        if (speedUpLastCount > 0)
        {
            speedUpLastCount--;
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
                break;
            case E_MonsterBuffType.Imprison:
                break;
            case E_MonsterBuffType.SpeedUp:
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