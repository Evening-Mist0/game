using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterMoveMgr : BaseMonoMgr<MonsterMoveMgr>
{
    [Header("配置")]
    public float delayBetweenColumns = 0.15f;

    public void StartBatchMove()
    {
        Dictionary<int, List<BaseMonsterCore>> columns = MonsterCreater.Instance.GetAliveColumns();
        // 新增调试：打印字典的列数 + 每列的怪物数量
        if (columns == null)
        {
            Debug.LogError("GetAliveColumns 返回null！");
            return;
        }
        Debug.Log($"获取到的列数：{columns.Count}，所有列的Key：{string.Join(",", columns.Keys)}");
        foreach (var kv in columns)
        {
            Debug.Log($"列 {kv.Key} 的存活怪物数量：{kv.Value.Count}");
        }

        if (columns.Count == 0)
        {
            Debug.Log("获取到怪物存活的数量为0，进入创建怪物阶段");
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_CreatMonster);
            return;
        }

       
        StartCoroutine(MoveByColumn(columns));
    }

    private IEnumerator MoveByColumn(Dictionary<int, List<BaseMonsterCore>> columns)
    {
        // 取到所有列并排序（1,2,3,4,5...）
        List<int> sorted = new List<int>(columns.Keys);
        sorted.Sort();

        // 遍历每一列（关键：必须按顺序全部执行）
        foreach (int col in sorted)
        {
            Debug.Log($"[移动列] 当前处理列 => {col}");

            // 安全判断
            if (!columns.ContainsKey(col) || columns[col].Count == 0)
            {
                Debug.Log($"列 {col} 无怪物，跳过");
                continue;
            }

            var monsters = columns[col];
            List<Coroutine> jobs = new List<Coroutine>();

            // 本列所有怪物一起移动
            foreach (var m in monsters)
            {
                if (m != null && m.IsAlive)
                {
                    jobs.Add(StartCoroutine(MoveSingle(m)));
                }
            }

            // ==============================
            // 【BUG修复】等待本列所有怪物移动完成
            // ==============================
            yield return null;
            if (jobs.Count > 0)
            {
                // 正确写法：等待所有协程结束
                foreach (var job in jobs)
                {
                    if (job != null) yield return job;
                }
            }

            // 列间隔
            yield return new WaitForSeconds(delayBetweenColumns);
        }

        Debug.Log("所有列全部移动完毕！");
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_CreatMonster);
    }

    IEnumerator MoveSingle(BaseMonsterCore monster)
    {
        if (monster == null || !monster.IsAlive) yield break;

        //执行竖直移动，等待完全完成
        yield return StartCoroutine(monster.MoveVertical(monster.baseMoveStepVertical));
        // 等待竖直移动的平滑动画收尾
        yield return new WaitWhile(() => monster.movement.IsMoving);

        //执行水平移动，等待完全完成
        yield return StartCoroutine(monster.MoveHorizontal(monster.baseMoveStepHorizontal));
        //等待水平移动的平滑动画收尾
        yield return new WaitWhile(() => monster.movement.IsMoving);
    }

    /// <summary>
    /// 横向相邻怪物位置交换
    /// </summary>
    /// <param name="m1"></param>
    /// <param name="m2"></param>
    public void HorizontallyAdjacentSwap(BaseMonsterCore m1,BaseMonsterCore m2)
    {
        if (m1 == null || m2 == null) { Debug.LogError("传入的怪物有空值"); return; }
        ;
            

        //获取第一个单元格
        Cell c1 = GridMgr.Instance.GetCell(m1.currentPos);
        if (c1 == null) { Debug.LogWarning("[位置交换],获取第一个对象的Cell失败");return; }


        //获取第二个单元格
        Cell c2 = GridMgr.Instance.GetCell(m2.currentPos);
        if (c2 == null) { Debug.LogWarning("[位置交换],获取第二个对象的Cell失败"); return; }

        if (c1.nowStateType != CellStateType.GhostOccupied)
            c1.UpdateOccupiedState(CellStateType.None, null);
        else
            c1.UpdateOccupiedState(CellStateType.GhostOccupied, null);

        if (c2.nowStateType != CellStateType.GhostOccupied)
            c2.UpdateOccupiedState(CellStateType.None, null);
        else
            c2.UpdateOccupiedState(CellStateType.GhostOccupied, null);

        m1.movement.MoveHorizontal(c2.logicalPos.x - c1.logicalPos.x);
        m2.movement.MoveHorizontal(c1.logicalPos.x - c2.logicalPos.x,-1,true);


    }
}