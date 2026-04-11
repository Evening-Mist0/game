using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterResNameData 
{
    public string Monster_Fire01_FlameSprite;
    public string Monster_Fire02_CombustionWorm;
    public string Monster_Fire03_MoltenGuard;
    public string Monster_Water01_WaterWisp;
    public string Monster_Water02_TideSoldier;
    public string Monster_Water03_AbyssEel;
    public string Monster_Earth01_StoneSprite;
    public string Monster_Earth02_ShieldGuard;
    public string Monster_Earth03_StoneGiant;
    public string Monster_None01_GodofAllElementalArts;

    //普通怪物数组
    private string[] basicMonsterNameArray;
    //普通怪物数组容量（随机数范围）
    private int basicMonsterValidCount;

    //分类后的普通怪物数组
    private string[] fireBasicMonsters;
    private string[] waterBasicMonsters;
    private string[] earthBasicMonsters;

    //装载精英怪物怪物资源路径的数组
    private string[] eliteMonsterNameArray;
    //精英怪物数组容量（随机数范围）
    private int eliteMonsterValidCount;

    //分类后的精英怪物数组
    private string[] fireEliteMonsters;
    private string[] waterEliteMonsters;
    private string[] earthEliteMonsters;

    public void Initialize() // 在JSON加载完成后调用
    {
        var tempList = new List<string>
        {
            Monster_Fire01_FlameSprite,
            Monster_Fire02_CombustionWorm,
            Monster_Water01_WaterWisp,
            Monster_Water02_TideSoldier,
            Monster_Earth01_StoneSprite,
            Monster_Earth02_ShieldGuard,
        };
        tempList.RemoveAll(string.IsNullOrEmpty);

        basicMonsterNameArray = tempList.ToArray();
        basicMonsterValidCount = basicMonsterNameArray.Length;

        // 预先分类普通怪物
        fireBasicMonsters = basicMonsterNameArray.Where(name => name.Contains("Fire")).ToArray();
        waterBasicMonsters = basicMonsterNameArray.Where(name => name.Contains("Water")).ToArray();
        earthBasicMonsters = basicMonsterNameArray.Where(name => name.Contains("Earth")).ToArray();

        var tempList2 = new List<string>
        {
            Monster_Fire03_MoltenGuard,
            Monster_Water03_AbyssEel,
            Monster_Earth03_StoneGiant,
        };
        tempList2.RemoveAll(string.IsNullOrEmpty);

        eliteMonsterNameArray = tempList2.ToArray();
        eliteMonsterValidCount = eliteMonsterNameArray.Length;

        // 预先分类精英怪物
        fireEliteMonsters = eliteMonsterNameArray.Where(name => name.Contains("Fire")).ToArray();
        waterEliteMonsters = eliteMonsterNameArray.Where(name => name.Contains("Water")).ToArray();
        earthEliteMonsters = eliteMonsterNameArray.Where(name => name.Contains("Earth")).ToArray();
    }

    /// <summary>
    /// 获取随机普通怪物名称
    /// </summary>
    public string GetRandomBasicMonsterName()
    {
        if (basicMonsterValidCount == 0) return null;

        int randomIndex = Random.Range(0, basicMonsterValidCount);
        return basicMonsterNameArray[randomIndex];
    }

    /// <summary>
    /// 获取随机精英怪物名称
    /// </summary>
    public string GetRandomEliteMonsterName()
    {
        if (eliteMonsterValidCount == 0) return null;

        int randomIndex = Random.Range(0, eliteMonsterValidCount);
        return eliteMonsterNameArray[randomIndex];
    }

    /// <summary>
    /// 获取随机火属性基础怪物名称
    /// </summary>
    public string GetRandomFireBasicMonsterName()
    {
        if (fireBasicMonsters.Length == 0) return null;
        int randomIndex = Random.Range(0, fireBasicMonsters.Length);
        return fireBasicMonsters[randomIndex];
    }

    /// <summary>
    /// 获取随机水属性基础怪物名称
    /// </summary>
    public string GetRandomWaterBasicMonsterName()
    {
        if (waterBasicMonsters.Length == 0) return null;
        int randomIndex = Random.Range(0, waterBasicMonsters.Length);
        return waterBasicMonsters[randomIndex];
    }

    /// <summary>
    /// 获取随机土属性基础怪物名称
    /// </summary>
    public string GetRandomEarthBasicMonsterName()
    {
        if (earthBasicMonsters.Length == 0) return null;
        int randomIndex = Random.Range(0, earthBasicMonsters.Length);
        return earthBasicMonsters[randomIndex];
    }

    /// <summary>
    /// 获取火属性精英怪物名称（目前只有一个）
    /// </summary>
    public string GetFireEliteMonsterName()
    {
        if (fireEliteMonsters.Length == 0) return null;
        return fireEliteMonsters[0];
    }

    /// <summary>
    /// 获取水属性精英怪物名称（目前只有一个）
    /// </summary>
    public string GetWaterEliteMonsterName()
    {
        if (waterEliteMonsters.Length == 0) return null;
        return waterEliteMonsters[0];
    }

    /// <summary>
    /// 获取土属性精英怪物名称（目前只有一个）
    /// </summary>
    public string GetEarthEliteMonsterName()
    {
        if (earthEliteMonsters.Length == 0) return null;
        return earthEliteMonsters[0];
    }
}
