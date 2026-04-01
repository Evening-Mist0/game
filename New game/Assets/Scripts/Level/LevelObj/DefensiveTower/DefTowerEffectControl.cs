using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefTowerEffectControl : MonoBehaviour
{
    // 血条控件
    private BloodEffectControl bloodControl;
    private void Awake()
    {
        bloodControl = GetComponentInChildren<BloodEffectControl>();
        if (bloodControl == null)
            Debug.LogError("BloodEffectControl为空");
    }

    /// <summary>
    /// 更新血条显示
    /// </summary>
    public void UpdateBlood(int hp, int maxHp)
    {
        bloodControl.UpdateSpriteBlood(hp, maxHp);
    }

    /// <summary>
    /// 更新护甲
    /// </summary>
    /// <param name="nowDef"></param>
    public void UpdateDef(int nowDef)
    {
        bloodControl.UpdateSpriteDef(nowDef);
    }

    public void ShowDamageText(int damage,Vector3 worldPos)
    {
        GameObject obj = PoolMgr.Instance.GetObj("TextSpriteDamage");
        TextSpriteDamage text = obj.GetComponent<TextSpriteDamage>();
        text.ShowDamage(damage, worldPos);
    }
}
