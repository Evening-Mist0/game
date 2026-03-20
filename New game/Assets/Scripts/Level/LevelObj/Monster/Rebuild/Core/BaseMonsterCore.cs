//using System.Collections;
//using UnityEngine;

///// <summary>
///// 怪物抽象基类，作为入口协调各组件，保留基础属性和对外接口
///// </summary>
//public abstract class BaseMonsterCore : BaseGameObject
//{
//    [Header("怪物基础配置")]
//    public string monsterID;
//    public string monsterName;
//    public int maxHp;
//    public int currentHp;
//    public int attack;
//    public MonsterElement element;
//    public MonsterIdentity identity;

//    [Header("移动基础配置")]
//    public int baseMoveStepHorizontal = 1;
//    public int baseMoveStepVertical = 1;
//    public int moveInterval = 1;

//    // 位置信息（外部需要访问）
//    public GridPos currentPos;

//    // 是否能受到效果攻击（外部卡牌逻辑设置）
//    [HideInInspector]
//    public bool isAllowedEffected = true;

//    //怪物是否存活
//    [HideInInspector]
//    public bool IsAlive => currentHp > 0;

//    // 组件引用
//    protected MonsterMovement movement;
//    protected MonsterBuffHandler buffHandler;
//    protected MonsterCombat combat;
//    protected MonsterEffectControl effectControl;

//    protected virtual void Awake()
//    {
//        // 获取组件
//        movement = GetComponent<MonsterMovement>();
//        buffHandler = GetComponent<MonsterBuffHandler>();
//        combat = GetComponent<MonsterCombat>();
//        effectControl = GetComponent<MonsterEffectControl>();

//        // 初始化组件（传递必要引用）
//        movement.Init(this, effectControl);
//        buffHandler.Init(this, effectControl);
//        combat.Init(this, effectControl);

//        // 初始化数值
//        currentHp = maxHp;
//        InitMyPos(); // 子类可重写
//    }

//    protected virtual void Start()
//    {
//        // 触发进入战场特性
//        TriggerOnEnter(new MonsterOnEnter());
//    }

//    /// <summary>
//    /// 初始化位置（子类可重写）
//    /// </summary>
//    public virtual void InitMyPos()
//    {
//        currentPos = new GridPos(GridMgr.Instance.gridWideCount - 1, GridMgr.Instance.gridHighCount - 1);
//    }

//    #region 对外接口（转发给组件）
//    public void TakeDamage(int atk, E_CardSkill skill) => combat.TakeDamage(atk, skill);
//    public void Die() => combat.Die();

//    public void OnRoundUpdate()
//    {
//        movement.OnRoundUpdate();   // 增加回合计数
//        buffHandler.OnRoundUpdate(); // 结算状态效果
//    }

//    public IEnumerator MoveHorizontal(int steps, int speed = 1) => movement.MoveHorizontal(steps, speed);
//    public IEnumerator MoveVertical(int steps, int speed = 1, bool isForced = false) => movement.MoveVertical(steps, speed, isForced);

//    // 卡牌效果接口
//    public void GetBurn(int duration) => buffHandler.ApplyBuff(E_MonsterBuffType.Burn, duration);
//    public void GetImprison(int duration) => buffHandler.ApplyBuff(E_MonsterBuffType.Imprison, duration);
//    public virtual void GetRepel(BaseCard card, Cell coreCell) => movement.GetRepel(card, coreCell);
//    #endregion

//    #region 特性虚方法（子类重写）
//    protected virtual void OnHurtSpecial(MonsterOnHurt evt) { }
//    protected virtual void OnMoveSpecial(MonsterOnMove evt) { }
//    protected virtual void OnEnterSpecial(MonsterOnEnter evt) { }
//    protected virtual void OnRoundSpecial(MonsterOnRound evt) { }
//    protected virtual void OnHpLowSpecial(MonsterOnHpLow evt) { }
//    protected virtual void OnDeadSpecial(MonsterOnDead evt) { }
//    protected virtual void OnGetDeBuffSpecial(MonsterOnGetDeBuff evt) { }
//    #endregion

//    #region 触发特性（供组件调用）
//    public void TriggerOnHurt(MonsterOnHurt evt) => OnHurtSpecial(evt);
//    public void TriggerOnMove(MonsterOnMove evt) => OnMoveSpecial(evt);
//    public void TriggerOnEnter(MonsterOnEnter evt) => OnEnterSpecial(evt);
//    public void TriggerOnRound(MonsterOnRound evt) => OnRoundSpecial(evt);
//    public void TriggerOnHpLow(MonsterOnHpLow evt) => OnHpLowSpecial(evt);
//    public void TriggerOnDead(MonsterOnDead evt) => OnDeadSpecial(evt);
//    public void TriggerOnGetDeBuff(MonsterOnGetDeBuff evt) => OnGetDeBuffSpecial(evt);
//    #endregion
//}