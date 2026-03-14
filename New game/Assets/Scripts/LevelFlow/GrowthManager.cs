using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthManager : BaseMgr<GrowthManager>
{
    private GrowthManager()
    {

    }

    public int currentExp = 0;
    public int currentLevel = 0;
    public int maxLevel = 5;

    // 增加经验
    public void AddExp(int amount)
    {
        if (currentLevel >= maxLevel) return;
        currentExp += amount;

        // 检查是否升级
        while (currentExp >= 2 && currentLevel < maxLevel)
        {
            currentExp -= 2;
            currentLevel++;
            TriggerLevelUp();
        }
    }

    // 升级弹窗
    private void TriggerLevelUp()
    {
        // 随机3个不重复的升级选项

    }
}
