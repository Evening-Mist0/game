using UnityEngine;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 元素神的形态状态
/// </summary>
public enum E_ElementGodState
{
    FireFrom,
    WaterForm,
    EarthForm,
}

/// <summary>
/// 万法元素神BOSS核心脚本
/// </summary>
public class None01_GodofAllElementalArts : BaseMonsterCore
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.Monster;

    /// <summary>
    /// 当前形态
    /// </summary>
    public E_ElementGodState nowState = E_ElementGodState.WaterForm;

    //[Tooltip("基础防御力")]
    //public int def;

    [Tooltip("火焰形态攻击力")]
    public int fireFormAtk;
    [Tooltip("水形态攻击力")]
    public int waterFormAtk;
    [Tooltip("水形态每次向上移动的距离")]
    public int verticalDistance = 2;

    /// <summary>
    /// 水形态每次攻击的目标数量（当前固定为2个）
    /// </summary>
    private int waterFormAtkCount = 2;

    [Tooltip("大地形态攻击力")]
    public int earthFormAtk;
    [Tooltip("大地形态反弹伤害")]
    public int earthFormReflectAtk;
    [Tooltip("大地形态每回合增加的护盾值")]
    public int addDefValue;


    [Tooltip("元素湮灭技能伤害值(真实伤害)")]
    public int ElementAnnihilationAtk;

    /// <summary>
    /// 是否已经释放过一次元素湮灭
    /// </summary>
    private bool isReleaseElementAnnihilation;

    /// <summary>
    /// 是否播放水形态攻击动画
    /// </summary>
    [HideInInspector]
    private bool isPlayWaterFormAtk;

    protected override void Awake()
    {
        base.Awake();
        //初始化形态
        ChangeState(E_ElementGodState.FireFrom);
    }

    protected override void OnHurtSpecial(MonsterOnHurt evt)
    {
        base.OnHurtSpecial(evt);
       

        switch (nowState)
        {
            case E_ElementGodState.FireFrom:
                // 火焰形态：受到火系卡牌攻击时，减免伤害
                if (evt.cardElement == E_Element.Fire)
                {
                    switch (evt.atkType)
                    {
                        case E_AtkType.CardAtk:
                            evt.resultAtk /= 2;
                            break;
                        case E_AtkType.BurnSkill:                     
                        case E_AtkType.DefAtk:
                            evt.resultAtk = 0;
                            break;
                    }      
                }
                break;
            case E_ElementGodState.WaterForm:
                break;

            case E_ElementGodState.EarthForm:
                if(evt.atkType == E_AtkType.CardAtk)
                GamePlayer.Instance.Hurt(earthFormReflectAtk, true);
                break;
        }
    }

    protected override void OnEnterSpecial(MonsterOnEnter evt)
    {       
        switch (nowState)
        {
            case E_ElementGodState.FireFrom://入场火系特效
                break;
            case E_ElementGodState.WaterForm://入场水系特效
                break;
            case E_ElementGodState.EarthForm://入场土系特效
                break;
        }
    }

    protected override void OnHpLowSpecial(MonsterOnHpLow evt)
    {
        base.OnHpLowSpecial(evt);
        // 血量阈值触发形态切换
        if (currentHp <= 23 && (nowState == E_ElementGodState.FireFrom))
        {
            Debug.Log("检测到BOSS血量小于23，切换为水形态");
            currentHp = 23;
            effectControl.UpdateBlood(currentHp, maxHp);
            ChangeState(E_ElementGodState.WaterForm);
        }
        else if (currentHp <= 11 && (nowState == E_ElementGodState.WaterForm))
        {
            currentHp = 11;
            effectControl.UpdateBlood(currentHp, maxHp);
            Debug.Log("检测到BOSS血量小于11，切换为大地形态");
            ChangeState(E_ElementGodState.EarthForm);
        }
        else if (currentHp <= 8 && (isReleaseElementAnnihilation == false))
        {
            // 血量极低时释放必杀技
            isReleaseElementAnnihilation = true;
            ElementAnnihilation();
        }
    }

    protected override void OnAtkSpecial(MonsterOnAtk evt)
    {
        base.OnAtkSpecial(evt);
        evt.isElementGodAtk = true;

        switch (nowState)
        {
            case E_ElementGodState.FireFrom:

                Debug.Log($"BOSS攻击的元素位置{evt.nowPos.x}{evt.nowPos.y}");
                if (evt.nowPos.x <= 3)
                {
                    effectControl.PlayAtkAnimation(E_AttackAnimType.Boss_God_FireFormAtk);
                    // 攻击同一行所有防御塔
                    for (int i = 0; i < GridMgr.Instance.gridWideCount; i++)
                    {
                        GridPos posFireForm1 = new GridPos(i, evt.nowPos.y);
                        if (GridMgr.Instance.cellDic[posFireForm1].nowObj != null)
                        {
                            if (GridMgr.Instance.cellDic[posFireForm1].nowObj.gameObjectType == E_GameObjectType.DefTower)
                            {
                                BaseDefTower tower = GridMgr.Instance.cellDic[posFireForm1].nowObj as BaseDefTower;
                                tower.Hurt(this);
                            }
                        }
                    }
                    // 攻击玩家
                    GamePlayer.Instance.Hurt(currentAtk);
                }
                else
                {
                    GridPos posFireForm2 = new GridPos(evt.nowPos.x - 1, evt.nowPos.y);
                    if (GridMgr.Instance.cellDic[posFireForm2].nowObj != null)
                    {
                        if (GridMgr.Instance.cellDic[posFireForm2].nowObj.gameObjectType == E_GameObjectType.DefTower)
                        {
                            BaseDefTower tower = GridMgr.Instance.cellDic[posFireForm2].nowObj as BaseDefTower;
                            tower.Hurt(this);
                        }
                    }
                }
                break;

            case E_ElementGodState.WaterForm:
                if (evt.isMonster)
                    return;

                effectControl.PlayAtkAnimation(E_AttackAnimType.Boss_God_WaterFormAtk);

                // 随机攻击两个防御塔
                for (int i = 0; i < waterFormAtkCount; i++)
                {
                    int random = Random.Range(0, GridMgr.Instance.gridWideCount - 1);
                    for (int j = 0; j < GridMgr.Instance.gridHighCount - 1; j++)
                    {
                        GridPos posWaterForm = new GridPos(random, j);
                        if (GridMgr.Instance.cellDic[posWaterForm].nowObj != null)
                        {
                            if (GridMgr.Instance.cellDic[posWaterForm].nowObj.gameObjectType == E_GameObjectType.DefTower)
                            {
                                GridMgr.Instance.cellDic[posWaterForm].myUIControl.EnterHighLight();
                                BaseDefTower tower = GridMgr.Instance.cellDic[posWaterForm].nowObj as BaseDefTower;
                                tower.Hurt(this);
                            }
                        }
                    }
                }
                // 攻击玩家
                GamePlayer.Instance.Hurt(currentAtk);
                break;

            case E_ElementGodState.EarthForm:
                GridPos posEarthForm = new GridPos(evt.nowPos.x - 1, evt.nowPos.y);
                if (evt.nowPos.x == 0)//如果在最左列直接攻击
                {
                    GamePlayer.Instance.Hurt(currentAtk);
                }
                else
                {
                    if (GridMgr.Instance.cellDic.ContainsKey(posEarthForm))
                    {

                        if (GridMgr.Instance.cellDic[posEarthForm].nowObj != null)
                        {
                            BaseGameObject target = GridMgr.Instance.cellDic[posEarthForm].nowObj;
                            if (target.gameObjectType == E_GameObjectType.DefTower)
                            {
                                var tower = target as BaseDefTower;
                                Debug.Log($"{monsterName} 攻击防御塔{tower.name}，造成 {currentAtk} 点伤害");
                                tower?.Hurt(this,true);
                            }


                        }
                    }
                }


                break;
        }
    }

    protected override void OnMoveSpecial(MonsterOnMove evt)
    {
        base.OnMoveSpecial(evt);
        switch (nowState)
        {
            case E_ElementGodState.FireFrom:
                break;

            case E_ElementGodState.WaterForm:
                // 水形态可以直接摧毁防御塔前进
                evt.isCoundDestoryDef = couldDestoryDefAndAhead;
                break;

            case E_ElementGodState.EarthForm:
                break;
        }
    }

    protected override void OnGetDeBuffSpecial(MonsterOnGetDeBuff evt)
    {
        base.OnGetDeBuffSpecial(evt);
        switch (nowState)
        {
            case E_ElementGodState.FireFrom:
                evt.isImmunityImprison = false;
                break;
            case E_ElementGodState.WaterForm:
                evt.isImmunityImprison = true;
                break;
            case E_ElementGodState.EarthForm:
                evt.isImmunityImprison = false;
                break;
        }
    }

    protected override void OnRoundSpecial(MonsterOnRound evt)
    {
        base.OnRoundSpecial(evt);
        nowDef = 0;
        switch (nowState)
        {
            case E_ElementGodState.FireFrom:
            case E_ElementGodState.WaterForm:
                break;

            case E_ElementGodState.EarthForm:
                // 每回合刷新护盾
                nowDef += addDefValue;
                effectControl.UpdateDef(nowDef);
                break;
        }
    }

    /// <summary>
    /// 进入火焰形态逻辑
    /// </summary>
    private void OnEnterFireForm()
    {
        //添加自身固有技能图标
        effectControl.AddBuffIcon(E_BuffIconType.Move);
        effectControl.UpdateIconCount(E_BuffIconType.Move, movement.MoveInterval - movement.CurrentRound);
        effectControl.AddBuffIcon(E_BuffIconType.ImmunityBurn);
        effectControl.AddBuffIcon(E_BuffIconType.FireDamegeRedution);
        nowState = E_ElementGodState.FireFrom;
        currentAtk = fireFormAtk;
        couldDestoryDefAndAhead = false;
        baseMoveStepVetical = 0;
    }

    /// <summary>
    /// 进入水形态逻辑
    /// </summary>
    private void OnEnterWaterForm()
    {
        //添加自身固有技能图标
        effectControl.RemoveBuffIcon(E_BuffIconType.FireDamegeRedution);
        effectControl.RemoveBuffIcon(E_BuffIconType.ImmunityBurn);
        effectControl.AddBuffIcon(E_BuffIconType.ImmunityImprison);
        effectControl.AddBuffIcon(E_BuffIconType.DestroyBuildings);
        //切换当前形态
        nowState = E_ElementGodState.WaterForm;
        //设置攻击力
        currentAtk = waterFormAtk;
        //可以直接摧毁防御塔前进
        couldDestoryDefAndAhead = true;
        //设置垂直移动距离
        baseMoveStepVetical = verticalDistance;
    }

    /// <summary>
    /// 进入大地形态逻辑
    /// </summary>
    private void OnEnterEarthForm()
    {
        effectControl.RemoveBuffIcon(E_BuffIconType.ImmunityImprison);
        effectControl.RemoveBuffIcon(E_BuffIconType.DestroyBuildings);
        effectControl.AddBuffIcon(E_BuffIconType.Reflect);
        effectControl.AddBuffIcon(E_BuffIconType.ArbitraryDamegeRedution);
        effectControl.AddBuffIcon(E_BuffIconType.GetDef);
        effectControl.AddBuffIcon(E_BuffIconType.AnnihilationOfElements);

        nowState = E_ElementGodState.EarthForm;
        currentAtk = earthFormAtk;
        couldDestoryDefAndAhead = false;
        baseMoveStepVetical = 0;
    }

    /// <summary>
    /// 切换BOSS形态
    /// </summary>
    private void ChangeState(E_ElementGodState state)
    {
        switch (state)
        {
            case E_ElementGodState.FireFrom:
                OnEnterFireForm();
                break;
            case E_ElementGodState.WaterForm:
                OnEnterWaterForm();
                break;
            case E_ElementGodState.EarthForm:
                OnEnterEarthForm();
                break;
        }
        nowState = state;
    }

    /// <summary>
    /// 元素湮灭（必杀技）
    /// </summary>
    private void ElementAnnihilation()
    {
        //删去技能图标
        effectControl.RemoveBuffIcon(E_BuffIconType.AnnihilationOfElements);
        Debug.Log("释放元素湮灭");
        //对玩家造成伤害
        GamePlayer.Instance.Hurt(ElementAnnihilationAtk, true);
        //清空玩家手牌
        Dealer.Instance.RemoveAllBasicCards();
    }
}