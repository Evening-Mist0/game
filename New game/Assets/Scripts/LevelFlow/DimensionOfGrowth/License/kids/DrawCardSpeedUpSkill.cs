using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawCardSpeedUpSkill : BasePlayerSkill
{
    /// 额外的抽牌数（基础牌）
    private int extraCardCount = 1;

    public override E_LevelUpOptionType SkillType => E_LevelUpOptionType.DrawCardSpeedUp;

    /// <summary>
    /// 每回合抽牌数（基础牌）+2
    /// </summary>
    /// <exception cref="System.NotImplementedException"></exception>
    public override void OnGetSkill()
    {
        Dealer.Instance.extraCardCount += extraCardCount;
    }

    public override void OnSetClear()
    {
        Dealer.Instance.extraCardCount -= extraCardCount;

    }
}
