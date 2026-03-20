using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物管理器，管理怪物的创建、销毁
/// </summary>
public class MonsterCreater : BaseMonoMgr<MonsterCreater>
{
    /// <summary>
    /// 每列最大怪物数量（可根据网格高度自动计算）
    /// </summary>
    private int maxMonstersPerColumn => GridMgr.Instance.gridHighCount;

    /// <summary>
    /// 键：列号  值 ：该列所有怪物
    /// </summary>
    public Dictionary<int, List<BaseMonster>> columnMonstersDic = new Dictionary<int, List<BaseMonster>>();


    /// <summary>
    /// 创建怪物（指定列，随机未占用Y轴格子）
    /// </summary>
    /// <param name="resName">怪物资源路径</param>
    /// <param name="birthColumn">出生列（X坐标)</param>
    /// <param name="count">要生成怪物的个数</param>
    public int CreateMonster(string resName, int count, int birthColumn)
    {
        // 参数验证
        if (count <= 0)
        {
            Debug.LogError($"生成怪物数量必须大于0，当前值：{count}");
            return 0;
        }

        // 检查列号是否有效
        if (birthColumn < 0 || birthColumn >= GridMgr.Instance.gridWideCount)
        {
            Debug.LogError($"无效的列号：{birthColumn}，有效范围：0-{GridMgr.Instance.gridWideCount - 1}");
            return 0;
        }

        //加载怪物预制体
        GameObject prefab = Resources.Load<GameObject>(resName);
        if (prefab == null)
        {
            Debug.LogError("找不到怪物：" + resName);
            return 0;
        }

        // 怪物生成成功的个数
        int successCount = 0;

        // 循环生成 count 个怪物
        for (int i = 0; i < count; i++)
        {
            // 每次生成前检查该列当前怪物数量是否已达到上限
            int currentMonsterCount = GetMonsterCountInColumn(birthColumn);
            if (currentMonsterCount >= maxMonstersPerColumn)
            {
                Debug.LogWarning($"列{birthColumn}的怪物数量已达到上限({maxMonstersPerColumn})，停止生成。已成功生成{successCount}个怪物");
                break;
            }

            //获取目标列未被占用的格子
            List<Cell> unoccupiedCells = GridMgr.Instance.GetUnoccupiedCellsInColumn(birthColumn);
            if (unoccupiedCells.Count == 0)
            {
                Debug.LogWarning($"列{birthColumn}已无未占用格子，停止生成。已成功生成{successCount}个怪物");
                break;
            }

            // 随机选一个未占用格子
            Cell targetCell = unoccupiedCells[Random.Range(0, unoccupiedCells.Count)];
            GridPos targetGridPos = targetCell.logicalPos;

            //在目标格子的世界坐标生成怪物
            GameObject monsterObj = Instantiate(prefab, targetCell.myWorldPos, Quaternion.identity);
            BaseMonster monster = monsterObj.GetComponent<BaseMonster>();
            if (monster == null)
            {
                Destroy(monsterObj);
                Debug.LogError($"怪物预制体{resName}缺少BaseMonster组件");
                continue; // 生成失败，跳过这个，继续生成下一个
            }

            successCount++;

            //标记格子为怪物占用状态
            targetCell.UpdateOccupiedState(CellStateType.MonsterOccupied, monster);

            //将怪物加入列管理
            AddMonsterToColumn(monster, birthColumn);

            //记录怪物所在格子
            monster.currentPos = targetGridPos;
        }

        Debug.Log($"[列{birthColumn}]怪物生成完成，目标数量：{count}，成功生成数量：{successCount}");
        return successCount;
    }

    /// <summary>
    /// 在最右列创建怪物
    /// </summary>
    /// <param name="resName">怪物资源路径</param>
    /// <param name="count">要生成怪物的个数</param>
    public int CreateMonster(string resName, int count)
    {
        int birthColumn = GridMgr.Instance.gridWideCount - 1;
        // 直接调用三个参数的版本，避免代码重复
        return CreateMonster(resName, count, birthColumn);
    }

