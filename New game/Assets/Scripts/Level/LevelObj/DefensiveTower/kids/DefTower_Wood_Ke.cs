using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DefTower_Wood_Ke : BaseDefTower
{
    public override E_GameObjectType gameObjectType => E_GameObjectType.DefTower;

    [Tooltip("额外血量(每增加一个柯加的血量)")]
    public int extraHp;
    //没有触发连锁时的基础血量
    private int basicHp;
    //被攻击时，自己作为主体受到伤害
    private bool isHurtMe;
    //放置提升血量上限时，自己是否是放置的对象
    private bool isPutMe;



    protected override void Awake()
    {
        base.Awake();
        TypeSafeEventCenter.Instance.Register<OnPlaceDefTower_Ke>(this, OnDefTower_Wood_KePlace);
        TypeSafeEventCenter.Instance.Register<OnDestoryDefTower_Ke>(this, OnDefTower_Wood_KeDestory);
        TypeSafeEventCenter.Instance.Register<OnAtkDefTower_Ke>(this, OnDefTower_Wood_KeHurt);
        basicHp = maxHP;
    }
    public override void OnPlace()
    {
        base.OnPlace();
        List<Cell> cells = GridMgr.Instance.GetColumnCells(myCell.logicalPos.x);
        int sameDefTowerCount = 0;
        for (int i = 0; i < cells.Count; i++)
        {
            if (cells[i].nowStateType == CellStateType.EntityOccupied)
            {
                BaseDefTower tower = cells[i].nowObj as BaseDefTower;
                if(tower.myTowerName == myTowerName)
                {
                    sameDefTowerCount++;
                }
            }
        }

        sameDefTowerCount -= 1; //自己不算在内

        Debug.Log("[柯] 当前列柯放置数量: " + sameDefTowerCount);
        OnPlaceDefTower_Ke evt = new OnPlaceDefTower_Ke();
        evt.currentColumn = myCell.logicalPos.x;
        evt.currentColumnCounts = sameDefTowerCount;
        isPutMe = true;
        TypeSafeEventCenter.Instance.Trigger<OnPlaceDefTower_Ke>(evt);
    }

    public override void OnDestory(OnDefTowerDestoryByMonsterEvents evt)
    {
        base.OnDestory(evt);
        OnDestoryDefTower_Ke evt2 = new OnDestoryDefTower_Ke();
        evt2.currentColumn = myCell.logicalPos.x;
        evt2.monster = evt.monster;
        TypeSafeEventCenter.Instance.Trigger<OnDestoryDefTower_Ke>(evt2);
    }

    public override void OnHurt(OnDefTowerHurtByMonsterEvents evt)
    {
        base.OnHurt(evt);
        OnAtkDefTower_Ke evt2 = new OnAtkDefTower_Ke();
        evt2.currentColumn = myCell.logicalPos.x;
        evt2.monster = evt.monster;
        isHurtMe = true;
        TypeSafeEventCenter.Instance.Trigger<OnAtkDefTower_Ke>(evt2);

    }

    /// <summary>
    /// 当放置了坷建筑后，更新其他柯的血量
    /// </summary>
    /// <param name="count">当前列柯存在的数量</param>
    private void OnDefTower_Wood_KePlace(OnPlaceDefTower_Ke evt)
    {
        if (evt.currentColumnCounts <= 0 || evt.currentColumn != myCell.logicalPos.x)
            return;

        Debug.Log("触发放置事件");
        maxHP = basicHp + extraHp * evt.currentColumnCounts;

        if (isPutMe)//当前放置物和其他放置物的增加血量逻辑不一样
            currentHP += extraHp * evt.currentColumnCounts;
        else
            currentHP += extraHp;

        isPutMe = false;

        effectControl.UpdateBlood(currentHP, maxHP);
    }

    private void OnDefTower_Wood_KeHurt(OnAtkDefTower_Ke evt)
    {
        if (evt.currentColumn != myCell.logicalPos.x || isHurtMe)
        {
            isHurtMe = false;
            return;
        }
 
        currentHP -= evt.monster.currentAtk;
        //更新血条
        effectControl.ShowDamageText(evt.monster.currentAtk, this.transform.position);
        effectControl.UpdateBlood(currentHP, maxHP);
        if (currentHP <= 0)
            DestroyMe();                
    }

    private void OnDefTower_Wood_KeDestory(OnDestoryDefTower_Ke evt)
    {
        if (evt.currentColumn != myCell.logicalPos.x)
            return;


        maxHP -= extraHp;
        currentHP -= extraHp;

        if (currentHP > 0)
        {
            currentHP = maxHP;
            effectControl.UpdateBlood(currentHP, maxHP);
        }
    }
}
