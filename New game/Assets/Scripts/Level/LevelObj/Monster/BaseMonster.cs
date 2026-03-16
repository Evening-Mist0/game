using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 怪物元素属性
/// </summary>
public enum MonsterElement
{
    /// <summary>
    /// 无属性
    /// </summary>
    None,
    /// <summary>
    /// 火系怪
    /// </summary>
    Fire,
    /// <summary>
    /// 水系怪
    /// </summary>
    Water,
    /// <summary>
    /// 土系怪
    /// </summary>
    Earth
}

/// <summary>
/// 怪物身份类型
/// </summary>
public enum MonsterIdentity
{
    /// <summary>
    /// 基础怪
    /// </summary>
    Basic,
    /// <summary>
    /// 精英怪
    /// </summary>
    Elite,
    /// <summary>
    /// Boss怪
    /// </summary>
    Boss
}

/// <summary>
/// 怪物特性触发类型
/// </summary>
public enum E_MonsterTriggerType
{
    /// <summary>
    /// 死亡触发
    /// </summary>
    Death,
    /// <summary>
    /// 受击触发
    /// </summary>
    Hurt,
    /// <summary>
    /// 移动触发
    /// </summary>
    Move,
    /// <summary>
    /// 进入战场触发
    /// </summary>
    Enter,
    /// <summary>
    /// 每回合触发
    /// </summary>
    Round,
    /// <summary>
    /// 血量低于阈值触发
    /// </summary>
    HpLow
}

[RequireComponent(typeof(StateMachine)), RequireComponent(typeof(Animator)), RequireComponent(typeof(MonsterEffectControl))]
/// <summary>
/// 怪物抽象基类
/// 所有怪物子类继承此基类
/// 必须继承I_AIAction接口
/// </summary>
public abstract class BaseMonster : BaseLevelObject, I_AIAction
{
    #region 通用属性
    [Header("怪物基础配置")]
    [Tooltip("怪物唯一ID")]
    public string monsterID;
    [Tooltip("怪物名称")]
    public string monsterName;
    [Tooltip("怪物最大血量")]
    public int maxHp;
    [Tooltip("怪物当前血量")]
    public int currentHp;
    [Tooltip("怪物攻击力")]
    public int attack;
    [Tooltip("怪物元素属性")]
    public MonsterElement element;
    [Tooltip("怪物身份类型")]
    public MonsterIdentity identity;

    [Header("移动基础配置")]
    [Tooltip("基础左移格数/回合")]
    public int baseMoveStepHorizontal = 1;
    [Tooltip("基础上/下 移格数/回合（没有上下移动的功能就填-1!）")]
    public int baseMoveStepVetical = 1;
    [Tooltip("移动间隔回合（1=每回合移，2=每2回合移）")]
    public int moveInterval = 1;
    /// <summary>
    /// 当前怪物所在位置
    /// </summary>
    public GridPos currentPos;
    #endregion

    #region 状态属性
    /// <summary>
    /// 是否存活
    /// </summary>
    public bool IsAlive => currentHp > 0;
    /// <summary>
    /// 当前回合数（用于判断移动间隔）
    /// </summary>
    protected int currentRound;

    /// <summary>
    /// 燃烧持续效果回合
    /// </summary>
    private int burnLastCount;

    /// <summary>
    /// 禁锢持续回合
    /// </summary>
    private int imprisonLastCount;

    private List<E_CardSkill> nowEffectedSkills = new List<E_CardSkill>();

    #endregion

    #region 关联组件
    StateMachine machine;
    Animator animator;
    MonsterEffectControl effectControl;
    #endregion

    //平滑移动协程
    private Coroutine smoothMoveCoroutine;
    /// <summary>是否正在平滑移动中</summary>
    public bool IsSmoothingMoving { get; private set; }
    //是否能受到效果攻击
    [HideInInspector]
    public bool isAllowedEffected = true;

    protected virtual void Awake()
    {
        //初始化数值
        InitBasicValue();
        // 初始化战场格子坐标（玩家看到4*6，实际上格子是4*7）
        InitMyPos();
        //初始化相关控件
        InitComponents();
    }

    protected virtual void Start()
    {
        TriggerSpecial(E_MonsterTriggerType.Enter);
    }


