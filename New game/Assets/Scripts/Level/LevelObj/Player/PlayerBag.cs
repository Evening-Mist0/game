using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;



public class PlayerBag : MonoBehaviour
{
    //public List<BaseTreasure> treasures = new List<BaseTreasure>();
    public List<BaseTreasure> treasures = new List<BaseTreasure>();

    public Dictionary<E_BookType, BaseBook> books = new Dictionary<E_BookType, BaseBook>();

    private List<BasePlayerSkill> skills = new List<BasePlayerSkill>();


    #region 奇物
    public void AddTreasure(string treasureID)
    {
        string className = treasureID;
        // 使用 BaseTreasure 所在的程序集，确保奇物子类在同一程序集中
        Assembly assembly = typeof(BaseTreasure).Assembly;
        Type type = assembly.GetType(className);

        if (type != null && typeof(BaseTreasure).IsAssignableFrom(type))
        {
            BaseTreasure treasure = Activator.CreateInstance(type) as BaseTreasure;
            if (treasure != null && !treasures.Contains(treasure))
            {
                treasures.Add(treasure);
                Debug.Log($"成功添加奇物: {treasureID}");
            }
        }
        else
        {
            Debug.LogWarning($"未找到奇物效果类: {className}，程序集: {assembly.FullName}");
            // 可选：打印程序集中所有类型名称，帮助调试
            var allTypes = assembly.GetTypes();
            Debug.Log($"程序集中包含的类型: {string.Join(", ", allTypes.Select(t => t.Name))}");
        }
    }


    public void RemoveTreasure(string treasureID)
    {
        // 需要先根据 ID 找到对应的 treasure 实例
        BaseTreasure treasure = FindTreasureByID(treasureID);

        if (treasure != null && treasures.Contains(treasure))
        {
            treasures.Remove(treasure);
            Debug.Log($"成功移除奇物: {treasureID}");
        }
        else
        {
            Debug.LogWarning($"未找到奇物: {treasureID}");
        }
    }


    // 辅助方法：根据 ID 查找实例
    private BaseTreasure FindTreasureByID(string treasureID)
    {
        // 直接比较类型名称是否等于 treasureID
        foreach (var treasure in treasures)
        {
            Debug.Log($"进行判定{treasure.GetType().Name} = {treasureID}");
            if (treasure.GetType().Name == treasureID)
            {
                Debug.Log("判定成功");
                return treasure;
            }
          
        }
        return null;
    }



    /// <summary>
    /// 触发所有奇物的抽牌效果
    /// </summary>
    public void OnDrawCard(BaseCard card)
    {
        for (int i = 0; i < treasures.Count; i++)
        {
            treasures[i].OnDrawCard(card);
        }
    }

    /// <summary>
    /// 触发所有奇物的打牌效果
    /// </summary>
    public void OnPlay(BaseCard card)
    {
        for (int i = 0; i < treasures.Count; i++)
        {
            treasures[i].OnPlay(card);
        }
    }

    /// <summary>
    /// 触发所有奇物的放置阻挡物效果
    /// </summary>
    public void OnCreateDefTower(BasePlaceCard card)
    {
        for (int i = 0; i < treasures.Count; i++)
        {
            treasures[i].OnCreateDefTower(card);
        }
    }

    /// <summary>
    /// 触发所有奇物的合成效果
    /// </summary>
    public void OnSynthesisSuccessed(BaseCard card)
    {
        for (int i = 0; i < treasures.Count; i++)
        {
            treasures[i].OnSynthesisSuccessed(card);
        }
    }

    public void OnPlayFinish(BaseCard card)
    {
        for (int i = 0; i < treasures.Count; i++)
        {
            treasures[i].OnPlayFinish(card);
        }
    }


    /// <summary>
    /// 重置所有奇物的临时状态(点击结束回合按钮时候)
    /// </summary>
    public void ResetOnClickOverTurn()
    {
        for (int i = 0; i < treasures.Count; i++)
        {
            treasures[i].ResetOnClickOverTurn();
        }
    }

    /// <summary>
    /// 重置所有奇物的临时状态(当前关卡结束时)
    /// </summary>
    public void ResetOnLevelOver()
    {
        for (int i = 0; i < treasures.Count; i++)
        {
            treasures[i].ResetOnLevelOver();
        }
    }

    public void OnPrevSlected(BaseCardScriptableData data)
    {
        for (int i = 0; i < treasures.Count; i++)
        {
            treasures[i].OnPrevSlected(data);
        }
    }

