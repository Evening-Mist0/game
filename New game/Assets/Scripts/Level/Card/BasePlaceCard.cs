using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlaceCard : BaseCard
{
    [HideInInspector]
    public string myDefTowerResName;
    [HideInInspector]
    //放置类卡牌当前实体的血量
    public int extraDefTowerHp;



  
    protected override void InitCardValue()
    {
        base.InitCardValue();
        BasePlaceCardScriptable placeCardData = cardData as BasePlaceCardScriptable;
        if (placeCardData == null)
        {
            Debug.LogError("里氏替换失败,该SO不属于BasePlaceCardScriptable");
            return;
        }
        myDefTowerResName = placeCardData.myDefTowerResName;

    }


}