    #region 核心通用方法
    /// <summary>
    /// 初始化格子坐标（子类可重写，用于MonsterCreater）
    /// </summary>
    public virtual void InitMyPos()
    {
        currentPos = new GridPos(GridMgr.Instance.gridWideCount - 1, GridMgr.Instance.gridHighCount - 1); // 默认初始在格子地图最右上角
    }

    /// <summary>
    /// 初始化怪物基础数值
    /// </summary>
    private void InitBasicValue()
    {
        // 初始化血量
        currentHp = maxHp;
        //初始化移动回合
        currentRound = 0;
        //是否能被附着卡牌特殊效果
        isAllowedEffected = true;
    }

    /// <summary>
    /// 初始化相关控件
    /// </summary>
    private void InitComponents()
    {
        machine = GetComponent<StateMachine>();
        animator = GetComponent<Animator>();
        effectControl = GetComponent<MonsterEffectControl>();
    }

    /// <summary>
    /// 更新回合状态(血量/卡牌效果结算等)（由LevelStepMgr的MonsterMoveState状态调用）
    /// </summary>
    /// <param name="type"></param>
    public virtual void OnRoundUpdate()
    {
        //在MonsterMoveState状态下才能进行位置更新
        if (LevelStepMgr.Instance.ComfirNowStateType(E_LevelState.MonsterTurn_Move))
        {
            Debug.Log("处于怪物移动状态，更新怪物移动");
            //更新行动回合
            currentRound++;
            if (IsAlive)
            {
                // 触发回合更新特性
                TriggerSpecial(E_MonsterTriggerType.Round);            
            }
        }
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="atk">原始伤害值</param>
    public virtual void TakeDamage(int atk)
    {
        if (!IsAlive) return;

        // 触发受击特性（如反弹、减伤等）
        int finalDamage = TriggerSpecial(E_MonsterTriggerType.Hurt, atk);
        finalDamage = Mathf.Max(finalDamage, 1);
        // 扣血
        currentHp -= finalDamage;
        Debug.Log($"{monsterName}受到{finalDamage}点伤害，当前血量：{currentHp}");

        // 死亡判断
        if (currentHp <= 0)
        {
            Die();
            return;
        }

        TriggerSpecial(E_MonsterTriggerType.HpLow);
    }


    /// <summary>
    /// 怪物移动(横向) → 改成协程 IEnumerator
    /// </summary>
    /// <param name="distance">横向移动总距离</param>
    /// <param name="speed">每次移动的速度</param>
    public IEnumerator MoveHorizontal(int distance, int speed = -1)
    {
        GridPos moveSpeed = new GridPos(speed, 0);

        for (int i = 0; i < distance; i++)
        {
            if (!Move(moveSpeed))
            {
                Debug.Log($"[水平移动]以距离{moveSpeed.x}{moveSpeed.y}无法移动到下一格,终止移动");
                yield break; // 协程退出
            }

            //等待当前平滑移动完成后，再执行下一步
            yield return new WaitWhile(() => IsSmoothingMoving);
            // 每步移动后额外等待一帧，确保逻辑完全收尾
            yield return null;
        }

        Debug.Log("[水平移动]水平移动执行完成,未碰到边界");
    }

    /// <summary>
    /// 怪物移动(纵向) → 已经是协程，我帮你补全 yield 保证能等待
    /// </summary>
    /// <param name="distance">纵向移动总距离</param>
    /// <param name="speed">每次移动的速度</param>
    /// <param name="isCardEffect">是否是被卡牌效果击退影响所进行的垂直移动</param>
    /// <param name="cardEffectDir">移动的方向（方向默认是1(向上))</param>
    /// <returns></returns>
    public IEnumerator MoveVertical(int distance, int speed = 1,bool isCardEffect = false)
    {
        if(isCardEffect)//如果是被卡牌效果影响移动，就不会尝试其他路径
        {
            GridPos moveSpeed = new GridPos(0, speed);
            Debug.Log($"检测到怪物是被卡牌效果而推动，获取推动的距离{moveSpeed.x}{moveSpeed.y}");


            for (int i = 0; i < distance; i++)
            {
                if (!Move(moveSpeed))
                {
                    Debug.Log("[垂直移动]因为约束无法移动到下一格,终止移动");
                    yield break; // 协程退出
                }
                Debug.Log("[垂直移动]垂直移动完美完成");
                //等待当前平滑移动完成后，再执行下一步
                yield return new WaitWhile(() => IsSmoothingMoving);
                // 每步移动后额外等待一帧，确保逻辑完全收尾
                yield return null;
            }
        }
        else
        {
            //baseMoveStepVetical < 0 表示无竖直移动能力
            if (baseMoveStepVetical < 0)
            {
                Debug.Log("检测到不合法的竖直移动步数小于0,终止竖直移动");
                yield break;
            }

            //随机初始方向
            int randomDir = UnityEngine.Random.value > 0.5f ? 1 : -1;

            //第一步尝试，记录是否成功
            GridPos firstDir = new GridPos(0, randomDir * speed);
            bool firstSuccess = Move(firstDir);

            // 等待第一步平滑移动完成
            if (firstSuccess)
            {
                yield return new WaitWhile(() => IsSmoothingMoving);
                yield return null;

                // 继续向同一方向移动剩余步数
                for (int i = 1; i < distance; i++)
                {
                    if (!Move(firstDir))
                        yield break;

                    yield return new WaitWhile(() => IsSmoothingMoving);
                    yield return null;
                }
                yield break;
            }

            //第一步失败，尝试反向
            int reverseDir = -randomDir;
            GridPos secondDir = new GridPos(0, reverseDir * speed);

            // 反向移动所有步数
            for (int i = 0; i < distance; i++)
            {
                if (!Move(secondDir))
                    yield break;

                yield return new WaitWhile(() => IsSmoothingMoving);
                yield return null;
            }
        }
        
    }

    #region I_AIAction接口

    /// <summary>
    /// 接口的方法不用在外部,由于架构迫使为public，具体移动由MoveHorizontal，MoveVertical实现
    /// </summary>
    /// <param name="speed">移动的距离(只能是1，0或者0，1或者0 -1)！</param>
    /// <returns>判定移动成功,移动后返回true/移动失败,不移动返回false</returns>
    public bool Move(GridPos speed)
    {
        //记录旧的列，用于怪物创建管理器的列字典更新
        int oldColumn = currentPos.x;
        //校验值是否为规定值
        if (!((speed.x == -1 && speed.y == 0) || (speed.x == 0 && speed.y == 1)|| (speed.x == 0 && speed.y == -1)|| (speed.x == 1 && speed.y == 0)))
        {
            Debug.LogError($"Move传入非法参数：{speed}{speed.x}{speed.y}，只能传入 (1,0) 或 (0,1)或(0,-1)或(1,0)！");
            return false;
        }

        // 判断是否满足移动条件
        if (!(currentRound % moveInterval == 0))
            return false;

        //如果到达第0列,停止移动并向玩家发起攻击
        if ((currentPos.x == 0)&&speed.x != 1)//如果不是造成的击退效果（speed.x == 1）这次移动就是进行攻击，不再移动
        {
            Debug.Log("怪物已经移动到最左边,不能再移动,直接对玩家发起攻击");
            PlayerTest.Instance.Hurt(attack);
            //被边界挡住传入null
            BeStopped(null);
            return false;
        }


        // 合法才继续执行
        Cell nextCell = null;
        //判定怪物当前位置是否存在单元格管理器的字典中
        if (!GridMgr.Instance.cellDic.ContainsKey(currentPos))
            return false;
        //计算下一格子
        GridPos nextCellLogicalPos = GridMgr.Instance.cellDic[currentPos].logicalPos + speed;

        if (!GridMgr.Instance.cellDic.ContainsKey(nextCellLogicalPos))
        {
            Debug.Log($"纵向移动，传入的坐标越界，坐标为{nextCellLogicalPos.x}{nextCellLogicalPos.y}");
            //被边界挡住传入null
            BeStopped(null);
            return false;
        }

        nextCell = GridMgr.Instance.cellDic[nextCellLogicalPos];



        //判定前方格子是否被占据，被占据无法移动
        if (!(nextCell.nowStateType == CellStateType.None))
        {
            Debug.Log($"[怪物移动]怪物要移动到一个格子的类型为{nextCell.nowStateType}无法移动");
            BeStopped(nextCell.nowObj);
            return false;
        }

        //终于能移动到新的格子了what can i say
        // 旧格子释放
        GridMgr.Instance.cellDic[currentPos].UpdateOccupiedState(CellStateType.None, null);
        // 新格子占用
        nextCell.UpdateOccupiedState(CellStateType.MonsterOccupied, this);
        // 记录旧位置（用于平滑移动起点）
        Vector3 oldWorldPos = transform.position;
        // 更新逻辑坐标
        currentPos = nextCell.logicalPos;
        // 更新列字典
        int newColumn = currentPos.x;
        if (oldColumn != newColumn)
        {
            MonsterCreater.Instance.UpdateMonsterColumn(this, oldColumn, newColumn);
        }

        // 播放移动动画
        effectControl.PlayAnimation(E_AIStateType.Move);

        // 停止上一次移动
        if (smoothMoveCoroutine != null)
            StopCoroutine(smoothMoveCoroutine);

        //开启协程（仅是视觉效果没有逻辑）
        smoothMoveCoroutine = StartCoroutine(SmoothMoveVisualOnly(oldWorldPos, nextCell.myWorldPos));
        return true;

    }

    /// <summary>
    /// 平滑移动协程
    /// </summary>
    private IEnumerator SmoothMoveVisualOnly(Vector3 start, Vector3 target)
    {
        IsSmoothingMoving = true; // 标记开始移动
        float duration = 0.15f;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = t * t * (3 - 2 * t); // 平滑插值

            transform.position = Vector3.Lerp(start, target, t);
            yield return null;
        }

        // 修正位置
        transform.position = target;
        smoothMoveCoroutine = null;
        IsSmoothingMoving = false; // 标记移动结束
    }


