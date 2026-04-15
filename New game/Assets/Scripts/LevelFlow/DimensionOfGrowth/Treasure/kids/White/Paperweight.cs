using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paperweight : BaseTreasure
{
    //建筑物额外血量
    private int extraHp = 2;

    public int weight = 4;


    public override void OnCreateDefTower(BasePlaceCard card)
    {
        Debug.Log("[镇纸]放置的建筑物血量额外增加" + extraHp);
        card.currentExtraDefTowerHp += extraHp;
    }

   
   
}
