using System;
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
    [Tooltip("怪物有哪些触发特性的方式")]
    public List<E_MonsterTriggerType> triggerTypeList;

    [Header("移动基础配置")]
    [Tooltip("基础左移格数/回合")]
    public int baseMoveStep = 1;
    [Tooltip("移动间隔回合（1=每回合移，2=每2回合移）")]
    public int moveInterval = 1;
    /// <summary>
    /// 当前怪物所在位置
    /// </summary>
    [HideInInspector]
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
    #endregion

    #region 关联组件
    StateMachine machine;
    Animator animator;
    MonsterEffectControl effectControl;
    #endregion

    protected virtual void Awake()
    { 
        //初始化数值
        InitBasicValue();
        // 初始化战场格子坐标（玩家看到4*6，实际上格子是4*7）
        InitMyPos();
        //初始化相关控件
        InitControl();
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
    }

    /// <summary>
    /// 初始化相关控件
    /// </summary>
    private void InitControl()
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
    /// <param name="damage">原始伤害值</param>
    /// <param name="damageElement">伤害元素属性</param>
    /// <param name="isBurn">是否为灼烧伤害</param>
    public virtual void TakeDamage(BaseCard card)
    {
        if (!IsAlive) return;

        // 触发受击特性（如反弹、减伤等）
        int finalDamage = TriggerSpecial(E_MonsterTriggerType.Hurt,card.currentAtk);
        finalDamage = Mathf.Max(finalDamage, 1);
        // 扣血
        currentHp -= finalDamage;
        Debug.Log($"{monsterName}受到{finalDamage}点伤害，当前血量：{currentHp}");

        // 死亡判断
        if (currentHp <= 0)
            Die();
    }

    /// <summary>
    /// 怪物攻击（通用攻击接口，子类可重写扩展AOE/随机攻击等）
    /// </summary>
    /// <param name="target">攻击目标（玩家/阻挡物，可传对应脚本）</param>
    public virtual void Attack(object target)
    {
        if (!IsAlive) return;
        Debug.Log($"{monsterName}对{target}造成{attack}点伤害");
    }

    /// <summary>
    /// 怪物移动(横向)
    /// </summary>
    /// <param name="distance">横向移动总距离</param>
    /// <param name="speed">每次移动的速度(现在怪物每次都移动1格，因为要一格格检测是否能移动)</param>
    public void MoveHorizontal(int distance, int speed = 1)
    {
        GridPos moveSpeed = new GridPos(speed, 0);

        for (int i =0 ; i < distance; i++)
        {
            if (!Move(moveSpeed))
                return;
        }   
    }

    /// <summary>
    /// 怪物移动(纵向)
    /// </summary>
    /// <param name="distance">纵向移动总距离</param>
    /// <param name="speed">每次移动的速度(现在怪物每次都移动1格，因为要一格格检测是否能移动)</param>
    public void MoveVertical(int distance, int speed = 1)
    {
        GridPos moveSpeed = new GridPos(0, speed);
        for (int i = 0; i < distance; i++)
        {
            if (!Move(moveSpeed))
                return;
        }
    }

    #region I_AIAction接口

    /// <summary>
    /// 接口的方法不用在外部,由于架构迫使为public，具体移动由MoveHorizontal，MoveVertical实现
    /// </summary>
    /// <param name="speed">移动的距离(只能是1，0或者0，1)！</param>
    /// <returns>判定移动成功,移动后返回true/移动失败,不移动返回false</returns>
    public bool Move(GridPos speed)
    {
        //校验值是否为规定值
        if (!((speed.x == 1 && speed.y == 0) || (speed.x == 0 && speed.y == 1)))
        {
            Debug.LogError($"Move传入非法参数：{speed}，只能传入 (1,0) 或 (0,1)！");
            return false;
        }

        // 判断是否满足移动条件
        if (!(currentRound % moveInterval == 0))
            return false;

        //如果到达第0列,停止移动并向玩家发起攻击
        if (currentPos.x == 0)
        {
            Atk(PlayerTest.Instance);
            return false;
        }

        // 合法才继续执行
        Cell nextCell = null;

        //判定怪物当前位置是否存在单元格管理器的字典中
        if (!GridMgr.Instance.cellDic.ContainsKey(currentPos))
            return false;

         GridPos nextCellLogicalPos = GridMgr.Instance.cellDic[currentPos].logicalPos + speed;
        nextCell = GridMgr.Instance.cellDic[nextCellLogicalPos];

        //判定前方格子是否被占据，被占据无法移动
        if (!(nextCell.nowStateType == CellStateType.None))
        {
            BeStopped(nextCell);
            return false;
        }

        //播放怪物移动动画
        effectControl.PlayAnimation(E_AIStateType.Move);

        //进行移动（平滑移动)
        Debug.Log($"怪物进行移动，移动目标位置为{nextCell.logicalPos.x}{nextCell.logicalPos.y},目前怪物无法平滑移动没写逻辑");
        this.gameObject.transform.position = nextCell.myWorldPos;
        //更新格子的状态
        nextCell.UpdateOccupiedState(CellStateType.MonsterOccupied,this);
        GridMgr.Instance.cellDic[currentPos].UpdateOccupiedState(CellStateType.None,null);
        //将怪物的单元格位置更新
        currentPos = nextCell.logicalPos;     
        return true;
    }


    /// <summary>
    /// 检查前方格子被占据的对象,多用于移动过程中被强制停止的情况
    /// </summary>
    /// <param name="cell">是被哪个格子阻挡移动</param>
    public void BeStopped(Cell cell)
    {
        Debug.Log("怪物受到阻挡停止移动");
        //检查前方格子范围类型并攻击
        OnStopped(cell);       
    }

    private void OnStopped(Cell cell)
    {
        switch (cell.nowStateType)
        {
            case CellStateType.MonsterOccupied:
            case CellStateType.None:
                break;
            case CellStateType.DefTowerOccupied:
                break;
        }
    }

    public void Atk(BaseLevelObject obj)
    {
        switch (obj.levelObjectType)
        {
            case E_LevelObjectType.Player:
                //调用玩家实例，调用玩家受伤的方法
                PlayerTest.Instance.Hurt(attack);
                break;
            case E_LevelObjectType.Monster://攻击的对象是怪物，不进行攻击
                break;
            case E_LevelObjectType.DefTower://对象是防御塔,调用防御塔的受伤逻辑
                BaseDefTower tower = obj as BaseDefTower;
                tower.Hurt(attack);
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
        //多用于有特殊的移动行为模式（比如移动效果加成或者纵向移动）
        return -1;
    }
    protected virtual int TriggerSpecialOnEnter()
    {
        //通过LevelStepMgr得到当前场地存在的怪物表，然后进行属性加成
        return -1;
    }

    protected virtual int TriggerSpecialOnRound()
    {
        //多用于受到的卡牌技能效果计算

        //更新行动回合
        currentRound++;
        return -1;
    }

    protected virtual int TriggerSpecialOnHpLow()
    {
        //得到玩家单例进行效果实现
        return -1;
    }
    #endregion
    #endregion




    #region 预留扩展接口

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
    #endregion

}