    /// <summary>
    /// 检查前方格子被占据的对象,多用于移动过程中被强制停止的情况
    /// </summary>
    /// <param name="cell">是被哪个格子阻挡移动</param>
    public void BeStopped(BaseLevelObject obj)
    {
        OnStopped(obj);
    }

    /// <summary>
    /// 处于当前格子该做的事
    /// </summary>
    /// <param name="obj">被哪个物体阻挡</param>
    private void OnStopped(BaseLevelObject obj)
    {
        if (obj == null)
            return;

        //更新当前格子位置(注意不是参数的格子,而是当前现在的格子)
        GridMgr.Instance.cellDic[currentPos].UpdateOccupiedState(CellStateType.MonsterOccupied, this);
        Atk(obj);
    }


    /// <summary>
    /// 攻击具体目标
    /// </summary>
    /// <param name="targetObj">攻击目标</param>

    public void Atk(BaseLevelObject targetObj)
    {
        if (targetObj == null) return;

        switch (targetObj.levelObjectType)
        {
            case E_LevelObjectType.Player:
                PlayerTest.Instance.Hurt(attack);
                Debug.Log($"{monsterName}攻击玩家，造成{attack}点伤害");
                break;

            case E_LevelObjectType.DefTower:
                var defTower = targetObj as BaseDefTower;
                defTower.Hurt(attack);
                Debug.Log($"{monsterName}攻击防御塔，造成{attack}点伤害");
                break;

            case E_LevelObjectType.Monster:
                // 怪物之间攻击逻辑可在此扩展
                Debug.Log($"{monsterName}攻击其他怪物，暂未实现逻辑");
                break;
        }
    }

