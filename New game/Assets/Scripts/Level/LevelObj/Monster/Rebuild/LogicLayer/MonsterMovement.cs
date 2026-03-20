//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

///// <summary>
///// 怪物移动组件，处理所有移动逻辑
///// </summary>
//public class MonsterMovement : MonoBehaviour
//{
//    private BaseMonsterCore owner;
//    private MonsterEffectControl effectControl;
//    private GridMgr gridMgr;
//    private MonsterCreater creater;

//    // 移动配置（从基类读取）
//    private int baseMoveStepHorizontal;
//    private int baseMoveStepVertical;
//    private int moveInterval;
//    private int currentRound;

//    // 平滑移动相关
//    private Coroutine smoothMoveCoroutine;
//    public bool IsMoving { get; private set; }

//    public void Init(BaseMonsterCore monster, MonsterEffectControl effect)
//    {
//        owner = monster;
//        effectControl = effect;
//        gridMgr = GridMgr.Instance;
//        creater = MonsterCreater.Instance;

//        // 从基类同步移动配置
//        baseMoveStepHorizontal = owner.baseMoveStepHorizontal;
//        baseMoveStepVertical = owner.baseMoveStepVertical;
//        moveInterval = owner.moveInterval;
//        currentRound = 0;
//    }

//    /// <summary>
//    /// 每回合调用，增加回合计数
//    /// </summary>
//    public void OnRoundUpdate()
//    {
//        currentRound++;
//    }

//    /// <summary>
//    /// 尝试移动一格（核心逻辑）
//    /// </summary>
//    private bool TryMove(GridPos direction)
//    {
//        // 校验方向合法性
//        if (!((direction.x == -1 && direction.y == 0) || (direction.x == 1 && direction.y == 0) ||
//              (direction.x == 0 && direction.y == 1) || (direction.x == 0 && direction.y == -1)))
//        {
//            Debug.LogError($"非法移动方向：{direction}");
//            return false;
//        }

//        // 被禁锢不能移动
//        if (owner.buffHandler != null && owner.buffHandler.IsImprisoned)
//            return false;

//        // 检查移动间隔
//        if (currentRound % moveInterval != 0)
//            return false;

//        // 如果到达最左边界且不是向右移动，则攻击玩家
//        if (owner.currentPos.x == 0 && direction.x != 1)
//        {
//            Debug.Log("怪物已到最左边，攻击玩家");
//            owner.combat?.AttackTarget(GamePlayer.Instance);
//            return false;
//        }

//        // 获取下一格
//        GridPos nextPos = owner.currentPos + direction;
//        if (!gridMgr.cellDic.ContainsKey(nextPos))
//        {
//            Debug.Log("移动越界，终止");
//            return false;
//        }

//        Cell nextCell = gridMgr.cellDic[nextPos];

//        // 判断前方格子状态
//        bool canMove = (nextCell.nowStateType == CellStateType.None) ||
//                       (nextCell.nowStateType == CellStateType.GhostOccupied);
//        if (!canMove)
//        {
//            Debug.Log($"前方格子被占用，类型：{nextCell.nowStateType}");
//            // 触发攻击阻挡物
//            if (nextCell.nowObj != null)
//                owner.combat?.AttackTarget(nextCell.nowObj);
//            return false;
//        }

//        // 记录旧列用于更新列字典
//        int oldColumn = owner.currentPos.x;

//        // 释放旧格子
//        gridMgr.cellDic[owner.currentPos].UpdateOccupiedState(CellStateType.None, null);
//        // 占用新格子
//        nextCell.UpdateOccupiedState(CellStateType.MonsterOccupied, owner);

//        // 更新位置
//        Vector3 oldWorldPos = owner.transform.position;
//        owner.currentPos = nextPos;

//        // 更新怪物创建器的列字典
//        if (oldColumn != owner.currentPos.x)
//        {
//            creater.UpdateMonsterColumn(owner, oldColumn, owner.currentPos.x);
//        }

//        // 播放移动动画
//        effectControl.PlayAnimation(E_AIStateType.Move);

