using System.Collections;
using System.Threading;
using UnityEngine;

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



    [Header("===== 火焰形态 =====")]
    [Header("火焰形态攻击力")]
    public int fireFormAtk;
    [Header("火焰形态每回合基础生成数量")]
    public int fireFormMonsterCount;
    [Header("火焰形态每回合精英生成数量")]
    public int fireFormEliteMonsterCount;
    [Header("火焰形态移动间隔")]
    public int fireFormMoveInterval;

    [Header("===== 水形态 =====")]
    [Header("水形态攻击力")]
    public int waterFormAtk;
    [Header("水形态每回合基础生成数量")]
    public int waterFormMonsterCount;
    [Header("水形态每回合精英生成数量")]
    public int waterFormEliteMonsterCount;
    [Header("水形态每次向上移动的距离")]
    public int verticalDistance = 2;
    [Header("水形态移动间隔")]
    public int waterFormMoveInterval;

    /// <summary>
    /// 水形态每次攻击的目标数量（当前固定为2个）
    /// </summary>
    private int waterFormAtkCount = 2;

    [Header("===== 大地形态 =====")]
    [Header("大地形态攻击力")]
    public int earthFormAtk;
    [Header("大地形态移动间隔")]
    public int earthFormMoveInterval;
    [Header("大地形态每回合基础生成数量")]
    public int earthFormMonsterCount;
    [Header("大地形态每回合精英生成数量")]
    public int earthFormEliteMonsterCount;
    [Header("大地形态反弹伤害")]
    public int earthFormReflectAtk;
    [Header("大地形态每回合增加的护盾值")]
    public int addDefValue;
    [Header("大地形态每回合回复的生命")]
    public int addHealValue;

    [Header("===== 技能 =====")]
    [Header("元素湮灭技能伤害值(真实伤害)")]
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

    public BuffIconControl atributeIcon;

    protected override void Awake()
    {
        base.Awake();
   
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
        base.OnEnterSpecial(evt);
        switch (nowState)
        {
            case E_ElementGodState.FireFrom://入场火系特效
                ChangeState(E_ElementGodState.FireFrom);
                //向前移动一次
                StartCoroutine(MoveHorizontal(baseMoveStepHorizontal));
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
            if(!isReleaseElementAnnihilation)
            currentHp = 11;       
            
            effectControl.UpdateBlood(currentHp, maxHp);
            Debug.Log("检测到BOSS血量小于11，切换为大地形态");
            ChangeState(E_ElementGodState.EarthForm);
        }
    }

    protected override void OnAtkSpecial(MonsterOnAtk evt)
    {
        base.OnAtkSpecial(evt);
        evt.isCancelNormalAtk = true;
        
        switch (nowState)
        {
            case E_ElementGodState.FireFrom:

                Debug.Log($"BOSS攻击的元素位置{evt.nowPos.x}{evt.nowPos.y}Boss处于的位置{currentPos.x}{currentPos.y}");
                if (currentPos.x - 1 <= 2 && evt.isImprison == false)//如果在左边三列，攻击这一排的所有防御塔，并对玩家造成攻击
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
                else//如果不在左边三列，怪物移动可能会被阻挡，如果是防御塔就会对防御塔发动攻击
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
                //if (evt.isMonster)
                //    return;
                
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
                                //GridMgr.Instance.cellDic[posWaterForm].myUIControl.EnterHighLight();//这是怪物攻击防御塔后的高亮提示，还没确定要不要摧毁防御塔，虽然注释，但是别删
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
                if(evt.isHorizontalMove)
                {
                    evt.isCancelAtk = true;
                    combat.AttackTarget(null);
                }             
                break;

            case E_ElementGodState.WaterForm:
                evt.isCoundDestoryDef = couldDestroyDefAndAhead;
                // 水形态可以直接摧毁防御塔前进
                if (evt.isHorizontalMove)
                {               
                    evt.isCancelAtk = true;
                    combat.AttackTarget(null);
                }
               
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


    /// <summary>
    /// 进入火焰形态逻辑
    /// </summary>
    private void OnEnterFireForm()
    {
        //设置火形态值
        monsterData = Resources.Load<BaseMonsterScriptableData>("BaseMonsterSO/Monster_None01_GodofAllElementalArts_FireForm");
        
        moveInterval = fireFormMoveInterval;
        nowState = E_ElementGodState.FireFrom;
        Debug.Log($"设置boss攻击力{currentAtk}为{fireFormAtk}");

        currentAtk = monsterData.baseAtk;
        couldDestroyDefAndAhead = false;
        baseMoveStepVertical = 0;

        //添加自身固有技能图标
        effectControl.UpdateIconCount(E_BuffIconType.Move, movement.MoveInterval - movement.CurrentRound);
        effectControl.AddBuffIcon(E_BuffIconType.AnnihilationOfElements);
        
    }

    /// <summary>
    /// 进入水形态逻辑
    /// </summary>
    private void OnEnterWaterForm()
    {
        monsterData = Resources.Load<BaseMonsterScriptableData>("BaseMonsterSO/Monster_None01_GodofAllElementalArts_WaterForm");

        //切换当前形态
        nowState = E_ElementGodState.WaterForm;
        //设置攻击力
        currentAtk = monsterData.baseAtk;
        //可以直接摧毁防御塔前进
        couldDestroyDefAndAhead = true;
        //设置垂直移动距离
        baseMoveStepVertical = verticalDistance;
        //更新移动间隔
        moveInterval = waterFormMoveInterval;
        //更换怪物图标特性描述
        atributeIcon.UpdateIconDescription(E_BuffIconType.MonsterDescription_Monster_None01_GodofAllElementalArts_WaterForm);
    }

    /// <summary>
    /// 进入大地形态逻辑
    /// </summary>
    private void OnEnterEarthForm()
    {
        monsterData = Resources.Load<BaseMonsterScriptableData>("BaseMonsterSO/Monster_None01_GodofAllElementalArts_EarthForm");

        //更新移动间隔
        moveInterval = earthFormMoveInterval;
        //释放元素湮灭
        ElementAnnihilation();

        atributeIcon.UpdateIconDescription(E_BuffIconType.MonsterDescription_Monster_None01_GodofAllElementalArts_EarthForm);

        nowState = E_ElementGodState.EarthForm;
        currentAtk = monsterData.baseAtk;
        couldDestroyDefAndAhead = false;
        baseMoveStepVertical = 0;
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
        if (isReleaseElementAnnihilation == true)
            return;
        //播放动画
        effectControl.PlayAtkAnimation(E_AttackAnimType.Boss_God_EarthFormAtk);
        //删去技能图标
        effectControl.RemoveBuffIcon(E_BuffIconType.AnnihilationOfElements);
        Debug.Log("释放元素湮灭");
        //对玩家造成伤害
        GamePlayer.Instance.Hurt(ElementAnnihilationAtk, true);
        //清空玩家手牌
        Dealer.Instance.RemoveAllCards();

        isReleaseElementAnnihilation = true;

    }

    /// <summary>
    /// 延迟一帧执行怪物生成（避免状态机尚未进入生成阶段导致初始化异常）
    /// </summary>
    private IEnumerator DelayedSpawn(System.Action spawnAction)
    {
        yield return null; // 等待一帧
        spawnAction?.Invoke();
    }

    // 修改 OnRoundSpecial 方法中的调用部分
    protected override void OnRoundSpecial(MonsterOnRound evt)
    {
        base.OnRoundSpecial(evt);
        nowDef = 0;
        switch (nowState)
        {
            case E_ElementGodState.FireFrom:
                //// 改为延迟执行
                StartCoroutine(DelayedSpawn(() => SpawnFireMonsters(fireFormMonsterCount, fireFormEliteMonsterCount)));
                break;
            case E_ElementGodState.WaterForm:
                StartCoroutine(DelayedSpawn(() => SpawnWaterMonsters(waterFormMonsterCount, waterFormEliteMonsterCount)));
                break;
            case E_ElementGodState.EarthForm:
                StartCoroutine(DelayedSpawn(() => SpawnEarthMonsters(earthFormMonsterCount, earthFormEliteMonsterCount)));

                // 更新位移
                effectControl.UpdateIconCount(E_BuffIconType.Move, movement.MoveInterval - movement.CurrentRound);
                // 增加生命
                currentHp += addHealValue;
                effectControl.UpdateBlood(currentHp, maxHp);
                if (currentHp > 11)
                    ChangeState(E_ElementGodState.WaterForm);
                break;
        }
    }

    private void SpawnFireMonsters(int basicCount,int elitCount)
    {
        LevelStepMgr.Instance.monsterAliveCount += MonsterCreater.Instance.CreateMonster(DataCenter.Instance.monsterResNameData.GetRandomFireBasicMonsterName(), basicCount);
        LevelStepMgr.Instance.monsterAliveCount += MonsterCreater.Instance.CreateMonster(DataCenter.Instance.monsterResNameData.GetFireEliteMonsterName(), elitCount);
    }

    private void SpawnWaterMonsters(int basicCount,int elitCount)
    {
        LevelStepMgr.Instance.monsterAliveCount += MonsterCreater.Instance.CreateMonster(DataCenter.Instance.monsterResNameData.GetRandomWaterBasicMonsterName(), basicCount);
        LevelStepMgr.Instance.monsterAliveCount += MonsterCreater.Instance.CreateMonster(DataCenter.Instance.monsterResNameData.GetWaterEliteMonsterName(), elitCount);
    }
    private void SpawnEarthMonsters(int basicCount,int elitCount)
    {
        LevelStepMgr.Instance.monsterAliveCount += MonsterCreater.Instance.CreateMonster(DataCenter.Instance.monsterResNameData.GetRandomEarthBasicMonsterName(), basicCount);
        LevelStepMgr.Instance.monsterAliveCount += MonsterCreater.Instance.CreateMonster(DataCenter.Instance.monsterResNameData.GetEarthEliteMonsterName(), elitCount);
    }
}