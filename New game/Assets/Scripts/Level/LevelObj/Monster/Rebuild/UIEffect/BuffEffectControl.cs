using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffectControl : MonoBehaviour
{
    public Dictionary<E_BuffIconType, BuffIconControl> buffIconControlDic = new Dictionary<E_BuffIconType, BuffIconControl>();

    private SpriteGridLayout layout;

    //图标的大小scale
    private Vector3 iconScale = new Vector3(0.6f, 0.6f, 1f);

    private void Awake()
    {
        layout = this.GetComponent<SpriteGridLayout>();
        if (layout == null)
            Debug.LogError($"{this.gameObject.name}没有挂载SpriteGridLayout");
        else
            Debug.Log("SpriteGridLayout获取成功");
    }

    public void AddBuffIcon(E_BuffIconType type)
    {
        if (buffIconControlDic.ContainsKey(type))
            return;
        BuffIconControl control = CreateIcon(type);
        if (control != null)
        {
            buffIconControlDic.Add(type, control);
            if(layout)
            layout.RefreshLayout();
        }
    }

    public void RemoveBuffIcon(E_BuffIconType type)
    {
        if (buffIconControlDic.TryGetValue(type, out BuffIconControl control))
        {
            GameObject obj = control.gameObject;
            obj.transform.SetParent(null);
            PoolMgr.Instance.PushObj(obj);
            buffIconControlDic.Remove(type);
            StartCoroutine(DelayedRefresh());
        }
    }

    private IEnumerator DelayedRefresh()
    {
        yield return null; // 等待一帧
        layout.RefreshLayout();
    }

    public void UpdateIconCount(E_BuffIconType type, int lastCount)
    {
        Debug.Log($"更新图标{type}，更新的数字为{lastCount}");
        if (buffIconControlDic.ContainsKey(type))
        {
            buffIconControlDic[type].UpdateMyIconCount(lastCount);
        }
    }

    private BuffIconControl CreateIcon(E_BuffIconType type)
    {
        if (buffIconControlDic.ContainsKey(type))
            return null;

        GameObject obj = null;
        switch (type)
        {
            case E_BuffIconType.Heal:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_Heal);
                break;
            case E_BuffIconType.Burn:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_Burn);
                break;
            case E_BuffIconType.ImmunityBurn:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_ImmunityBurn);
                break;
            case E_BuffIconType.Imprison:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_Imprison);
                break;
            case E_BuffIconType.ImmunityImprison:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_ImmunityImprison);
                break;
            case E_BuffIconType.SpeedUp:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_SpeedUp);
                break;
            case E_BuffIconType.Reflect:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_Reflect);
                break;
            case E_BuffIconType.ArbitraryDamegeRedution:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_ArbitraryDamegeRedution);
                break;
            case E_BuffIconType.FireDamegeRedution:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_FireDamegeRedution);
                break;
            case E_BuffIconType.AnnihilationOfElements:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_AnnihilationOfElements);
                break;
            case E_BuffIconType.DestroyBuildings:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_DestroyBuildings);
                break;
            case E_BuffIconType.GetDef:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_GetDef);
                break;
            case E_BuffIconType.DeadReflect:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_DeadReflect);
                break;
            case E_BuffIconType.AddBloodToMonster:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_AddBloodToMonster);
                break;
            case E_BuffIconType.Move:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_Move);
                break;
            case E_BuffIconType.Weakness:
                obj = PoolMgr.Instance.GetObj(DataCenter.Instance.buffIconResNameData.BuffIcon_Weakness);
                break;
            default:
                obj = null;
                break;
        }

        if (obj == null)
        {
            DebugLogError($"Buff图标不存在：{type}");
            return null;
        }

        obj.transform.SetParent(this.transform, false);
        obj.transform.localScale = iconScale;
        return obj.GetComponent<BuffIconControl>();
    }

    void DebugLogError(string str)
    {
        Debug.LogError("<color=red>BuffEffectControl:</color> " + str);
    }
}