using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

/// <summary>
/// 怪物被那种伤害受伤
/// </summary>
public enum E_AtkType
{
    BurnSkill,
    CardAtk,
    DefAtk,
}
/// <summary>
/// 怪物战斗组件，处理受伤、攻击、死亡逻辑，属于怪物核心模块
/// </summary>
public class MonsterCombat : MonoBehaviour
{
    private BaseMonsterCore owner;
    private MonsterEffectControl effectControl;
    private GridMgr gridMgr;
    private MonsterCreater creater;
    private LevelStepMgr levelStepMgr;

    public void Init(BaseMonsterCore monster, MonsterEffectControl effect)
    {
        owner = monster;
        effectControl = effect;
        gridMgr = GridMgr.Instance;
        creater = MonsterCreater.Instance;
        levelStepMgr = LevelStepMgr.Instance;
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(int atk, E_Element cardElement,E_AtkType atkType,bool isTrueDamage)
    { 
        if (!owner.IsAlive) return;

        // 触发受伤事件，可修改最终伤害
        MonsterOnHurt evt = new MonsterOnHurt();
        evt.resultAtk = atk;
        evt.cardElement = cardElement;
        evt.atkType = atkType;
        evt.isTrueDamage = isTrueDamage;


        owner.TriggerOnHurt(evt);

        int totalDamage = evt.resultAtk;


        if (owner.nowDef >= totalDamage)
        {
            // 护盾足够，完全抵消本次伤害
            owner.nowDef -= totalDamage;
            evt.resultAtk = 0;
        }
        else
        {
            // 护盾破碎，剩余伤害生效
            evt.resultAtk = totalDamage - owner.nowDef;
            owner.nowDef = 0;
        }





        // 扣除生命值
        owner.currentHp -= evt.resultAtk;
        effectControl.UpdateBlood(owner.currentHp, owner.maxHp);
        effectControl.UpdateDef(owner.nowDef);
        effectControl.ShowDamegeText(evt.resultAtk);
        Debug.Log($"{owner.monsterName} 受到 {evt.resultAtk} 点伤害，剩余血量 {owner.currentHp}");

        if (owner.currentHp <= 0)
        {
            Die();
            return;
        }

        // 触发血量过低事件
        owner.TriggerOnHpLow(new MonsterOnHpLow());
    }

    /// <summary>
    /// 攻击指定目标
    /// </summary>
    public void AttackTarget(BaseGameObject target)
    {
        MonsterOnAtk evt = new MonsterOnAtk();
        evt.nowPos = owner.currentPos;
        if (target != null && target.gameObjectType == E_GameObjectType.Monster)
            evt.isMonster = true;

        owner.TriggerOnAtk(evt);

        if (target == null) return;

        if (!evt.isElementGodAtk)
        {
            switch (target.gameObjectType)
            {
                case E_GameObjectType.Player:
                    GamePlayer.Instance.Hurt(owner.currentAtk);
                    Debug.Log($"{owner.monsterName} 攻击玩家，造成 {owner.currentAtk} 点伤害");
                    break;
                case E_GameObjectType.DefTower:
                    var tower = target as BaseDefTower;
                    Debug.Log($"{owner.monsterName} 攻击防御塔{tower.name}，造成 {owner.currentAtk} 点伤害");
                    tower?.Hurt(owner);
                    break;
                case E_GameObjectType.Monster:
                    // 怪物攻击怪物，暂未实现
                    Debug.Log($"{owner.monsterName} 攻击友方单位，暂未实现");
                    break;
            }
        }
    }

    /// <summary>
    /// 怪物死亡
    /// </summary>
    public void Die()
    {
        Debug.Log($"{owner.monsterName} 死亡");
        owner.TriggerOnDead(new MonsterOnDead());

        // 从对象池管理器中释放
        creater.ReleaseMonsterCell(owner);

        // 清空当前格子占用
        if (gridMgr.cellDic.ContainsKey(owner.currentPos))
        {
            gridMgr.cellDic[owner.currentPos].UpdateOccupiedState(CellStateType.None, null);
        }

        // 更新关卡怪物存活数量
        levelStepMgr.UpdatMonsterAliveCount();

        // 销毁对象
        Destroy(owner.gameObject);
    }

    /// <summary>
    /// 受到治疗效果
    /// </summary>
    public void GetHeal(int healValue)
    {
        Debug.Log($"[治疗效果]怪物{this.name}受到治疗，当前血量{owner.currentHp}");
        owner.currentHp += healValue;

        // 血量不能超过最大值
        if (owner.currentHp > owner.maxHp)
            owner.currentHp = owner.maxHp;

        owner.effectControl.UpdateBlood(owner.currentHp, owner.maxHp);
        Debug.Log($"[治疗效果]怪物{this.name}治疗完成，当前血量{owner.currentHp}");
    }
}