//using UnityEngine;

///// <summary>
///// 怪物战斗组件，处理受击、攻击、死亡等战斗逻辑
///// </summary>
//public class MonsterCombat : MonoBehaviour
//{
//    private BaseMonsterCore owner;
//    private MonsterEffectControl effectControl;
//    private GridMgr gridMgr;
//    private MonsterCreater creater;
//    private LevelStepMgr levelStepMgr;

//    public void Init(BaseMonsterCore monster, MonsterEffectControl effect)
//    {
//        owner = monster;
//        effectControl = effect;
//        gridMgr = GridMgr.Instance;
//        creater = MonsterCreater.Instance;
//        levelStepMgr = LevelStepMgr.Instance;
//    }

//    /// <summary>
//    /// 受到伤害
//    /// </summary>
//    public void TakeDamage(int atk, E_CardSkill skill)
//    {
//        if (!owner.IsAlive) return;

//        // 创建事件，允许子类修改伤害
//        MonsterOnHurt evt = new MonsterOnHurt { atk = atk, isTrueDamage = (skill == E_CardSkill.TrueDamage) };
//        owner.TriggerOnHurt(evt);

//        if (!evt.isTrueDamage)
//        {
//            // 普通伤害，可在此添加减伤逻辑
//        }

//        owner.currentHp -= evt.atk;
//        effectControl.UpdateBlood(owner.currentHp);
//        Debug.Log($"{owner.monsterName} 受到 {evt.atk} 伤害，剩余血量 {owner.currentHp}");

//        if (owner.currentHp <= 0)
//        {
//            Die();
//            return;
//        }

//        // 触发血量低于阈值特性
//        owner.TriggerOnHpLow(new MonsterOnHpLow());
//    }

//    /// <summary>
//    /// 攻击指定目标
//    /// </summary>
//    public void AttackTarget(BaseGameObject target)
//    {
//        if (target == null) return;

//        switch (target.gameObjectType)
//        {
//            case E_GameObjectType.Player:
//                GamePlayer.Instance.Hurt(owner.attack);
//                Debug.Log($"{owner.monsterName} 攻击玩家，造成 {owner.attack} 伤害");
//                break;
//            case E_GameObjectType.DefTower:
//                var tower = target as BaseDefTower;
//                Debug.LogWarning("防御塔攻击怪物，目前使用BaseMonsterCore，暂时取消逻辑");
//                //tower?.Hurt(owner);
//                break;
//            case E_GameObjectType.Monster:
//                // 怪物互攻逻辑（如有需要）
//                Debug.Log($"{owner.monsterName} 攻击其他怪物，未实现");
//                break;
//        }
//    }

//    /// <summary>
//    /// 死亡处理
//    /// </summary>
//    public void Die()
//    {
//        Debug.Log($"{owner.monsterName} 死亡");
//        owner.TriggerOnDead(new MonsterOnDead());

//        // 从怪物创建器移除
//        creater.ReleaseMonsterCell(owner);

//        // 释放当前格子
//        if (gridMgr.cellDic.ContainsKey(owner.currentPos))
//        {
//            gridMgr.cellDic[owner.currentPos].UpdateOccupiedState(CellStateType.None, null);
//        }

//        // 更新存活计数
//        levelStepMgr.UpdatMonsterAliveCount();

//        // 销毁对象
//        Destroy(owner.gameObject);
//    }
//}