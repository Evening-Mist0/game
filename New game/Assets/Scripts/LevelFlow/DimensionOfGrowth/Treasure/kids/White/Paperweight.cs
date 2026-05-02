using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paperweight : BaseTreasure
{
    //建筑物额外血量
    private int extraHp = 2;

    public int weight = 4;
    public override int round => -1;


    public override E_TreasureType type => E_TreasureType.Paperweight;

    public override void OnCreateDefTower(BaseDefTower tower)
    {
        Debug.Log("[镇纸]放置的建筑物血量额外增加" + extraHp);
        tower.maxHP += extraHp;
        tower.currentHP += extraHp;
        tower.effectControl.UpdateBlood(tower.currentHP,tower.maxHP);
    }

   
   
}
