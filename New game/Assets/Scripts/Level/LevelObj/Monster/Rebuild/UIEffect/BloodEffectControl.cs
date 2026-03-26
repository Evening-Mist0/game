using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class BloodEffectControl : MonoBehaviour
{


    public SpriteRenderer srBlood;
    public SpriteRenderer srDef;
    public TMP_Text textBlood;
    public TMP_Text textDef;
    //原始图片宽度
    private float originLength = 0;



    /// <summary>
    /// 更新血量图片
    /// </summary>
    public void UpdateSpriteBlood(int hp,int maxHp)
    {
        if (hp < 0)
            return;


        if (originLength == 0)
            originLength = srBlood.transform.localScale.x;

        float ratio = (hp / (float)maxHp);
        Debug.Log("计算出的比例为" + ratio);

        // 按比例缩放
        srBlood.transform.localScale = new Vector3(originLength * ratio, srBlood.transform.localScale.y, 1);
        //更新text血量
        string strBlood = hp.ToString() + "/" + maxHp.ToString();
        textBlood.text = strBlood;
    }

    /// <summary>
    /// 更新护甲图片
    /// </summary>
    public void UpdateSpriteDef(int currentDef)
    {
        Debug.Log("[更新护甲血条]传入的防御值为" + currentDef);
        if (currentDef <= 0)
        {
            Debug.Log("[更新护甲血条]当前护甲值小于1，隐藏面板");
            srDef.gameObject.SetActive(false);
            textDef.gameObject.SetActive(false);
            textBlood.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("[更新护甲血条]当前护甲值大于0，显示面板");

            srDef.gameObject.SetActive(true);
            textDef.gameObject.SetActive(true);
            textBlood.gameObject.SetActive(false);
            textDef.text = currentDef.ToString();
        }
    }

}
