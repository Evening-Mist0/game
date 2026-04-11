using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;



public class PlayerBag : MonoBehaviour
{
    //public List<BaseTreasure> treasures = new List<BaseTreasure>();
    public List<I_Treasure> treasures = new List<I_Treasure>();

    private List<BaseBook> books = new List<BaseBook>();

    private List<BasePlayerSkill> skills = new List<BasePlayerSkill>();


    #region 奇物
    public void AddTreasure(string treasureID)
    {
        string className = treasureID;
        Assembly assembly = Assembly.GetExecutingAssembly();
        Type type = assembly.GetType(className);

        if (type != null && typeof(I_Treasure).IsAssignableFrom(type))
        {
            // 创建新实例
            I_Treasure treasure = Activator.CreateInstance(type) as I_Treasure;
            if (treasure != null)
            {
                if (!treasures.Contains(treasure))
                {
                    treasures.Add(treasure);
                    Debug.Log($"成功添加奇物: {treasureID}");
                }
            }
        }
        else
        {
            Debug.LogWarning($"未找到奇物效果类: {className}");
        }
    }


    public void RemoveTreasure(string treasureID)
    {
        // 需要先根据 ID 找到对应的 treasure 实例
        I_Treasure treasure = FindTreasureByID(treasureID);

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
    private I_Treasure FindTreasureByID(string treasureID)
    {
        string targetClassName = $"RelicEffects.{treasureID}Effect";

        foreach (var treasure in treasures)
        {
            if (treasure.GetType().FullName == targetClassName ||
                treasure.GetType().Name == $"{treasureID}Effect")
            {
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
    public void OnSynthesis(BaseCard card)
    {
        for (int i = 0; i < treasures.Count; i++)
        {
            treasures[i].OnSynthesis(card);
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
    #endregion


    #region 典籍

    public void AddBook(E_BookType type)
    {
        BaseBook book = CreateBook(type);
        if (book == null) return;

        if (!books.Exists(b => b.BookType == type))
            books.Add(book);
    }

    public void RemoveBook(E_BookType type)
    {
        var book = books.FirstOrDefault(b => b.BookType == type);
        if (book != null)
            books.Remove(book);
    }

    /// <summary>
    /// 创建书籍
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private BaseBook CreateBook(E_BookType type)
    {
        return type switch
        {
            E_BookType.Fire_LiaoYuan => new FireBook(),
            E_BookType.Water_BaiChuan => new WaterBook(),
            E_BookType.Earth_HouTu => new EarthBook(),
            E_BookType.Wood_KuRong => new WoodBook(),
            E_BookType.Battle_PoWang => new WarBook(),
            _ => null
        };
    }

    public void BookOnComposite(BaseCard card)
    {
        for (int i = 0; i < books.Count; i++)
        {
            books[i].OnComposite(card);
        }
    }

    public void BookOnPlay(BaseCard card)
    {
        Debug.Log("[典籍]触发卡牌打出典籍效果");
        for (int i = 0; i < books.Count; i++)
        {
            books[i].OnPlay(card);
            Debug.Log("[典籍]触发典籍效果" + books[i].BookType);

        }
    }

    #endregion


    #region 技能
    public void AddSkill(E_LevelUpOptionType type)
    {
        BasePlayerSkill skill = CreateSkill(type);
        if (skill == null) return;

        if (!skills.Exists(s => s.SkillType == type))
        {
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
