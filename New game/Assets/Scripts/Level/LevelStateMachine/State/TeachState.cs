using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeachState : BaseLevelState
{
    //记录进入教学状态的次数，每个次数对应不用教学状态
    [HideInInspector]
    private int enterCount;


    private bool isTeaching;

    public override E_LevelState myStateType => E_LevelState.PlayerTurn_Teach;

    public override void EnterState()
    {
        enterCount++;
        isTeaching = true;
        Debug.Log("进入教学状态，当前进入的次数为" + enterCount);
        EnterTeach();

        Debug.Log("进入DrawCardState状态");
        if (isTeaching)
        {
            Debug.Log("进入DrawCardSate,补充基础卡牌");
            bool isfirst = LevelStepMgr.Instance.currentWave == 1 ? true : false;
            Dealer.Instance.DealBasicCardsOnTeach(isfirst);
        }
    }

    public override void ExitState()
    {
        isTeaching = false;
    }

    public override void OnState()
    {
        if(isTeaching)
            LevelStepMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_CardOperate);
    }

    /// <summary>
    /// 重置进入状态计数，用于反复进入进入教学关卡的情况，应当在胜利教学面板调用一次改方法
    /// </summary>
    public void ResetEnterCount()
    {
        enterCount = 0;
    }

    /// <summary>
    /// 第一次进入，讲解左键可以拖曳打出卡牌，点击结束按钮结束回合
    /// </summary>
    private void EnterOne()
    {
        UIMgr.Instance.ShowPanel<TeachPanelStep1>();
    }

    /// <summary>
    /// 第二次进入，讲解卡牌可以合成打出
    /// </summary>
    private void EnterTow()
    {
        //关闭教学面板
        UIMgr.Instance.HidePanel<TeachPanelStep1>();
        UIMgr.Instance.ShowPanel<TeachPanelStep2>();

    }

    /// <summary>
    /// 第三次进入，讲解可以选取兑换的卡牌，可以用部首牌跟卡牌合成
    /// </summary>
    private void EnterThree()
    {
        UIMgr.Instance.HidePanel<TeachPanelStep3>();
        UIMgr.Instance.ShowPanel<TeachPanelStep4>();

    }

    /// <summary>
    /// 第四次进入，讲解奇物效果，并解释连击数的作用
    /// </summary>
    private void EnterFour()
    {

        UIMgr.Instance.HidePanel<TeachPanelStep4>();
        UIMgr.Instance.ShowPanel<TeachPanelStep5>();

    }

    private void EnterFive()
    {
        UIMgr.Instance.HidePanel<TeachPanelStep5>();
        UIMgr.Instance.ShowPanel<TeachPanelStep6>();

    }

    private void EnterSix()
    {
        UIMgr.Instance.HidePanel<TeachPanelStep6>();
        UIMgr.Instance.ShowPanel<TeachPanelStep7>();

    }

    private void EnterSeven()
    {
        UIMgr.Instance.HidePanel<TeachPanelStep7>();
        UIMgr.Instance.ShowPanel<TeachPanelStep8>();
    }

    private void EnterEight()
    {
        UIMgr.Instance.HidePanel<TeachPanelStep8>();

    }

    public void HideAllPanel()
    {
        UIMgr.Instance.HidePanel<TeachPanelStep1>();
        UIMgr.Instance.HidePanel<TeachPanelStep2>();
        UIMgr.Instance.HidePanel<TeachPanelStep3>();
        UIMgr.Instance.HidePanel<TeachPanelStep4>();
        UIMgr.Instance.HidePanel<TeachPanelStep5>();
        UIMgr.Instance.HidePanel<TeachPanelStep6>();
        UIMgr.Instance.HidePanel<TeachPanelStep7>();
        UIMgr.Instance.HidePanel<TeachPanelStep8>();
    }
    /// <summary>
    /// 根据enterCount进入哪一次的教学关卡
    /// </summary>
    public void EnterTeach()
    {
        switch (enterCount)
        {
            case 1:
                EnterOne();
                break;
            case 2:
                EnterTow();
                break;
            case 3:
                EnterThree();
                break;
            case 4:
                EnterFour();
                break;
            case 5:
                EnterFive();
                break;
            case 6:
                EnterSix();
                break;
            case 7:
                EnterSeven();
                break;
            case 8:
                EnterEight();
                break;
            default:
                break;

        }
    }

  

}
