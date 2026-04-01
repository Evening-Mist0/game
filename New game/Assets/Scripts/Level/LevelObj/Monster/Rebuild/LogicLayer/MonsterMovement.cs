using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 怪物移动组件，负责怪物移动逻辑
/// </summary>
public class MonsterMovement : MonoBehaviour
{
    private BaseMonsterCore owner;
    private MonsterEffectControl effectControl;
    private GridMgr gridMgr;
    private MonsterCreater creater;

    // 移动配置，从怪物身上读取
    private int baseMoveStepHorizontal;
    private int baseMoveStepVertical;
    private int moveInterval;
    public int MoveInterval => moveInterval;
    private int currentRound;
    public int CurrentRound => currentRound;

    // 平滑移动相关
    private Coroutine smoothMoveCoroutine;
    public bool IsMoving { get; private set; }

    public void Init(BaseMonsterCore monster, MonsterEffectControl effect)
    {
        owner = monster;
        effectControl = effect;
        gridMgr = GridMgr.Instance;
        creater = MonsterCreater.Instance;

        // 同步怪物的移动配置
        baseMoveStepHorizontal = owner.baseMoveStepHorizontal;
        baseMoveStepVertical = owner.baseMoveStepVetical;
        moveInterval = owner.moveInterval;
        currentRound = 0;
    }

    /// <summary>
    /// 每回合更新，记录回合数
    /// </summary>
    public void OnRoundUpdate()
    {
        currentRound++;
        if (currentRound == moveInterval)
            currentRound = 0;

        //移动剩余回合数
        int number = moveInterval - currentRound-1;
        Debug.Log($"移动间隔{moveInterval}，当前累计回合{currentRound}");
        effectControl.UpdateIconCount(E_BuffIconType.Move, number);
    }

    /// <summary>
    /// 尝试移动一格（核心逻辑）
    /// </summary>
    private bool TryMove(GridPos direction, bool isCardEffect = false)
    {
        MonsterOnMove evt = new MonsterOnMove();

        // 校验移动方向合法性
        if (!((direction.x == -1 && direction.y == 0) || (direction.x == 1 && direction.y == 0) ||
              (direction.x == 0 && direction.y == 1) || (direction.x == 0 && direction.y == -1)))
        {
            Debug.LogError($"非法移动方向{direction}");
            return false;
        }

        if ((direction.x == -1 && direction.y == 0) || (direction.x == 1 && direction.y == 0))
        {
            if (!isCardEffect)
                evt.isHorizontalMove = true;
        }

        // 触发移动事件
        owner.TriggerOnMove(evt);

        // 到达左边界，如果不是垂直移动，则无法继续左移，直接攻击玩家
        if (owner.currentPos.x == 0 && direction.x != 1 && direction.y == 0)
        {
            if (!evt.isCancelAtk)
                owner.combat?.AttackTarget(GamePlayer.Instance);
            return false;
        }

        // 获取下一个格子位置
        GridPos nextPos = owner.currentPos + direction;
        if (!gridMgr.cellDic.ContainsKey(nextPos))
        {
            Debug.Log("移动越界，停止");
            return false;
        }

        Cell nextCell = gridMgr.cellDic[nextPos];

        // 移动间隔检查（不是卡牌效果才需要判断）
        if (currentRound % moveInterval != 0 && (isCardEffect == false))
        {
            // 未到移动回合，直接攻击前方目标
            owner.combat?.AttackTarget(nextCell.nowObj);
            return false;
        }

        // 被禁锢无法移动
        if (owner.buffHandler != null && owner.buffHandler.isImprison)
        {
            // 被禁锢时攻击前方单位
            owner.combat?.AttackTarget(nextCell.nowObj);
            if (owner.buffHandler.imprisonLastCount <= 0)
                owner.buffHandler.isImprison = false;
            return false;
        }

        // 判断前方格子状态
        // 前方是空地或幽灵占位，可以移动
        bool canMove = (nextCell.nowStateType == CellStateType.None) ||
                       ((nextCell.nowStateType == CellStateType.GhostOccupied && nextCell.nowObj == null));
        if (!canMove)
        {
            if (evt.isCoundDestoryDef && (nextCell.nowStateType == CellStateType.EntityOccupied))
            {
                //可以破坏防御塔，直接造成伤害
                BaseDefTower tower = nextCell.nowObj as BaseDefTower;
                tower.Hurt(owner);

            }
            else
            {
                // 无法直接破坏前方障碍物，停止移动并攻击
                Debug.Log($"前方格子被占用，类型：{nextCell.nowStateType}");
                if (nextCell.nowObj != null)
                {
                    Debug.Log($"怪物{this.gameObject.name}碰撞{nextCell.nowObj.name}，停止移动并攻击");
                    if ((!evt.isCancelAtk) && (!isCardEffect))//如果是被击退效果造成影响，不尝试攻击前方
                        owner.combat?.AttackTarget(nextCell.nowObj);
                    if(nextCell.nowObj != null)//进行攻击，如果前方没有障碍物了，继续移动
                    {
                        return false;
                    }
                    Debug.Log($"怪物{this.gameObject.name}销毁了建筑物，继续移动");
                }
            }
        }

        //// 横向移动时攻击目标(现在发现这个代码多余了，保险期间不删)
        //if (evt.isHorizontalMove)
        //    owner.combat?.AttackTarget(nextCell.nowObj);

        // 记录旧列号
        int oldColumn = owner.currentPos.x;

        // 释放旧格子
        gridMgr.cellDic[owner.currentPos].UpdateOccupiedState(CellStateType.None, null);
        // 占用新格子
        nextCell.UpdateOccupiedState(CellStateType.MonsterOccupied, owner);

        // 记录位置并更新坐标
        Vector3 oldWorldPos = owner.transform.position;
        owner.currentPos = nextPos;

        // 更新对象池列信息
        if (oldColumn != owner.currentPos.x)
        {
            creater.UpdateMonsterColumn(owner, oldColumn, owner.currentPos.x);
        }

        // 播放移动特效
        effectControl.PlayMoveAnimation();

        // 开始平滑移动
        StartSmoothMove(oldWorldPos, nextCell.myWorldPos);

        return true;
    }