    /// <summary>
    /// 怪物死亡后最终处理（留空，由关卡管理器实现：如销毁、计分、更新单元格状态）
    /// </summary>
    public void Die()
    {
        Debug.Log($"{monsterName}已死亡");
        // 触发死亡特性
        TriggerSpecial(E_MonsterTriggerType.Death);
        
        // 当死亡时要触发的逻辑(移除Level当前怪物表的该怪物等)
        OnMonsterDie();
        
    }
    #endregion
    #endregion

    #region 触发特性条件相关
    /// <summary>
    /// 根据怪物状态触发怪物特性（如伤害减半，技能免疫，添加全场Buff等）
    /// BaseMonster父类中有对应的虚函数，要触发这个怪物的特性就重写虚函数
    /// </summary>
    /// <param name="triggerType">触发怪物特性的条件枚举</param>
    /// <param name="value">触发怪物特性传入的值(目前只有受伤传入攻击伤害)</param>
    private int TriggerSpecial(E_MonsterTriggerType triggerType, int value = 0)
    {
        switch (triggerType)
        {
            case E_MonsterTriggerType.Death:
                return TriggerSpecialOnDeath();
            case E_MonsterTriggerType.Hurt:
                return TriggerSpecialOnHurt(value);
            case E_MonsterTriggerType.Move:
                return TriggerSpecialOnMove();
            case E_MonsterTriggerType.Enter:
                return TriggerSpecialOnEnter();
            case E_MonsterTriggerType.Round:
                return TriggerSpecialOnRound();
            case E_MonsterTriggerType.HpLow:
                return TriggerSpecialOnHpLow();
        }
        Debug.LogError("没有找到怪物特性触发状态相关枚举");
        return 0;
    }


