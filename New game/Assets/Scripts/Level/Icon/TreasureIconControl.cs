using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TreasureIconControl : MonoBehaviour
{
    public Image imgTreasure;
    public Image imgNumber;
    public E_TreasureType myType;

    public bool isNumberImgVisible;
   
   

    public void UpdateMyIconCount(int count)
    {
        if (!isNumberImgVisible)
            return;
        if (count < 0 || count > 9)
            return;
        string path = "Number/" + count.ToString();
        imgNumber.sprite = Resources.Load<Sprite>(path);
    }

}
