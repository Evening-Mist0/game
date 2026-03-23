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

    //装载所有怪物资源路径的数组
    private string[] monsterNameArray;
    //数组容量（随机数范围）
    private int validCount;

    public void Initialize() // 在JSON加载完成后调用
    {
        var tempList = new List<string>
        {
            Monster_Fire01_FlameSprite,
            Monster_Fire02_CombustionWorm,
            Monster_Fire03_MoltenGuard,
            Monster_Water01_WaterWisp,
            Monster_Water02_TideSoldier,
            Monster_Water03_AbyssEel,
            Monster_Earth01_StoneSprite,
            Monster_Earth02_ShieldGuard,
            Monster_Earth03_StoneGiant,
            Monster_None01_GodofAllElementalArts
        };
        tempList.RemoveAll(string.IsNullOrEmpty);

        monsterNameArray = tempList.ToArray();
        validCount = monsterNameArray.Length;
    }

    /// <summary>
    /// 获取随机怪物名称
    /// </summary>
    public string GetRandomMonsterName()
    {
        if (validCount == 0) return null;

        int randomIndex = Random.Range(0, validCount);
        return monsterNameArray[randomIndex];
    }
}