    public void OnCancelPrevSlected(BaseCardScriptableData data)
    {
        for (int i = 0; i < treasures.Count; i++)
        {
            treasures[i].OnCancelPrevSlected(data);
        }
    }


    #endregion


    #region 典籍
    public void AddBook(E_BookType type)
    {
        BaseBook book = CreateBook(type);
        if (book == null) return;

        // 使用字典的 ContainsKey 检查是否已存在
        if (!books.ContainsKey(type))
        {
            books.Add(type, book);
        }
    }

    public void RemoveBook(E_BookType type)
    {
        if (books.ContainsKey(type))
        {
            books.Remove(type);
        }
    }

    /// <summary>
    /// 创建书籍实例
    /// </summary>
    private BaseBook CreateBook(E_BookType type)
    {
        return type switch
        {
            E_BookType.Fire_Xie => new FireBook_Xie(),
            E_BookType.Fire_Fen => new FireBook_Fen(),
            E_BookType.Fire_Yi => new FireBook_Yi(),
            E_BookType.Water_Miao => new WaterBook_Miao(),
            E_BookType.Water_Chi => new WaterBook_Chi(),
            E_BookType.Water_Lin => new WaterBook_Lin(),
            E_BookType.Earth_Yao => new EarthBook_Yao(),
            E_BookType.Earth_Zhuo => new EarthBook_Zhuo(),
            E_BookType.Wood_Yi => new WoodBook_Yi(),
            E_BookType.Wood_Bi => new WoodBook_Bi(),
            _ => null
        };
    }

    public BaseBook GetBook(E_BookType type)
    {
        if (books.ContainsKey(type))
            return books[type];
        return null;
    }

    public void BookOnCreateNewDefTower(BaseDefTower tower)
    {
        // 遍历字典中的所有典籍值
        foreach (var book in books.Values)
        {
            book.BookOnCreateNewDefTower(tower);
        }
    }
    public void BookOnCreateNewCard(BaseCard newCard)
    {
        // 遍历字典中的所有典籍值
        foreach (var book in books.Values)
        {
            book.BookOnCreateNewCard(newCard);
        }
    }

    public void BookOnPlay(BaseCard card)
    {
        Debug.Log("[典籍]触发卡牌打出典籍效果");
        foreach (var book in books.Values)
        {
            book.OnPlay(card);
            Debug.Log("[典籍]触发典籍效果" + book.BookType);
        }
    }

    public void BookOnPrevSlected(BaseCardScriptableData data)
    {
        Debug.Log("[典籍]触发卡牌选中效果");
        foreach (var book in books.Values)
        {
            book.OnPrevSlected(data);
        }
    }
    #endregion


    #region 技能
    public void AddSkill(E_LevelUpOptionType type)
    {
        BasePlayerSkill skill = CreateSkill(type);
        Debug.Log("玩家获得技能" + skill);
        if (skill == null) return;

        if (!skills.Exists(s => s.SkillType == type))
        {
            Debug.Log("确认没获得过该技能，进行获得" + skill);

            skill.OnGetSkill();
            skills.Add(skill);           
        }
    }

    public void RemoveSkill(E_LevelUpOptionType type)
    {
        var skill = skills.FirstOrDefault(s => s.SkillType == type);
        if (skill != null)
            skills.Remove(skill);
    }

    /// <summary>
    /// 创建技能
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private BasePlayerSkill CreateSkill(E_LevelUpOptionType type)
    {
        return type switch
        {
            E_LevelUpOptionType.HpMaxAdd => new HpMaxAddSkill(),
            E_LevelUpOptionType.InitArmor => new InitArmorSkill(),
            E_LevelUpOptionType.HandCardMaxAdd => new HandCardMaxAddSkill(),
            E_LevelUpOptionType.DrawCardSpeedUp => new DrawCardSpeedUpSkill(),
            E_LevelUpOptionType.InkGrowthAddSkill => new InkGrowthAddSkill(),
            _ => null
        };
    }


    /// <summary>
    /// 清空所有物品（奇物、典籍、技能等），在玩家死亡或者通关时调用
    /// </summary>
    public void ClearAllItems()
    {
        // 清空奇物
        treasures.Clear();
        // 清空典籍
        books.Clear();

        //重置玩家基础属性
        for(int i = 0; i < skills.Count; i++)
        {
            skills[i].OnSetClear();
        }
        // 清空技能
        skills.Clear();
    }
  
    #endregion

}
