using System.Collections;
using UnityEngine;

/// < summary >
/// 怪物元素属性
/// </ summary >
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

/// <summary>
/// 怪物可以持有的回合buff（含正负面）
/// </summary>
public enum E_MonsterBuffType
{
    /// <summary>
    /// 灼烧
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
/// 怪物抽象基类，作为入口协调各组件，保留基础属性和对外接口
/// </summary>
/// 
[RequireComponent(typeof(MonsterMovement)), RequireComponent(typeof(MonsterBuffHandler)),RequireComponent(typeof(MonsterCombat)),RequireComponent(typeof(MonsterEffectControl))]
public abstract class BaseMonsterCore : BaseGameObject
{
    [Header("怪物基础配置")]
    public string monsterID;
    public string monsterName;
    public int maxHp;
    public int currentHp;
    public int currentAtk;
    public MonsterElement element;
    public MonsterIdentity identity;

    [Header("移动基础配置")]
    [Tooltip("基础左移格数/回合")]
    public int baseMoveStepHorizontal = 1;
    [Tooltip("基础上/下 移格数/回合（没有上下移动的功能就填-1!）")]
    public int baseMoveStepVetical = 1;
    [Tooltip("移动间隔回合（1=每回合移，2=每2回合移）")]
    public int moveInterval = 1;
    [Tooltip("是否可以直接摧毁阻挡物并前进")]
    public bool couldDestoryDefAndAhead;


    // 位置信息（外部需要访问）
    public GridPos currentPos;

    // 是否能受到效果攻击（外部卡牌逻辑设置）
    [HideInInspector]
    public bool isAllowedEffected = true;

    //怪物是否存活
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
        if (movement == null) Debug.LogError("没有挂载MonsterMovement");
        buffHandler = GetComponent<MonsterBuffHandler>();
        if (buffHandler == null) Debug.LogError("没有挂载MonsterBuffHandler");
        combat = GetComponent<MonsterCombat>();
        if (combat == null) Debug.LogError("没有挂载MonsterCombat");
        effectControl = GetComponent<MonsterEffectControl>();
        if (effectControl == null) Debug.LogError("没有挂载MonsterEffectControl");


        // 初始化组件（传递必要引用）
        movement.Init(this, effectControl);
        buffHandler.Init(this, effectControl);
        combat.Init(this, effectControl);
        effectControl.Init(maxHp);

        // 初始化数值
        currentHp = maxHp;
    }

    protected virtual void Start()
    {
        // 触发进入战场特性
        TriggerOnEnter(new MonsterOnEnter());
    }


    /// <summary>
    /// 更新怪物的格子位置
    /// </summary>
    public void UpdateMyGridPos(GridPos myPos)
    {
        currentPos = myPos;
    }

    #region 对外接口（转发给组件）
    public void TakeDamage(int atk,E_Element element,E_CardSkill skill) => combat.TakeDamage(atk,element, skill);
    public void Die() => combat.Die();

    public void OnRoundUpdate()
    {
        movement.OnRoundUpdate();      // 增加回合计数
        buffHandler.OnRoundUpdate();   // 结算状态效果

        MonsterOnRound evt = new MonsterOnRound();
        evt.currentPos = currentPos;
        TriggerOnRound(evt); // 触发回合特性
    }

    public IEnumerator MoveHorizontal(int steps, int speed = -1) => movement.MoveHorizontal(steps, speed);
    public IEnumerator MoveVertical(int steps, int speed = 1, bool isForced = false) => movement.MoveVertical(steps, speed, isForced);

    // 卡牌效果接口
    public void GetBurn(int duration) => buffHandler.ApplyBuff(E_MonsterBuffType.Burn, duration);
    public void GetImprison(int duration) => buffHandler.ApplyBuff(E_MonsterBuffType.Imprison, duration);
    public virtual void GetRepel(BaseCard card, Cell coreCell) => movement.GetRepel(card, coreCell);

    public void GetHeal(int healValue) => combat.GetHeal(healValue);
    #endregion

    #region 特性虚方法（子类重写）
    protected virtual void OnHurtSpecial(MonsterOnHurt evt)
    {
        //如果是真伤，就不进行伤害减免计算了
        if (evt.cardSkill == E_CardSkill.TrueDamage)
            return;
    }
    protected virtual void OnMoveSpecial(MonsterOnMove evt) { }
    protected virtual void OnEnterSpecial(MonsterOnEnter evt) { }
    protected virtual void OnRoundSpecial(MonsterOnRound evt) { }
    protected virtual void OnHpLowSpecial(MonsterOnHpLow evt) { }
    protected virtual void OnDeadSpecial(MonsterOnDead evt) { }
    protected virtual void OnGetDeBuffSpecial(MonsterOnGetDeBuff evt) { }
    protected virtual void OnAtkSpecial(MonsterOnAtk evt) { }
    #endregion

    #region 触发特性（供组件调用）
    public void TriggerOnHurt(MonsterOnHurt evt) => OnHurtSpecial(evt);
    public void TriggerOnMove(MonsterOnMove evt) => OnMoveSpecial(evt);
    public void TriggerOnEnter(MonsterOnEnter evt) => OnEnterSpecial(evt);
    public void TriggerOnRound(MonsterOnRound evt) => OnRoundSpecial(evt);
    public void TriggerOnHpLow(MonsterOnHpLow evt) => OnHpLowSpecial(evt);
    public void TriggerOnDead(MonsterOnDead evt) => OnDeadSpecial(evt);
    public void TriggerOnGetDeBuff(MonsterOnGetDeBuff evt) => OnGetDeBuffSpecial(evt);

    public void TriggerOnAtk(MonsterOnAtk evt) => OnAtkSpecial(evt);
    #endregion
}