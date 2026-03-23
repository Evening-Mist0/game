//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 怪物状态效果组件，管理灼烧、禁锢等持续效果
///// </summary>
//public class MonsterBuffHandler : MonoBehaviour
//{
//    private BaseMonsterCore owner;
//    private MonsterEffectControl effectControl;

//    // 当前激活的效果列表
//    private List<E_MonsterBuffType> activeBuffs = new List<E_MonsterBuffType>();

//    // 各效果剩余回合
//    private int burnLastCount;
//    private int imprisonLastCount;
//    private bool isImprison;

//    public bool IsImprisoned => isImprison;

//    public void Init(BaseMonsterCore monster, MonsterEffectControl effect)
//    {
//        owner = monster;
//        effectControl = effect;
//        activeBuffs.Clear();
//        burnLastCount = 0;
//        imprisonLastCount = 0;
//        isImprison = false;
//    }

//    /// <summary>
//    /// 应用一个持续效果
//    /// </summary>
//    public void ApplyBuff(E_MonsterBuffType type, int duration)
//    {
//        // 触发获取效果特性，子类可免疫
//        MonsterOnGetDeBuff evt = new MonsterOnGetDeBuff { skill = ConvertToCardSkill(type) };
//        owner.TriggerOnGetDeBuff(evt);
//        if (evt.isImmunity)
//            return;

//        switch (type)
//        {
//            case E_MonsterBuffType.Burn:
//                if (burnLastCount < duration)
//                {
//                    burnLastCount = duration;
//                    effectControl.DisplayIcon(E_IconType.Burn);
//                }
//                AddBuffToList(type);
//                break;
//            case E_MonsterBuffType.Imprison:
//                if (imprisonLastCount < duration)
//                {
//                    imprisonLastCount = duration;
//                    effectControl.DisplayIcon(E_IconType.Imprison);
//                }
//                isImprison = true;
//                AddBuffToList(type);
//                break;
//            default:
//                Debug.LogWarning($"未处理的效果类型：{type}");
//                break;
//        }
//    }

//    /// <summary>
//    /// 移除一个效果
//    /// </summary>
//    private void RemoveBuff(E_MonsterBuffType type)
//    {
//        activeBuffs.Remove(type);
//        switch (type)
//        {
//            case E_MonsterBuffType.Burn:
//                burnLastCount = 0;
//                effectControl.DestoryIcon(E_IconType.Burn);
//                break;
//            case E_MonsterBuffType.Imprison:
//                imprisonLastCount = 0;
//                isImprison = false;
//                effectControl.DestoryIcon(E_IconType.Imprison);
//                break;
//        }
//    }

//    private void AddBuffToList(E_MonsterBuffType type)
//    {
//        if (!activeBuffs.Contains(type))
//            activeBuffs.Add(type);
//    }

//    private E_CardSkill ConvertToCardSkill(E_MonsterBuffType buff)
//    {
//        switch (buff)
//        {
//            case E_MonsterBuffType.Burn: return E_CardSkill.Burn;
//            case E_MonsterBuffType.Imprison: return E_CardSkill.Imprison;
//            default: return E_CardSkill.None;
//        }
//    }

//    /// <summary>
//    /// 每回合结算效果（由基类调用）
//    /// </summary>
//    public void OnRoundUpdate()
//    {
//        // 遍历副本，因为可能在循环中修改 activeBuffs
//        List<E_MonsterBuffType> tempList = new List<E_MonsterBuffType>(activeBuffs);
//        foreach (var buff in tempList)
//        {
//            switch (buff)
//            {
//                case E_MonsterBuffType.Burn:
//                    burnLastCount--;
//                    if (burnLastCount <= 0)
//                    {
//                        RemoveBuff(E_MonsterBuffType.Burn);
//                        break;
//                    }
//                    // 造成灼烧伤害（固定伤害值可配置，这里用 BaseCard.burnAtk）
//                    owner.TakeDamage(BaseCard.burnAtk, E_CardSkill.Burn);
//                    break;
//                case E_MonsterBuffType.Imprison:
//                    imprisonLastCount--;
//                    Debug.Log($"禁锢剩余回合：{imprisonLastCount}");
//                    if (imprisonLastCount <= 0)
//                    {
//                        RemoveBuff(E_MonsterBuffType.Imprison);
//                    }
//                    break;
//                case E_MonsterBuffType.SpeedUp:
//                    // 速度加成处理（如有需要）
//                    Debug.Log($"{owner.monsterName} 速度加成生效");
//                    break;
//                default:
//                    Debug.LogWarning($"未知的Buff类型：{buff}");
//                    break;
//            }
//        }
//    }
//}