using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 打牌面板
/// </summary>
public class CardPlayingPanel : BasePanel
{
    //实例化主卡牌槽的原始位置（基础牌+组合牌）
    public RectTransform originMainPos;
    //实例化副卡牌槽的原始位置（部首)
    public RectTransform originMinorPos;
    //控制面板置灰的组件
    public CanvasGroup canvasGroup;
    //格子布局更新事件回调
    public GridLayoutCallback mainCallBack;
    public GridLayoutCallback minorCallBack;

    protected override void Awake()
    {
        base.Awake();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
        mainCallBack = this.GetComponentInChildren<GridLayoutCallback>();
        if (mainCallBack == null)
            Debug.LogError("originMainPos对象没有挂载GridLayoutCallback组件");

    }
    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch (name)
        {
            case "btnOverMyTurn":
                HandleOverMyTurn();
                break;
        }
    }

    private void HandleOverMyTurn()
    {
        Debug.Log("按钮点击结束回合");
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_EnterSettle);
    }

    /// <summary>
    /// 面板进入置灰状态，只能看见不能被点击
    /// </summary>
    public void EnterAsh()
    {
        canvasGroup.blocksRaycasts = false;           
    }

    /// <summary>
    /// 退出置灰状态
    /// </summary>
    public void ExitAsh()
    {
        canvasGroup.blocksRaycasts = true;
    }
}