//        // 开始平滑移动
//        StartSmoothMove(oldWorldPos, nextCell.myWorldPos);

//        // 触发移动特性
//        owner.TriggerOnMove(new MonsterOnMove());

//        return true;
//    }

//    /// <summary>
//    /// 启动平滑移动
//    /// </summary>
//    private void StartSmoothMove(Vector3 from, Vector3 to)
//    {
//        if (smoothMoveCoroutine != null)
//            StopCoroutine(smoothMoveCoroutine);
//        smoothMoveCoroutine = StartCoroutine(SmoothMoveCoroutine(from, to));
//    }

//    private IEnumerator SmoothMoveCoroutine(Vector3 from, Vector3 to)
//    {
//        IsMoving = true;
//        float duration = 0.15f;
//        float elapsed = 0f;
//        while (elapsed < duration)
//        {
//            elapsed += Time.deltaTime;
//            float t = elapsed / duration;
//            t = t * t * (3f - 2f * t); // 平滑插值
//            owner.transform.position = Vector3.Lerp(from, to, t);
//            yield return null;
//        }
//        owner.transform.position = to;
//        IsMoving = false;
//        smoothMoveCoroutine = null;
//    }

//    #region 公开移动接口（协程）
//    public IEnumerator MoveHorizontal(int steps, int speed = 1)
//    {
//        GridPos dir = new GridPos(speed, 0);
//        for (int i = 0; i < steps; i++)
//        {
//            if (!TryMove(dir))
//                yield break;
//            yield return new WaitWhile(() => IsMoving);
//            yield return null;
//        }
//    }

//    public IEnumerator MoveVertical(int steps, int speed = 1, bool isForced = false)
//    {
//        if (!isForced && baseMoveStepVertical < 0)
//        {
//            Debug.Log("怪物无垂直移动能力");
//            yield break;
//        }

//        if (isForced)
//        {
//            // 强制垂直移动（如击退）
//            GridPos dir = new GridPos(0, speed);
//            for (int i = 0; i < steps; i++)
//            {
//                if (!TryMove(dir))
//                    yield break;
//                yield return new WaitWhile(() => IsMoving);
//                yield return null;
//            }
//        }
//        else
//        {
//            // 随机方向尝试
//            int randomDir = Random.value > 0.5f ? 1 : -1;
//            GridPos firstDir = new GridPos(0, randomDir * speed);
//            bool firstSuccess = TryMove(firstDir);
//            if (firstSuccess)
//            {
//                yield return new WaitWhile(() => IsMoving);
//                yield return null;
//                for (int i = 1; i < steps; i++)
//                {
//                    if (!TryMove(firstDir))
//                        yield break;
//                    yield return new WaitWhile(() => IsMoving);
//                    yield return null;
//                }
//            }
//            else
//            {
//                GridPos secondDir = new GridPos(0, -randomDir * speed);
//                for (int i = 0; i < steps; i++)
//                {
//                    if (!TryMove(secondDir))
//                        yield break;
//                    yield return new WaitWhile(() => IsMoving);
//                    yield return null;
//                }
//            }
//        }
//    }

//    /// <summary>
//    /// 处理击退效果（由卡牌调用）
//    /// </summary>
//    public void GetRepel(BaseCard card, Cell coreCell)
//    {
//        if (card.CardRangeType == E_CardRangeType.Cross)
//        {
//            GridPos dir = owner.currentPos - coreCell.logicalPos;
//            if (dir.x == 1 || dir.x == -1)
//                owner.StartCoroutine(MoveHorizontal(card.baseEffectExtraValue, dir.x));
//            else if (dir.y == 1 || dir.y == -1)
//                owner.StartCoroutine(MoveVertical(card.baseEffectExtraValue, dir.y, true));
//            else if (dir.x == 0 && dir.y == 0)
//                owner.StartCoroutine(MoveHorizontal(card.baseEffectExtraValue, 1));
//            else
//                Debug.LogError("击退方向计算错误");
//        }
//        else
//        {
//            owner.StartCoroutine(MoveHorizontal(card.baseEffectExtraValue, 1));
//        }
//    }
//    #endregion
//}