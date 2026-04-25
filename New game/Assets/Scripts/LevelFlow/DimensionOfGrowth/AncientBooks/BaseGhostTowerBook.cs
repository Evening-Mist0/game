using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGhostTowerBook : BaseBook
{

    

    /// <summary>
    /// 建筑物的存在回合(幽灵防御塔)
    /// </summary>
    public int extraExistRound;

    public abstract BasePlaceCardScriptable placeCardData { get; }



    public override E_BookShape E_BookShape => E_BookShape.Tower;

    public abstract GhostTowerScriptableData ghostTowerData { get; }

    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard(card);


        if (card.bookType == BookType)//如果书的种类对应，那就进行升级判断，根据等级读取典籍卡牌的所有配置数据
        {
            //根据典籍的等级，获得对应等级卡牌的数据
           string path = "BaseCardScriptableObject/Level" + currentLevel.ToString() + "/Level" + currentLevel.ToString() + "_" + card.cardID;      
            BasePlaceCardScriptable data = Resources.Load<BasePlaceCardScriptable>(path);

            Debug.Log("[典籍升级]当前典籍的等级为" + currentLevel + "卡牌的描述为" + data.desEffection);

            if (data != null)
                card.UpdateSO(data);
            else
                Debug.LogError($"{path}路径没有找到对应SO");
        }          
    }

    public override void BookOnCreateNewDefTower(BaseDefTower tower)
    {
        base.BookOnCreateNewDefTower(tower);

        if (tower.bookType == BookType)//如果书的种类对应，那就进行升级判断，根据等级读取典籍卡牌的所有配置数据
        {
            //根据典籍当前等级，实例化对应等级数据
            string path = "BaseGhostTowerSO/Level" + currentLevel.ToString()+ "/Level" + currentLevel.ToString() + "_" + tower.myTowerName.ToString();

            GhostTowerScriptableData data = Resources.Load<GhostTowerScriptableData>(path);

            Debug.Log("获得新数据的名字为" + data.towerName + "持续回合为" + data.existRound);
            if (data != null)
                tower.UpdateSO(data);
            else
                Debug.LogError($"{path}路径没有找到对应SO");
        }
    }

    public override void OnPrevSlected(BaseCardScriptableData data)
    {
        base.OnPrevSlected(data);
        if (data.bookType == BookType)//如果书的种类对应，那就进行升级判断，根据等级读取典籍卡牌的所有配置数据
        {
            //根据典籍的等级，获得对应等级卡牌的数据
            string path = "BaseCardScriptableObject/Level" + currentLevel.ToString() + "/Level" + currentLevel.ToString() + "_" + data.cardID;
            BasePlaceCardScriptable newData = Resources.Load<BasePlaceCardScriptable>(path);
            Debug.Log("[典籍升级]当前典籍的等级为" + currentLevel+"卡牌的描述为"+newData.desEffection);

            if (data != null)
            {
                data.desPrevComposite = newData.desPrevComposite;
                data.baseAtk = newData.baseAtk;
                data.baseRecRangeWide = newData.baseRecRangeWide;
                data.baseRecRangeHigh = newData.baseRecRangeHigh;
                data.isFirstActive = true;
            }
            else
                Debug.LogError($"{path}路径没有找到对应SO");
        }
    }
}
