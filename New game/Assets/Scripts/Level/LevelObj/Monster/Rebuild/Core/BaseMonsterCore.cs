using System.Collections;
using UnityEngine;

/// <summary>
/// 怪物元素属性枚举
/// </summary>
public enum MonsterElement
{
    /// <summary>
    /// 无属性
    /// </summary>
    None,
    /// <summary>
    /// 火属性
    /// </summary>
    Fire,
    /// <summary>
    /// 水属性
    /// </summary>
    Water,
    /// <summary>
    /// 地属性
    /// </summary>
    Earth
}

/// <summary>
/// 怪物身份类型枚举
/// </summary>
public enum MonsterIdentity
{
    /// <summary>
    /// 普通怪
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
/// 怪物触发事件类型枚举
/// </summary>
public enum E_MonsterTriggerType
{
    /// <summary>
    /// 死亡事件
    /// </summary>
    Death,
    /// <summary>
    /// 受伤事件
    /// </summary>
    Hurt,
    /// <summary>
    /// 移动事件
    /// </summary>
    Move,
    /// <summary>
    /// 进入战斗事件
    /// </summary>
    Enter,
    /// <summary>
    /// 每回合结束
    /// </summary>
    Round,
    /// <summary>
    /// 当前生命值过低
    /// </summary>
    HpLow
}

/// <summary>
/// 怪物可被施加的BUFF类型枚举
/// </summary>
public enum E_MonsterBuffType
{
    /// <summary>
    /// 燃烧
    /// </summary>
    Burn,
    /// <summary>
    /// 禁锢
    /// </summary>
    Imprison,
    /// <summary>
    /// 加速
    /// </summary>
    SpeedUp,
}

/// <summary>
/// 怪物核心基类，作为所有怪物的共同父类
/// </summary>
[RequireComponent(typeof(MonsterMovement)), RequireComponent(typeof(MonsterBuffHandler)), RequireComponent(typeof(MonsterCombat)), RequireComponent(typeof(MonsterEffectControl))]
public abstract class BaseMonsterCore : BaseGameObject
{
    [Header("怪物基础数值")]
    public string monsterID;
    public string monsterName;
    public int maxHp;
    public int currentHp;
    public int currentAtk;
    public int nowDef;
    public MonsterElement element;
    public MonsterIdentity identity;

    [Header("移动行为设置")]
    [Tooltip("基础横向移动步数/每回合")]
    public int baseMoveStepHorizontal = 1;
    [Tooltip("基础纵向移动步数/每回合，没有移动能力填-1！")]
    public int baseMoveStepVetical = 1;
    [Tooltip("移动间隔回合，1=每回合移动，2=每2回合移动")]
    public int moveInterval = 1;
    [Tooltip("是否可以直接摧毁前方障碍物")]
    public bool couldDestoryDefAndAhead;

    // 网格位置信息
    public GridPos currentPos;

    // 是否可以被效果影响
    [HideInInspector]
    public bool isAllowedEffected = true;

    // 是否存活
    [HideInInspector]
    public bool IsAlive => currentHp > 0;

    // 组件引用
    [HideInInspector]
    public MonsterMovement movement;
    [HideInInspector]
    public MonsterBuffHandler buffHandler;
    [HideInInspector]
    public MonsterCombat combat;
    [HideInInspector]
    public MonsterEffectControl effectControl;

    protected virtual void Awake()
    {
        // 获取组件
        movement = GetComponent<MonsterMovement>();
        if (movement == null) Debug.LogError("未找到组件：MonsterMovement");

        buffHandler = GetComponent<MonsterBuffHandler>();
        if (buffHandler == null) Debug.LogError("未找到组件：MonsterBuffHandler");

        combat = GetComponent<MonsterCombat>();
        if (combat == null) Debug.LogError("未找到组件：MonsterCombat");

        effectControl = GetComponent<MonsterEffectControl>();
        if (effectControl == null) Debug.LogError("未找到组件：MonsterEffectControl");

        // 初始化子模块
        movement.Init(this, effectControl);
        buffHandler.Init(this, effectControl);
        combat.Init(this, effectControl);
        effectControl.Init(maxHp, maxHp, nowDef, this);

        // 初始化血量
        currentHp = maxHp;
    }