    /// <summary>
    /// 获取指定列的怪物数量
    /// </summary>
    private int GetMonsterCountInColumn(int column)
    {
        if (!columnMonstersDic.ContainsKey(column))
            return 0;

        // 只计算存活的怪物
        int aliveCount = 0;
        foreach (var monster in columnMonstersDic[column])
        {
            if (monster != null && monster.IsAlive)
                aliveCount++;
        }
        return aliveCount;
    }

    /// <summary>
    /// 检查指定列是否已达到怪物数量上限
    /// </summary>
    public bool IsColumnFull(int column)
    {
        return GetMonsterCountInColumn(column) >= maxMonstersPerColumn;
    }

    /// <summary>
    /// 获取指定列剩余的可用格子数量
    /// </summary>
    public int GetRemainingSlotsInColumn(int column)
    {
        return maxMonstersPerColumn - GetMonsterCountInColumn(column);
    }

    /// <summary>
    /// 怪物死亡时释放格子占用（同时处理怪物死亡时移除MonsterCreater（该单例的怪物存在表）
    /// </summary>
    /// <param name="monster">要释放的怪物</param>
    public void ReleaseMonsterCell(BaseMonster monster)
    {
        if (GridMgr.Instance.cellDic.TryGetValue(monster.currentPos, out Cell cell))
        {
            cell.UpdateOccupiedState(CellStateType.None, null);
        }
        RemoveMonsterOnDeath(monster);
    }



    /// <summary>
    /// 添加到列
    /// </summary>
    private void AddMonsterToColumn(BaseMonster monster, int column)
    {
        if (!columnMonstersDic.ContainsKey(column))
            columnMonstersDic[column] = new List<BaseMonster>();

        Debug.Log($"[怪物列位置添加]将创建出来的怪物添加到列{column}");
        columnMonstersDic[column].Add(monster);
    }

    /// <summary>
    /// 怪物移动成功后调用：从旧列移到新列
    /// </summary>
    public void UpdateMonsterColumn(BaseMonster monster, int oldColumn, int newColumn)
    {
        // 检查新列是否已满
        if (IsColumnFull(newColumn))
        {
            Debug.LogWarning($"目标列{newColumn}已满，怪物无法移动");
            return;
        }

        //从旧列移除
        if (columnMonstersDic.ContainsKey(oldColumn))
        {
            columnMonstersDic[oldColumn].Remove(monster);
            // 空列删除
            if (columnMonstersDic[oldColumn].Count == 0)
                columnMonstersDic.Remove(oldColumn);
        }

        //添加到新列
        if (!columnMonstersDic.ContainsKey(newColumn))
            columnMonstersDic[newColumn] = new List<BaseMonster>();

        Debug.Log($"[怪物列位置更新]将怪物{monster.gameObject.name}添加到列{newColumn}");
        columnMonstersDic[newColumn].Add(monster);
    }

    /// <summary>
    /// 死亡移除
    /// </summary>
    private void RemoveMonsterOnDeath(BaseMonster monster)
    {
        foreach (var col in columnMonstersDic.Values)
        {
            if (col.Contains(monster))
            {
                col.Remove(monster);
                break;
            }
        }
    }



    /// <summary>
    /// 获取存活列（给移动管理器用）
    /// </summary>
    public Dictionary<int, List<BaseMonster>> GetAliveColumns()
    {
        Dictionary<int, List<BaseMonster>> result = new();
        foreach (var kvp in columnMonstersDic)
        {
            var alive = kvp.Value.FindAll(m => m != null && m.IsAlive);
            if (alive.Count > 0) result[kvp.Key] = alive;
        }
        return result;
    }

    /// <summary>
    /// 获取所有存活的怪物列表(给状态流，怪物状态更新用)
    /// </summary>
    /// <returns>包含所有存活怪物的列表</returns>
    public List<BaseMonster> GetAllAliveMonsters()
    {
        List<BaseMonster> allMonsters = new List<BaseMonster>();

        foreach (var monsterList in columnMonstersDic.Values)
        {
            // 只添加存活且不为空的怪物
            foreach (var monster in monsterList)
            {
                if (monster != null && monster.IsAlive)
                {
                    allMonsters.Add(monster);
                }
            }
        }

        return allMonsters;
    }
}