using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerGrowthData
{
    /// <summary>
    /// 玩家基础属性
    /// </summary>
    //玩家最大血量
    public int playerMaxHp;
    //玩家当前血量
    public int playerCurrentHp;
    //玩家当前护甲
    public int playerCurrentArmor;
    

    /// <summary>
    /// 执照系统
    /// </summary>
    //当前执照等级
    public int licenseLevel;
    //当前执照经验
    public int licenseExp;
    //已选择的升级选项列表
    public List<E_LevelUpOptionType> selectedLevelUpOptions = new List<E_LevelUpOptionType>();
    

    /// <summary>
    /// 典籍系统
    /// </summary>
    //已获得的典籍列表 单局上限2本
    public List<E_BookType> ownedBooks = new List<E_BookType>();
    public readonly int maxBookCount = 2;
    

    /// <summary>
    /// 奇物系统
    /// </summary>
    //已获得的奇物列表
    public List<string> ownedRelicIds = new List<string>();
   

    // 重置成长数据
    public void ResetData(int initMaxHp, int initHp)
    {
        // 基础属性重置
        playerMaxHp = initMaxHp;
        playerCurrentHp = initHp;
        playerCurrentArmor = 0;

        // 执照重置
        licenseLevel = 0;
        licenseExp = 0;
        selectedLevelUpOptions.Clear();

        // 典籍奇物重置
        ownedBooks.Clear();
        ownedRelicIds.Clear();
    }
}