    protected virtual void Start()
    {
        // 触发进入战斗事件
        MonsterOnEnter evt = new MonsterOnEnter();
        evt.currentPos = currentPos;
        TriggerOnEnter(evt);
    }

    /// <summary>
    /// 更新当前所在的网格坐标
    /// </summary>
    public void UpdateMyGridPos(GridPos myPos)
    {
        currentPos = myPos;
    }

    #region 外部调用接口
    public void TakeDamage(int atk, E_Element element,E_AtkType atkType,bool isTrueDamage) => combat.TakeDamage(atk, element,atkType,isTrueDamage);
    public void Die() => combat.Die();

    public void OnRoundUpdate()
    {
        MonsterOnRound evt = new MonsterOnRound();
        evt.currentPos = currentPos;
        TriggerOnRound(evt);

        movement.OnRoundUpdate();
        buffHandler.OnRoundUpdate();     
    }

    /// <summary>
    /// 加血（UI效果同时更新）
    /// </summary>
    public void AddHp(int value)
    {
        currentHp += value;
        if (currentHp > maxHp)
            currentHp = maxHp;

       effectControl.UpdateBlood(currentHp, maxHp);
    }

    public IEnumerator MoveHorizontal(int steps, int speed = -1) => movement.MoveHorizontal(steps, speed);
    public IEnumerator MoveVertical(int steps, int speed = 1, bool isForced = false) => movement.MoveVertical(steps, speed, isForced);

    // BUFF效果接口
    public void GetBurn(int duration) => buffHandler.ApplyBuff(E_MonsterBuffType.Burn, duration);
    public void GetImprison(int duration) => buffHandler.ApplyBuff(E_MonsterBuffType.Imprison, duration);
    public virtual void GetRepel(BaseCard card, Cell coreCell,int effectValue) => movement.GetRepel(card, coreCell, effectValue);

    public void GetHeal(int healValue) => combat.GetHeal(healValue);
    #endregion

    #region 子类可重写的特殊逻辑
    protected virtual void OnHurtSpecial(MonsterOnHurt evt)
    {
        // 真实伤害不触发额外效果
        if (evt.isTrueDamage == true)
            return;
    }

    protected virtual void OnMoveSpecial(MonsterOnMove evt) { }

    protected virtual void OnMoveOverSpecial(MonsterOnMoveOver evt)
    {

    }
    protected virtual void OnEnterSpecial(MonsterOnEnter evt)
    {
        effectControl.AddBuffIcon(E_BuffIconType.Move);
        effectControl.UpdateIconCount(E_BuffIconType.Move, movement.MoveInterval - movement.CurrentRound);
    }
    protected virtual void OnRoundSpecial(MonsterOnRound evt) { }
    protected virtual void OnHpLowSpecial(MonsterOnHpLow evt) { }
    protected virtual void OnDeadSpecial(MonsterOnDead evt) { }
    protected virtual void OnGetDeBuffSpecial(MonsterOnGetDeBuff evt) { }
    protected virtual void OnAtkSpecial(MonsterOnAtk evt) { }
    #endregion

    #region 事件触发方法
    public void TriggerOnHurt(MonsterOnHurt evt) => OnHurtSpecial(evt);
    public void TriggerOnMove(MonsterOnMove evt) => OnMoveSpecial(evt);
    public void TriggerOnMoveOver(MonsterOnMoveOver evt) => OnMoveOverSpecial(evt);
    public void TriggerOnEnter(MonsterOnEnter evt) => OnEnterSpecial(evt);
    public void TriggerOnRound(MonsterOnRound evt) => OnRoundSpecial(evt);
    public void TriggerOnHpLow(MonsterOnHpLow evt) => OnHpLowSpecial(evt);
    public void TriggerOnDead(MonsterOnDead evt) => OnDeadSpecial(evt);
    public void TriggerOnGetDeBuff(MonsterOnGetDeBuff evt) => OnGetDeBuffSpecial(evt);
    public void TriggerOnAtk(MonsterOnAtk evt) => OnAtkSpecial(evt);
    #endregion
}