    /// <summary>
    /// 启动平滑移动
    /// </summary>
    private void StartSmoothMove(Vector3 from, Vector3 to)
    {
        if (smoothMoveCoroutine != null)
            StopCoroutine(smoothMoveCoroutine);
        smoothMoveCoroutine = StartCoroutine(SmoothMoveCoroutine(from, to));
    }

    private IEnumerator SmoothMoveCoroutine(Vector3 from, Vector3 to)
    {
        IsMoving = true;
        float duration = 0.15f;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = t * t * (3f - 2f * t); // 平滑曲线
            owner.transform.position = Vector3.Lerp(from, to, t);
            yield return null;
        }
        owner.transform.position = to;
        IsMoving = false;
        smoothMoveCoroutine = null;
    }

    #region 外部移动接口（协程）

    /// <summary>
    /// 水平移动协程
    /// </summary>
    /// <param name="steps">移动步数</param>
    /// <param name="speed">每次移动的方向，仅支持一格，卡牌效果使用-1/1</param>
    public IEnumerator MoveHorizontal(int steps, int speed = -1, bool isCardEffect = false)
    {
        int tempSpeed = speed;
        Debug.Log("tempSpeed" + speed);
        GridPos dir = new GridPos(tempSpeed, 0);
        Debug.Log($"水平移动方向{dir.x}{dir.y}");

        for (int i = 0; i < steps; i++)
        {
            if (!TryMove(dir, isCardEffect))
                yield break;
            yield return new WaitWhile(() => IsMoving);
            yield return null;
        }

        MonsterOnMoveOver evt = new MonsterOnMoveOver();
        evt.currentPos = owner.currentPos;
        owner.TriggerOnMoveOver(evt);
    }

    public IEnumerator MoveVertical(int steps, int speed = 1, bool isCardEffect = false)
    {
        if (!isCardEffect && baseMoveStepVertical < 0)
        {
            Debug.Log("怪物无纵向移动能力");
            yield break;
        }

        if (isCardEffect)
        {
            // 强制纵向移动（卡牌效果）
            GridPos dir = new GridPos(0, speed);
            for (int i = 0; i < steps; i++)
            {
                if (!TryMove(dir, isCardEffect))
                    yield break;
                yield return new WaitWhile(() => IsMoving);
                yield return null;
            }
        }
        else
        {
            if (steps <= 0)
                yield break;

            // 随机上下移动
            int randomDir = Random.value > 0.5f ? 1 : -1;
            GridPos firstDir = new GridPos(0, randomDir * speed);
            Debug.Log($"怪物随机纵向移动方向为{firstDir.x}{firstDir.y}");
            bool firstSuccess = TryMove(firstDir);
            if (firstSuccess)
            {
                yield return new WaitWhile(() => IsMoving);
                yield return null;
                for (int i = 1; i < steps; i++)
                {
                    if (!TryMove(firstDir))
                        yield break;
                    yield return new WaitWhile(() => IsMoving);
                    yield return null;
                }
            }
            else
            {
                GridPos secondDir = new GridPos(0, -randomDir * speed);
                for (int i = 0; i < steps; i++)
                {
                    if (!TryMove(secondDir))
                        yield break;
                    yield return new WaitWhile(() => IsMoving);
                    yield return null;
                }
            }
        }

        MonsterOnMoveOver evt = new MonsterOnMoveOver();
        evt.currentPos = owner.currentPos;
        owner.TriggerOnMoveOver(evt);
    }

    /// <summary>
    /// 受到卡牌的击退效果
    /// </summary>
    /// <param name="card">哪张卡牌</param>
    /// <param name="coreCell">鼠标点击释放卡牌的位置</param>
    /// <param name="effectValue">造成击退效果的距离</param>
    public void GetRepel(BaseCard card, Cell coreCell,int effectValue)
    {
        if (card.cardRangeType == E_CardRangeType.Cross)
        {
            GridPos dir = owner.currentPos - coreCell.logicalPos;
            if (dir.x == 1 || dir.x == -1)
                owner.StartCoroutine(MoveHorizontal(effectValue, dir.x, true));
            else if (dir.y == 1 || dir.y == -1)
                owner.StartCoroutine(MoveVertical(effectValue, dir.y, true));
            else if (dir.x == 0 && dir.y == 0)
                owner.StartCoroutine(MoveHorizontal(effectValue, 1, true));
            else
                Debug.LogError("击退方向异常");
        }
        else
        {
            owner.StartCoroutine(MoveHorizontal(effectValue, 1, true));
        }
    }

    #endregion
}