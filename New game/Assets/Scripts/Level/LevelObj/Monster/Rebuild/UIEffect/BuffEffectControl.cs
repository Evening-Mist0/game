using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffEffectControl : MonoBehaviour
{
    public Dictionary<E_BuffIconType, BuffIconControl> buffIconControlDic = new Dictionary<E_BuffIconType, BuffIconControl>();

    private SpriteGridLayout layout;

    private void Awake()
    {
        layout = this.GetComponent<SpriteGridLayout>();
        if (layout == null)
            Debug.LogError("没有挂载SpriteGridLayout");
    }

    public void AddBuffIcon(E_BuffIconType type)
    {
        if (buffIconControlDic.ContainsKey(type))
            return;
        BuffIconControl control = CreateIcon(type);
        if (control != null)
        {
            buffIconControlDic.Add(type, control);
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
            default:
                obj = null;
                break;
        }

        if (obj == null)
        {
            DebugLogError($"Buff图标不存在：{type}");
            return null;
        }

        obj.transform.SetParent(transform, false);
        obj.transform.localScale = new Vector3(0.4f, 0.4f, 1);
        return obj.GetComponent<BuffIconControl>();
    }

    void DebugLogError(string str)
    {
        Debug.LogError("<color=red>BuffEffectControl:</color> " + str);
    }
}