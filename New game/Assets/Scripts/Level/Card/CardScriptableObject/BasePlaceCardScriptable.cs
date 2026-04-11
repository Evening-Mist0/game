using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BaseCardScriptable/放置类卡牌SO")]

public  class BasePlaceCardScriptable : BaseCardScriptableData
{

    public override string MyResName => myResName;

    public string myResName;
    public string myDefTowerResName;

    /// <summary>
    /// 放置类卡牌自带的给防御塔的血量加成
    /// </summary>
    public int extraDefTowerHp = 0;



}
