using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffectBook : BaseBook
{

    public override E_BookShape E_BookShape => E_BookShape.Effect;

    public abstract EffectCardScriptable effectCardData {get;}

    public abstract CardSkillPair extraSkillAddition { get; }

    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard(card);

        if(card.bookType == BookType)
        {
            string path = "BaseCardScriptableObject/Level" + currentLevel.ToString() + "/Level" + currentLevel.ToString() + "_" + card.cardID;
            BaseCardScriptableData data = Resources.Load<BaseCardScriptableData>(path);
            if (data != null)
            {
                Debug.Log("[典籍升级]当前典籍的等级为" + currentLevel + "卡牌的描述为" + data.desEffection);
                card.UpdateSO(data);
            }
            else
                Debug.LogError($"{path}路径没有找到对应SO");
        }
       
    }

    public override void OnPrevSlected(BaseCardScriptableData data)
    {
        base.OnPrevSlected(data);


        bool isEqule = BookType == data.bookType ? true : false;
        Debug.Log("[典籍升级]书籍种类" + BookType + "数据种类" + data.bookType+"是否相等"+ isEqule);



        if (data.bookType == BookType)
        {
            string path = "BaseCardScriptableObject/Level" + currentLevel.ToString() + "/Level" + currentLevel.ToString() + "_" + data.cardID;
            Debug.Log($"[典籍升级] 尝试加载路径: {path}");
            BaseCardScriptableData newData = Resources.Load<BaseCardScriptableData>(path);

            if (newData != null)
            {
                Debug.Log("[典籍升级]当前典籍的等级为" + currentLevel + "卡牌的描述为" + newData.desEffection);
                data.desPrevComposite = newData.desPrevComposite;
                data.baseAtk = newData.baseAtk;
                data.baseRecRangeWide = newData.baseRecRangeWide;
                data.baseRecRangeHigh = newData.baseRecRangeHigh;
                data.isFirstActive = true;
            }
            else
            {
                Debug.LogError($"典籍升级失败！路径 {path} 没有找到对应 SO，请检查：\n" +
                               $"1. 资源是否放在 Resources 文件夹下\n" +
                               $"2. 文件名是否为 Level{currentLevel}_{data.cardID}\n" +
                               $"3. 文件扩展名是否为 .asset");
            }

        }
    }
}