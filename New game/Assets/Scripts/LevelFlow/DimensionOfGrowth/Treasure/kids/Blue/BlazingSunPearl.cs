using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlazingSunPearl : BaseTreasure
{
    //火元素卡牌的暴击率
    private int doubleProb = 25;


  



    public override void OnPlay(BaseCard card)
    {
        bool isDouble = Random.Range(0, 100) < doubleProb;
        Debug.Log($"[艳阳宝珠]火元素卡牌暴击率判定，本次为{isDouble}");
        if(isDouble && card.elementType == E_Element.Fire)
        {
            card.currentAtk *= 2;
        }
    }


 

}
