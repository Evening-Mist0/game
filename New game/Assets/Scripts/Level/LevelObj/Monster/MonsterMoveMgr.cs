

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MonsterMoveMgr : BaseMonoMgr<MonsterMoveMgr>
{
    [Header("配置")]
    public float delayBetweenColumns = 0.15f;

    // 协程引用，用于生命周期管理
    private Coroutine currentMoveCoroutine = null;

    // 超时保护协程的引用
    private Coroutine timeoutCoroutine = null;

    private void OnDestroy()
    {
        // 组件销毁时，若移动协程未完成，强制执行清理
        if (currentMoveCoroutine != null)
        {
            Debug.LogWarning("[MonsterMoveMgr] 组件销毁时检测到未完成的移动协程，强制执行清理");
            ForceCompleteMoveCleanup();
        }
    }

    /// <summary>
    /// 强制完成移动后的清理逻辑（启用按钮 + 切换状态）
    /// </summary>
    private void ForceCompleteMoveCleanup()
    {
        // 启用结束回合按钮
        CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();
        if (panel != null)
            panel.EnableOverMyTurnButton();
        else
            Debug.LogWarning("[MonsterMoveMgr] CardPlayingPanel 未找到，无法启用按钮");

    
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_CreatMonster);
        Debug.Log("[MonsterMoveMgr] 已调用 ChangeState(MonsterTurn_CreatMonster)");
     

        // 清理超时协程
        if (timeoutCoroutine != null)
        {
            StopCoroutine(timeoutCoroutine);
            timeoutCoroutine = null;
        }

        currentMoveCoroutine = null;
    }

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
            CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();
            if (panel != null)
                panel.EnableOverMyTurnButton();
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_CreatMonster);
            return;
        }

        // 避免重叠：停止已有协程并清理
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            ForceCompleteMoveCleanup();
        }

        currentMoveCoroutine = StartCoroutine(MoveByColumn(columns));
    }

    private IEnumerator MoveByColumn(Dictionary<int, List<BaseMonsterCore>> columns)
    {
        bool completed = false;

        // 启动超时保护：如果移动过程超过8秒还未完成，自动强制清理（防止因未知bug无限等待）
        if (timeoutCoroutine != null) StopCoroutine(timeoutCoroutine);
        timeoutCoroutine = StartCoroutine(MoveTimeoutWatcher());

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
            // 停止超时监测
            if (timeoutCoroutine != null)
            {
                StopCoroutine(timeoutCoroutine);
                timeoutCoroutine = null;
            }

            if (!completed)
            {
                Debug.LogError("怪物移动过程异常中断，强制切换到创建怪物状态");
            }

            // 清理操作：启用按钮、切换状态
            CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();
            if (panel != null)
                panel.EnableOverMyTurnButton();
            else
                Debug.LogWarning("[MonsterMoveMgr] 清理时未找到 CardPlayingPanel");

            if (LevelStepMgr.Instance?.machine != null)
            {
                // 记录切换前的状态（如果有CurrentState属性则尝试获取）
                Debug.Log($"[MonsterMoveMgr] 移动阶段结束，切换状态到 MonsterTurn_CreatMonster");
                LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_CreatMonster);
            }
            else
            {
                Debug.LogError("[MonsterMoveMgr] LevelStepMgr 或 machine 为空，无法切换状态");
            }

            currentMoveCoroutine = null;
        }
    }

    /// <summary>
    /// 超时保护：30秒内如果移动还未完成，强制执行清理并报错
    /// </summary>
    private IEnumerator MoveTimeoutWatcher()
    {
        float timeout = 8f; // 可根据实际情况调整
        yield return new WaitForSeconds(timeout);
        Debug.LogError($"[MonsterMoveMgr] 怪物移动超时（{timeout}秒），强制结束移动并清理状态");
        // 注意：此时协程 MoveByColumn 可能仍在运行，需要强制停止它
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
        }
        ForceCompleteMoveCleanup();
    }

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