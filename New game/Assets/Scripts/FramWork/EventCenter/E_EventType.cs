using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_EventType 
{
   /// <summary>
   /// 加载进度
   /// </summary>
   loadProgrees,

   /// <summary>
   /// 卡牌打出后怪物受伤，在BaseCard中注册，预计在一个碰撞体判断类发生（鼠标点击格子会格局生成规则生成一个规则范围碰撞体，用这个碰撞体去检测怪物）
   /// </summary>
   MonsterHurt,

    /// <summary>
    /// -左键点击-卡牌时发生的事件，在DrawLineMgr中注册，在PaperBounceControl中发生（用于DrawLineMgr的画线起始点）
    /// </summary>
    OnCardClick0_Vector3,
    /// <summary>
    /// -右键点击-卡牌时发生的事件，在DrawLineMgr中注册，在PaperBounceControl中发生（用于DrawLineMgr的取消画线）
    /// </summary>
    OnCardClick1,



    /// <summary>
    /// -左键点击-卡牌时发生的事件，在CardOperateState中注册，在PaperBounceControl中发生(用于更新选中的要打出的卡牌)
    /// </summary>
    OnCardClick0_BaseCard,
    /// <summary>
    /// -右键键点击- 后 再点击右键时发生的事件，在CardOperateState中注册，在PaperBounceControl中发生(用于右键选中卡牌的移除)
    /// </summary>
    OnCardClick1_BaseCard,


    /// <summary>
    /// -左键点击-卡牌时发生的事件，在BaseCard中注册，在PaperBounceControl中发生(用于BaseCard的更新是否选中状态)
    /// </summary>
    OnCardClick0_Bool, 
    /// <summary>
    /// -右键键点击- 后 再点击右键时发生的事件，在BaseCard中注册，在PaperBounceControl中发生(更新选中的牌为合成牌)
    /// </summary>
    OnCardClick1_Bool,
    /// <summary>
    /// 取消卡牌选择，在BaseCard，DrawLineMgr中注册，在PaperBounceControl中发生(取消选中的合成牌，取消画线)
    /// </summary>
    CancelSelected,


    /// <summary>
    /// 玩家通过鼠标右键选择到第一张卡后，没有点击第二张卡的情况。在PaperBounceControl里面注册，在CardOperateStatef发生
    /// </summary>
    //OnCardOperateCancelCard1,

    //峰老师留下的锅，没有把冲突解决干净，临时补一个
    OnLayerChanged,

    // ========== 新增：爬塔模块事件 ==========
    /// <summary> 爬塔初始化完成 </summary>
    Tower_InitComplete,
    /// <summary> 楼层切换事件 参数：当前楼层数 </summary>
    Tower_LayerChanged,
    /// <summary> 节点状态变更事件 参数：节点ID </summary>
    Tower_NodeStateChanged,
    /// <summary> 进入节点事件 参数：节点数据 </summary>
    Tower_EnterNode,
    /// <summary> 节点战斗胜利事件 </summary>
    Tower_NodeBattleWin,
    /// <summary> 爬塔失败事件 </summary>
    Tower_Failed,
    /// <summary> 爬塔通关事件 </summary>
    Tower_Complete,
    /// <summary> 爬塔节点生成事件 </summary>
    Tower_Bron,

    // ========== 新增：成长模块事件 ==========
    /// <summary> 执照经验变更事件 参数：当前经验值 </summary>
    Growth_LicenseExpChanged,
    /// <summary> 执照等级升级事件 参数：当前等级 </summary>
    Growth_LicenseLevelUp,
    /// <summary> 获得典籍事件 参数：典籍ID </summary>
    Growth_AddBook,
    /// <summary> 获得奇物事件 参数：奇物ID </summary>
    Growth_AddRelic,
    /// <summary> 玩家血量变更事件 参数：当前血量/最大血量 </summary>
    Growth_PlayerHpChanged,

    // 战斗相关事件
    Battle_LoadNormalBattle,
    Battle_LoadEliteBattle,
    Battle_LoadBossBattle,
    Battle_NormalBattleWin,
    Battle_EliteBattleWin,
    Battle_BossBattleWin,
    Battle_BossBattleFail,
    Battle_BattleLose,

    // 成长获得事件
    Growth_GetRelic,
    Growth_GetBook,

    // 营地相关事件
    Camp_OptionConfirm,
    Camp_SetWuDaoInteractable,
    Camp_BookSelectConfirm,

    // 事件面板相关事件
    Event_InitPanel,
    Event_OptionConfirm,

    // UI面板相关事件
    UI_ShowRelicSelectPanel,
    UI_ShowBookSelectPanel,

    /// <summary> 楼层完成事件 参数：楼层索引 </summary>
    Tower_LayerComplete,
    /// <summary> 启程按钮状态变更事件 参数：是否可点击 </summary>
    UI_DepartBtnStateChanged,
    UI_BookSelectConfirm,


}
