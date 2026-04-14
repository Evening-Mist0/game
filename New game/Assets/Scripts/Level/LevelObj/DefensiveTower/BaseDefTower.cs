using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/// <summary>
/// 防御塔的类型
/// </summary>
public enum E_TowerType
{
    /// <summary>
    /// 实体，可以阻挡怪物
    /// </summary>
    Entity,
    /// <summary>
    /// 幽灵，怪物可以穿过
    /// </summary>
    Ghost,
}

public enum E_TowerName
{
    DefTower_Earth_Di,
    DefTower_Earth_Ke,
    DefTower_Earth_Yao,
    DefTower_Earth_Yi,
    DefTower_Water_Chi,
    DefTower_Wood_Ke

}
public abstract class BaseDefTower : BaseGameObject
{
    [Header("防御塔基础配置")]
    [Tooltip("防御塔血量")]
    public int maxHP;
    [Tooltip("防御塔类型")]
    public E_TowerType myTowerType;
    [Tooltip("防御塔名称")]
    public E_TowerName myTowerName;

    [HideInInspector]
    public DefTowerEffectControl effectControl;

    //防御塔是否被摧毁
    private bool isDestory;

    [HideInInspector]
    /// <summary>
    /// 当前血量
    /// </summary>
    
    public int currentHP;

    public int nowDef;


    /// <summary>
    /// 自身处于哪个单元格
    /// </summary>
    public Cell myCell;

    protected virtual void Awake()
    {
        InitControl();
        InitValue();
        TypeSafeEventCenter.Instance.Register<OnExitMonsterMoveStateEvent>(this,OnRound);

    }

    private void Start()
    {
        if(myTowerType == E_TowerType.Entity)
        {
            effectControl.UpdateBlood(currentHP, maxHP);
            effectControl.UpdateDef(nowDef);
        }
       
    }

    protected virtual void InitValue()
    {
        currentHP = maxHP;
    }



