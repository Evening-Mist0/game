
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
            //取消结束回合按钮取消禁用
            CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();
            if (panel != null)
                panel.EnableOverMyTurnButton();
            return;
        }

        StartCoroutine(MoveByColumn(columns));
    }

    private IEnumerator MoveByColumn(Dictionary<int, List<BaseMonsterCore>> columns)
    {
        // 标记是否正常完成，用于在 finally 中判断是否需要打印错误日志
        bool completed = false;

        // 【关键修改】使用 try-finally（不允许 catch，但 finally 保证一定会执行）
        try
        {
            List<int> sorted = new List<int>(columns.Keys);
            sorted.Sort();

            foreach (int col in sorted)
            {
                Debug.Log($"[移动列] 当前处理列 => {col}");

                if (!columns.ContainsKey(col) || columns[col].Count == 0)
                {
                    Debug.Log($"列 {col} 无怪物，跳过");
                    continue;
                }

                var monsters = columns[col];
                List<Coroutine> jobs = new List<Coroutine>();

                foreach (var m in monsters)
                {
                    if (m != null && m.IsAlive)
                    {
                        jobs.Add(StartCoroutine(MoveSingle(m)));
                    }
                }

                if (jobs.Count > 0)
                {
                    foreach (var job in jobs)
                    {
                        if (job != null) yield return job;
                    }
                }

                yield return new WaitForSeconds(delayBetweenColumns);
            }

            Debug.Log("所有列全部移动完毕！");
            completed = true;
        }
        finally
        {
            // 【保证状态切换】无论协程如何结束（正常、异常、被中断），都会执行到这里
            if (!completed)
            {
                Debug.LogError("怪物移动过程异常中断，强制切换到创建怪物状态");
            }
            //取消结束回合按钮取消禁用
            CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();
            if (panel != null)
                panel.EnableOverMyTurnButton();

            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_CreatMonster);
        }
    }

    // 【注意】MoveSingle 保持原样，无需 try-catch（因为异常会冒泡到 MoveByColumn 的 finally）
    IEnumerator MoveSingle(BaseMonsterCore monster)
    {
        if (monster == null || !monster.IsAlive) yield break;

        // 竖直移动
        yield return StartCoroutine(monster.MoveVertical(monster.baseMoveStepVertical));
        yield return new WaitWhile(() => monster.movement.IsMoving);

        // 水平移动
        yield return StartCoroutine(monster.MoveHorizontal(monster.baseMoveStepHorizontal));
        yield return new WaitWhile(() => monster.movement.IsMoving);
    }

    public void HorizontallyAdjacentSwap(BaseMonsterCore m1, BaseMonsterCore m2)
    {
        if (m1 == null || m2 == null) { Debug.LogError("传入的怪物有空值"); return; }

        Cell c1 = GridMgr.Instance.GetCell(m1.currentPos);
        if (c1 == null) { Debug.LogWarning("[位置交换],获取第一个对象的Cell失败"); return; }

        Cell c2 = GridMgr.Instance.GetCell(m2.currentPos);
        if (c2 == null) { Debug.LogWarning("[位置交换],获取第二个对象的Cell失败"); return; }

        Vector3 m1Pos = c1.myWorldPos;
        m1.transform.position = m2.transform.position;
        m2.transform.position = m1Pos;

        MonsterCreater.Instance.UpdateMonsterColumn(m1, m1.currentPos.x, m2.currentPos.x);
        MonsterCreater.Instance.UpdateMonsterColumn(m2, m2.currentPos.x, m1.currentPos.x);

        GridPos m1GridPos = m1.currentPos;
        m1.currentPos = m2.currentPos;
        m2.currentPos = m1GridPos;

        if (c1.nowStateType == CellStateType.GhostOccupied)
            c1.nowObj = m2;
        else
            c1.UpdateOccupiedState(CellStateType.MonsterOccupied, m2);

        if (c2.nowStateType == CellStateType.GhostOccupied)
            c2.nowObj = m1;
        else
            c2.UpdateOccupiedState(CellStateType.MonsterOccupied, m1);
    }
}