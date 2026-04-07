using System.Collections;
using System.Collections.Generic;
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

    //装载普通怪物怪物资源路径的数组
    private string[] basicMonsterNameArray;
    //普通怪物数组容量（随机数范围）
    private int basicMonsterValidCount;

    //装载精英怪物怪物资源路径的数组
    private string[] eliteMonsterNameArray;
    //精英怪物数组容量（随机数范围）
    private int eliteMonsterValidCount;


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
            //Monster_None01_GodofAllElementalArts
        };
        tempList.RemoveAll(string.IsNullOrEmpty);

        basicMonsterNameArray = tempList.ToArray();
        basicMonsterValidCount = basicMonsterNameArray.Length;

        var tempList2 = new List<string>
        {
            Monster_Fire03_MoltenGuard,
            Monster_Water03_AbyssEel,
            Monster_Earth03_StoneGiant,
        };
        tempList.RemoveAll(string.IsNullOrEmpty);

        eliteMonsterNameArray = tempList.ToArray();
        eliteMonsterValidCount = eliteMonsterNameArray.Length;
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
}
