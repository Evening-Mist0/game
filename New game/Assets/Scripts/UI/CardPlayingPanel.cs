using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 打牌面板
/// </summary>
public class CardPlayingPanel : BasePanel
{
    //实例化主卡牌槽的原始位置（基础牌+组合牌）
    public RectTransform originMainPos;
    //实例化副卡牌槽的原始位置（部首)
    public RectTransform originMinorPos;

    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch(name)
        {
            case "btnOverMyTurn":               
                HandleOverMyTurn();
                break;
        }
    }

    private void HandleOverMyTurn()
    {
        Debug.Log("按钮点击结束回合");
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_Move);
    }
}