    #endregion




    #region 预留扩展接口

    #region 具体特性效果
    protected virtual int TriggerSpecialOnDeath()
    {
        return -1;
    }

    /// <summary>
    /// 返回受到的伤害值，注意！受到伤害后最低值为1
    /// </summary>
    /// <param name="atk">原本受到的伤害</param>
    /// <returns></returns>
    protected virtual int TriggerSpecialOnHurt(int atk)
    {
        
        return atk;
    }
    protected virtual int TriggerSpecialOnMove()
    {
        //移动时候的特殊效果（比如有移速加成等（可能没有））
        //注意：垂直移动的Move方法已写不要再次实现
        return -1;
    }
    protected virtual int TriggerSpecialOnEnter()
    {
        //通过LevelStepMgr得到当前场地存在的怪物表，然后进行属性加成
        return -1;
    }

    protected virtual int TriggerSpecialOnRound()
    {
        //受到的卡牌技能效果伤害结算(灼烧、禁锢等)
        return -1;
    }

    protected virtual int TriggerSpecialOnHpLow()
    {
        //得到玩家单例进行效果实现(目前就一个Boss)
        return -1;
    }
    #endregion
    /// <summary>
    /// 血量低于阈值触发（如大地BOSS的元素湮灭）
    /// 子类可重写判断逻辑，调用OnTriggerSpecial(TriggerType.HpLow)
    /// </summary>
    /// <param name="hpThreshold">血量阈值</param>
    public virtual void CheckHpLow(int hpThreshold)
    {
        if (currentHp < hpThreshold)
        {
            TriggerSpecial(E_MonsterTriggerType.HpLow);
        }
    }

    /// <summary>
    /// 怪物死亡时会发生的事情,如销毁物体、战场移除（由LevelMgr的怪物移动状态处理，此处留接口）
    /// </summary>
    public virtual void OnMonsterDie()
    {
        //怪物死亡时触发的特殊效果
        TriggerSpecial(E_MonsterTriggerType.Death);
        //将怪物从场景表中移除
        MonsterCreater.Instance.ReleaseMonsterCell(this);

        //更新格子状态
        if (GridMgr.Instance.cellDic.ContainsKey(currentPos))
        {
            GridMgr.Instance.cellDic[currentPos].UpdateOccupiedState(CellStateType.None,null);
        }

        //销毁
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 检测同列/全场怪物（需求中潮行兵等需此逻辑，留虚方法由子类实现）
    /// 重写时候不用调用父函数
    /// </summary>
    /// <param name="isSameColumn">是否仅检测同列</param>
    /// <param name="targetElement">目标元素属性</param>
    /// <returns>符合条件的怪物列表</returns>
    public virtual List<BaseMonster> CheckOtherMonsters(bool isSameColumn, MonsterElement targetElement)
    {
        return null;
    }


    #region 卡牌效果
    /// <summary>
    /// 该怪物触发燃烧效果
    /// </summary>
    public void GetBurn(int lastcount)
    {
        burnLastCount = lastcount;
        AddEffect(E_CardSkill.Burn);

    }

    /// <summary>
    /// 该怪物触发禁锢效果
    /// </summary>
    /// <param name="lastCount">持续回合</param>
    public void GetImprison(int lastCount)
    {
        imprisonLastCount = lastCount;
        AddEffect(E_CardSkill.Imprison);
    }

   /// <summary>
   /// 受到反伤伤害
   /// </summary>
   /// <param name="hurt">受到的伤害</param>
    public void GetReflect(int hurt)
    {
        
    }

    /// <summary>
    /// 受到真伤
    /// </summary>
    /// <param name="hurt">受到的伤害</param>
    public void GetTrueDemage(int hurt)
    {

    }

    /// <summary>
    /// 移除效果
    /// </summary>
    /// <param name="skill">要移除哪个效果</param>
    public void ReliveEffect(E_CardSkill skill)
    {

    }

    /// <summary>
    /// 添加效果
    /// </summary>
    /// <param name="skill">要添加的效果</param>
    public void AddEffect(E_CardSkill skill)
    {
        if (nowEffectedSkills.Contains(skill))
            return;

        nowEffectedSkills.Add(skill);
    }


        #endregion

        #endregion
}