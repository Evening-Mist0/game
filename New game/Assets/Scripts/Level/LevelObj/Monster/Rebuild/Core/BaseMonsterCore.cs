using System.Collections;
using UnityEngine;

/// <summary>
/// 怪物元素属性枚举
/// </summary>
public enum MonsterElement
{
    None,
    Fire,
    Water,
    Earth
}

/// <summary>
/// 怪物身份类型枚举
/// </summary>
public enum MonsterIdentity
{
    Basic,
    Elite,
    Boss
}

/// <summary>
/// 怪物触发事件类型枚举
/// </summary>
public enum E_MonsterTriggerType
{
    Death,
    Hurt,
    Move,
    Enter,
    Round,
    HpLow
}

/// <summary>
/// 怪物可被施加的BUFF类型枚举
/// </summary>
public enum E_MonsterBuffType
{
    Burn,
    Imprison,
    SpeedUp,
    Weakness,
}

/// <summary>
/// 怪物核心基类，作为所有怪物的共同父类
/// </summary>

public abstract class BaseMonsterCore : BaseGameObject
{
    [Header("怪物数据配置（SO）")]
    public BaseMonsterScriptableData monsterData;

    #region 运行时动态数据
    [Header("运行时数值")]
    [HideInInspector] public string monsterID;
    [HideInInspector] public string monsterName;
    [HideInInspector] public int maxHp;
    public int currentAtk;
    [HideInInspector] public int nowDef;
    [HideInInspector] public MonsterElement element;
    [HideInInspector] public MonsterIdentity identity;

    [Header("移动行为设置")]
    [HideInInspector] public int baseMoveStepHorizontal = 1;
    [HideInInspector] public int baseMoveStepVertical = 1;
    [HideInInspector] public int moveInterval = 1;
    [HideInInspector] public bool couldDestroyDefAndAhead;

    [Header("动态状态")]
    [HideInInspector] public int currentHp;// 当前血量
    #endregion

    // 网格位置信息
    public GridPos currentPos;

    // 是否可以被效果影响
    [HideInInspector] public bool isAllowedEffected = true;

    // 是否存活
    [HideInInspector] public bool IsAlive => currentHp > 0;

    // 组件引用
    [HideInInspector] public MonsterMovement movement;
    [HideInInspector] public MonsterBuffHandler buffHandler;
    [HideInInspector] public MonsterCombat combat;
    [HideInInspector] public MonsterEffectControl effectControl;
    [HideInInspector] public MonsterCardDrop cardDrop;

    [Header("动画设置")]
    protected Animator _animator;

    protected virtual void Awake()
    {
        // 1. 初始化数值（从SO加载）
        InitMonsterValue();

        // 2. 获取组件
        movement = GetComponent<MonsterMovement>();
        if (movement == null) Debug.LogError("未找到组件：MonsterMovement");

        buffHandler = GetComponent<MonsterBuffHandler>();
        if (buffHandler == null) Debug.LogError("未找到组件：MonsterBuffHandler");

        combat = GetComponent<MonsterCombat>();
        if (combat == null) Debug.LogError("未找到组件：MonsterCombat");

        effectControl = GetComponent<MonsterEffectControl>();
        if (effectControl == null)
            effectControl = this.gameObject.GetComponentInChildren<MonsterEffectControl>();
        
        if(effectControl == null)
        Debug.LogError("未找到组件：MonsterEffectControl");

        cardDrop = GetComponent<MonsterCardDrop>();
        if (cardDrop == null) Debug.LogError("未找到组件：MonsterCardDrop");

        // 3. 初始化子模块
        movement.Init(this, effectControl);
        buffHandler.Init(this, effectControl);
        combat.Init(this, effectControl);
        effectControl.Init(maxHp, maxHp, nowDef, this);
        cardDrop.Init(this, effectControl);

        // 4. 初始化当前血量
        currentHp = maxHp;

        _animator = GetComponent<Animator>();
        if (_animator == null) Debug.LogWarning("怪物未挂载Animator组件，无法播放动画！");
    }

    protected virtual void Start()
    {
        // 触发进入战斗事件
        MonsterOnEnter evt = new MonsterOnEnter();
        evt.currentPos = currentPos;
        TriggerOnEnter(evt);
    }

    /// <summary>
    /// 从ScriptableObject初始化怪物的静态数值（参照BaseCard.InitCardValue）
    /// </summary>
    protected virtual void InitMonsterValue()
    {
        if (monsterData == null)
        {
            Debug.LogError($"怪物 {gameObject.name} 的 monsterData 未配置！");
            return;
        }

        // 基础数值
        monsterID = monsterData.monsterID;
        monsterName = monsterData.monsterName;
        maxHp = monsterData.maxHp;
        currentAtk = monsterData.baseAtk;
        nowDef = monsterData.baseDef;
        element = monsterData.element;
        identity = monsterData.identity;

        // 移动行为
        baseMoveStepHorizontal = monsterData.baseMoveStepHorizontal;
        baseMoveStepVertical = monsterData.baseMoveStepVertical;
        moveInterval = monsterData.moveInterval;
        couldDestroyDefAndAhead = monsterData.couldDestroyDefAndAhead;
    }

    /// <summary>
    /// 更新当前所在的网格坐标
    /// </summary>
    public void UpdateMyGridPos(GridPos myPos)
    {
        currentPos = myPos;
    }

    #region 外部调用接口
    public void TakeDamage(int atk, E_Element element, E_AtkType atkType, bool isTrueDamage) => combat.TakeDamage(atk, element, atkType, isTrueDamage);
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
    public virtual void GetRepel(BaseCard card, Cell coreCell, int effectValue) => movement.GetRepel(card, coreCell, effectValue);
    public void GetHeal(int healValue) => combat.GetHeal(healValue);

    public void GetWeakness(int duration) => buffHandler.ApplyBuff(E_MonsterBuffType.Weakness, duration);

    #region 子类可重写的特殊逻辑
    protected virtual void OnHurtSpecial(MonsterOnHurt evt)
    {
        // 真实伤害不触发额外效果
        if (evt.isTrueDamage == true)
            return;
    }

    protected virtual void OnMoveSpecial(MonsterOnMove evt) { }
    protected virtual void OnMoveOverSpecial(MonsterOnMoveOver evt) { }
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
    public void TriggerOnHurt(MonsterOnHurt evt)
    {
        OnHurtSpecial(evt);
    }
    public void TriggerOnMove(MonsterOnMove evt) => OnMoveSpecial(evt);
    public void TriggerOnMoveOver(MonsterOnMoveOver evt) => OnMoveOverSpecial(evt);
    public void TriggerOnEnter(MonsterOnEnter evt) => OnEnterSpecial(evt);
    public void TriggerOnRound(MonsterOnRound evt) => OnRoundSpecial(evt);
    public void TriggerOnHpLow(MonsterOnHpLow evt) => OnHpLowSpecial(evt);
    public void TriggerOnDead(MonsterOnDead evt)
    {
        OnDeadSpecial(evt);
        cardDrop.TryDropCard();
    }
    public void TriggerOnGetDeBuff(MonsterOnGetDeBuff evt) => OnGetDeBuffSpecial(evt);
    public void TriggerOnAtk(MonsterOnAtk evt) => OnAtkSpecial(evt);
    #endregion
    #endregion
}