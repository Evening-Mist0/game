using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffectBook : BaseBook
{

    public override E_BookShape E_BookShape => E_BookShape.Effect;

    public abstract EffectCardScriptable effectCardData {get;}
    /// <summary>
    /// 当前典籍卡牌持有的技能的 》额外加成！！《（有多个技能再加一个成员变量，通过构造函数初始化）
    /// </summary>
    public abstract CardSkillPair extraSkillAddition { get; }

    public override void BookOnCreateNewCard(BaseCard card)
    {
        base.BookOnCreateNewCard(card);

        if(card.bookType == BookType)//如果书的种类对应，那就进行升级判断，根据等级读取典籍卡牌的所有配置数据
        {
            //根据典籍的等级，获得对应等级卡牌的数据
            string path = "BaseCardScriptableObject/Level" + currentLevel.ToString() + "/Level" + currentLevel.ToString() + "_" + card.cardID;
            EffectCardScriptable data = Resources.Load<EffectCardScriptable>(path);
            Debug.Log("[典籍升级]当前典籍的等级为" + currentLevel + "卡牌的描述为" + data.desEffection);

            if (data != null)
                card.UpdateSO(data);
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
            EffectCardScriptable newData = Resources.Load<EffectCardScriptable>(path);
            Debug.Log("[典籍升级]当前典籍的等级为" + currentLevel + "卡牌的描述为" + newData.desEffection);

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
