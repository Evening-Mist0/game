using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InkWashViewControl : MonoBehaviour
{
    public Image srInk;
    public TMP_Text textInkValue;
    //原始图片宽度
    private float originLength = 0;
   
    

    public void UpdateInkValue(int currentInk,int maxInkValue)
    {
        textInkValue.text = currentInk.ToString() + "/" + maxInkValue.ToString();
    }

    /// <summary>
    /// 更新墨水图片
    /// </summary>
    public void UpdateSpriteInk(int currentInk, int maxInk)
    {
        if (currentInk < 0)
            return;


        if (originLength == 0)
            originLength = srInk.transform.localScale.x;

        // 防止以0做除数
        if (maxInk <= 0)
        {
            maxInk = 1;
            Debug.LogWarning("检测到最大墨水值小于等于零，请检查墨水设置");
            return;
        }

        float ratio = (currentInk / (float)maxInk);
        Debug.Log("计算出的比例为" + ratio);



        srInk.transform.localScale = new Vector3(originLength * ratio, srInk    .transform.localScale.y, 1);
        //更新text墨水值
        string strInk = currentInk.ToString() + "/" + maxInk.ToString();
        textInkValue.text = strInk;
    }
}