    protected virtual void InitControl()
    {
        effectControl = this.GetComponent<DefTowerEffectControl>();
        if (effectControl == null)
            Debug.LogError("没有挂载DefTowerEffectControl组件");
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    /// <param name="value">被哪个怪物伤害伤害</param>
    public void Hurt(BaseMonsterCore monster,bool isTrueDamage = false)
    {
        if (myTowerType == E_TowerType.Ghost)
            return;

        int damage = monster.currentAtk;
        Debug.Log("防御塔受到伤害" + damage);
        if (isTrueDamage)
        {
            currentHP -= damage;
            Debug.Log("防御塔受到受到真伤");
        }
        else
        {
            //护甲抵挡
            int overDamage = damage - nowDef;
            Debug.Log($"防御塔受伤：伤害值{damage}-护甲值{nowDef}");
            if (overDamage <= 0)
            {
                // 护甲足够，完全抵挡
                nowDef -= damage;
                Debug.Log("[防御塔受伤] 护甲完全抵挡伤害，剩余护甲：" + nowDef);
            }
            else
            {
                // 护甲被击穿，剩余伤害扣血
                nowDef = 0;
                currentHP -= overDamage;
                Debug.Log("[玩家受伤] 护甲被击穿，实际受到伤害：" + overDamage);
            }
        }

        //更新护甲/血条
        effectControl.ShowDamageText(monster.currentAtk, this.transform.position);
        effectControl.UpdateBlood(currentHP, maxHP);
        effectControl.UpdateDef(nowDef);

        OnDefTowerHurtByMonsterEvents evt = new OnDefTowerHurtByMonsterEvents();
        evt.monster = monster;
        OnHurt(evt);


        if (currentHP <= 0)
        {
            Debug.Log("防御塔被摧毁");
            DestroyMe(monster);
        }

    }

    

    /// <summary>
    /// 清理护甲
    /// </summary>
    public void OnRound(OnExitMonsterMoveStateEvent evt)
    {
        nowDef = 0;
        effectControl.UpdateDef(nowDef);
    }



    public virtual void OnHurt(OnDefTowerHurtByMonsterEvents evt)
    {


    }

    public virtual void OnDestory(OnDefTowerDestoryByMonsterEvents evt)
    {
        if (evt.monster == null)
            Debug.LogWarning("本次摧毁该防御塔的对象为空，只有系统层才为空，请注意");
            
    }

    /// <summary>
    /// 当放置防御塔时触发
    /// </summary>
    public virtual void OnPlace()
    {
   
    }



    public void GetDef(int value)
    {
        Debug.Log($"建筑物{this.gameObject.name}获得护甲{value}");
        nowDef += value;
        effectControl.UpdateDef(nowDef);
    }

    /// <summary>
    /// 受到系统层面的伤害（主要是用于清理怪物出生点存在的建筑物）
    /// </summary>
    public void HurtWithSystem(int damage)
    {
        currentHP -= damage;

        effectControl.UpdateBlood(currentHP, maxHP);
        effectControl.UpdateDef(nowDef);
        effectControl.ShowDamageText(damage,this.transform.position);
        if (currentHP <= 0)
            DestroyMe();
    }


    /// <summary>
    /// 销毁自己
    /// </summary>
    /// <param name="obj">被哪个对象触发了销毁</param>
    public void DestroyMe(BaseMonsterCore obj = null)
    {
        
        if (isDestory == true)
            return;

        OnDefTowerDestoryByMonsterEvents evt = new OnDefTowerDestoryByMonsterEvents();
        evt.monster = obj;
        OnDestory(evt);

        isDestory = true;
        switch (myTowerType)
        {
            case E_TowerType.Entity:
                myCell.UpdateOccupiedState(CellStateType.None, null);
                break;

            case E_TowerType.Ghost:
                HandleGhostTowerDestroy();
                break;
            default:
                myCell.UpdateOccupiedState(CellStateType.None, null);
                break;
        }

        //销毁建筑物
        Destroy(this.gameObject);
    }

    /// <summary>
    /// 单独拆分幽灵塔的逻辑（解耦嵌套）
    /// </summary>
    private void HandleGhostTowerDestroy()
    {
        if (myCell.nowObj == null)
        {
            myCell.UpdateOccupiedState(CellStateType.None, null);
            return;
        }

        // 根据格子上的对象类型更新状态
        switch (myCell.nowObj.gameObjectType)
        {
            case E_GameObjectType.Player:
                myCell.UpdateOccupiedState(CellStateType.PlayerOccupied, myCell.nowObj);
                break;

            case E_GameObjectType.Monster:
                myCell.UpdateOccupiedState(CellStateType.MonsterOccupied, myCell.nowObj);
                break;

            case E_GameObjectType.DefTower:
                UpdateTowerCellState(myCell.nowObj);
                break;

            case E_GameObjectType.Cell:
            default:
                myCell.UpdateOccupiedState(CellStateType.None, null);
                break;
        }
    }

    /// <summary>
    /// 拆分防御塔状态更新（彻底消除嵌套）
    /// </summary>
    private void UpdateTowerCellState(BaseGameObject obj)
    {
        BaseDefTower tower = obj as BaseDefTower;
        if (tower == null)
        {
            myCell.UpdateOccupiedState(CellStateType.None, null);
            return;
        }

        switch (tower.myTowerType)
        {
            case E_TowerType.Entity:
                myCell.UpdateOccupiedState(CellStateType.EntityOccupied, obj);
                break;

            case E_TowerType.Ghost:
                myCell.UpdateOccupiedState(CellStateType.GhostOccupied, obj);
                break;

            default:
                myCell.UpdateOccupiedState(CellStateType.None, null);
                break;
        }
    }

    /// <summary>
    /// 设置该防御塔在哪个单元格（重要）
    /// </summary>
    /// <param name="myCell">防御卡处于的单元格</param>
    public void SetMyCell(Cell myCell)
    {
        this.myCell = myCell;
        myCell.UpdateOccupiedState(CellStateType.EntityOccupied, this);
    }


}
