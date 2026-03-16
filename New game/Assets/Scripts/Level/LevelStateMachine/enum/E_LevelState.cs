using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡核心状态枚举
/// </summary>
public enum E_LevelState
{
    /// <summary>
    /// 关卡初始化（加载网格/阻挡物/玩家数据/初始手牌，生成第一波怪物）
    /// 仅关卡启动时触发一次
    /// </summary>
    Init,

    /// <summary>
    /// 玩家回合-抽牌阶段（补抽基础牌至上限、更新手牌UI、解锁卡牌操作）
    /// 玩家回合的第一个子阶段
    /// </summary>
    PlayerTurn_DrawCard,

    /// <summary>
    /// 玩家回合-卡牌操作阶段（合成/打出/拾取卡牌，监听卡牌交互事件）
    /// 玩家回合核心交互阶段，可点击结束回合按钮切状态
    /// </summary>
    PlayerTurn_CardOperate,

    /// <summary>
    /// 玩家回合-结束结算（清除部首牌、结算持续效果、锁定卡牌UI、记录回合数据）
    /// 玩家回合最后一个子阶段，无玩家交互，纯逻辑结算
    /// </summary>
    PlayerTurn_EndSettle,

    /// <summary>
    /// 怪物回合-根据变量创建本轮的怪物数量
    /// 每次关卡怪物有固定的数量，只有创建完了才会进行游戏结束判定
    /// </summary>
    MonsterTurn_CreatMonster,

    /// <summary>
    /// 怪物回合-移动阶段（遍历怪物按规则移动、校验碰撞、同步网格/渲染坐标）
    /// 怪物回合第一个子阶段，纯逻辑执行，无玩家交互
    /// </summary>
    MonsterTurn_Move,

    /// <summary>
    /// 怪物回合-攻击阶段（第1列怪物攻击玩家、扣血、播放攻击特效、更新玩家血量UI）
    /// 怪物回合第二个子阶段，纯逻辑执行，无玩家交互
    /// </summary>
    MonsterTurn_Attack,

    /// <summary>
    /// 怪物回合-结束结算（胜负判定、检查波次怪物是否清空、准备下一波生成）
    /// 怪物回合最后一个子阶段，核心触发胜负判定逻辑
    /// </summary>
    MonsterTurn_EndSettle,

    /// <summary>
    /// 关卡胜利（暂停回合循环、播放胜利动画/音效、弹出胜利界面、结算奖励/升级）
    /// 触发后关卡主流程终止，等待玩家界面操作
    /// </summary>
    LevelWin,

    /// <summary>
    /// 关卡失败（停止时间流动、播放失败动画/音效、弹出失败界面）
    /// 触发后关卡主流程终止，等待玩家界面操作
    /// </summary>
    LevelLose,

    /// <summary>
    /// 波次过渡（多波次关卡专用：清空上波残留、生成下一波怪物、解锁玩家回合）
    /// 仅胜利判定为「单波清完但未通关」时触发
    /// </summary>
    WaveTransition,

    /// <summary>
    /// 关卡暂停（暂停所有逻辑更新/动画/音效，如玩家点击暂停按钮、弹出升级界面）
    /// 可恢复至暂停前的原状态，不中断关卡主流程
    /// </summary>
    Pause
}
