using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectCreater : BaseMonoMgr<EffectCreater>
{

    /// <summary>
    /// 在BaseGameObject上生成对应技能的特效
    /// </summary>
    /// <param name="type">生成哪个特效</param>
    /// <param name="obj">以这个对象作为特效对象的父物体</param>
    public void CreatEffect(E_AttackEffectType type, BaseGameObject obj)
    {
        GameObject effectObj;
        AttackEffectCore effectCore;
        switch (type)
        {
            case E_AttackEffectType.Burn:
            case E_AttackEffectType.Fire:
                effectObj = PoolMgr.Instance.GetObj(DataCenter.Instance.effectResNameData.Effect_Fire);
                break;
            case E_AttackEffectType.Water:
                effectObj = PoolMgr.Instance.GetObj(DataCenter.Instance.effectResNameData.Effect_Water);
                break;
            case E_AttackEffectType.Repel:
                effectObj = PoolMgr.Instance.GetObj(DataCenter.Instance.effectResNameData.Effect_Repel);
                break;
            case E_AttackEffectType.TrueDamage:
                effectObj = PoolMgr.Instance.GetObj(DataCenter.Instance.effectResNameData.Effect_TrueDamage);
                break;
            case E_AttackEffectType.Heal:
                effectObj = PoolMgr.Instance.GetObj(DataCenter.Instance.effectResNameData.Effect_Heal);
                break;
            case E_AttackEffectType.Imprison:
                effectObj = PoolMgr.Instance.GetObj(DataCenter.Instance.effectResNameData.Effect_Imprison);
                break;
            case E_AttackEffectType.GetDef:
                effectObj = PoolMgr.Instance.GetObj(DataCenter.Instance.effectResNameData.Effect_GetDef);
                break;
            case E_AttackEffectType.SpeedUp:
                effectObj = PoolMgr.Instance.GetObj(DataCenter.Instance.effectResNameData.Effect_SpeedUp);
                break;
            default:
                effectObj = null;
                Debug.Log("未找到传入该特效枚举的实例");
                break;

        }

        if (effectObj != null)
        {
            effectCore = effectObj.GetComponent<AttackEffectCore>();
            effectObj.transform.position = obj.transform.position;
            effectCore.target = obj.transform;

        }


    }
}
