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
        yield return StartCoroutine(monster.MoveVertical(monster.baseMoveStepVetical));
        // 等待竖直移动的平滑动画收尾
        yield return new WaitWhile(() => monster.movement.IsMoving);

        //执行水平移动，等待完全完成
        yield return StartCoroutine(monster.MoveHorizontal(monster.baseMoveStepHorizontal));
        //等待水平移动的平滑动画收尾
        yield return new WaitWhile(() => monster.movement.IsMoving);
    }
}