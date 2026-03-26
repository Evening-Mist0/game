using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.U2D;

public class BloodEffectControl : MonoBehaviour
{


    public SpriteRenderer srBlood;
    public TMP_Text textBlood;
    //原始图片宽度
    private float originLength = 0;



    /// <summary>
    /// 更新血量图片
    /// </summary>
    public void UpdateSpriteBlood(int hp,int maxHp)
    {
        if (hp < 0)
            return;

        if(originLength == 0)
            originLength = srBlood.transform.localScale.x;

        float ratio = (hp / (float)maxHp);
        Debug.Log("计算出的比例为" + ratio);

        // 按比例缩放
        srBlood.transform.localScale = new Vector3(originLength * ratio, srBlood.transform.localScale.y, 1);
        //更新text血量
        string strBlood = hp.ToString() + "/" + maxHp.ToString();
        textBlood.text = strBlood;
    }

}
