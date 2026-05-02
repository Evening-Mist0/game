using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InkWashViewControl : MonoBehaviour
{
    public Image imgInk;
    public RectTransform rectInk;
    public TMP_Text textInkValue;
    //原始图片高度
    private float originHeight = 580;



    public void UpdateInkValue(int currentInk,int maxInkValue)
    {
        textInkValue.text = currentInk.ToString() + "/" + maxInkValue.ToString();
    }

    /// <summary>
    /// 更新墨水图片
    /// </summary>
    public void UpdateSpriteInk(int currentInk, int maxInk)
    {
        if (currentInk < 0) return;
        if (maxInk <= 0) maxInk = 1;

        // 第一次调用时记录原始高度
        if (originHeight == 0)
        {
            originHeight = imgInk.rectTransform.rect.height;
        }

        float ratio = currentInk / (float)maxInk;

        rectInk.sizeDelta = new Vector2(rectInk.sizeDelta.x, originHeight * ratio);

        ////设置高度
        //imgInk.rectTransform.anchoredPosition = imgInk.rectTransform.anchoredPosition;
        //imgInk.rectTransform.sizeDelta = new Vector2(imgInk.rectTransform.sizeDelta.x, targetHeight);

        // 更新文字
        textInkValue.text = $"{currentInk}/{maxInk}";

        //Debug.Log("【水墨控件】更新图片的新高度为" + targetHeight + "文字描述为"+ textInkValue.text);
    }
}

