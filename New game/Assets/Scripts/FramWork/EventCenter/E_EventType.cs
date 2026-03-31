using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_EventType 
{
    /// <summary>
    /// 怪物死亡
    /// </summary>
   monster_Dead,

   /// <summary>
   /// 加载进度
   /// </summary>
   loadProgrees,

   /// <summary>
   /// 层数变更
   /// </summary>
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
    Battle_NormalBattleWin,   // 改为带参数：string nodeId
    Battle_EliteBattleWin,    // 带参数：string nodeId
    Battle_BossBattleWin,     // 带参数：string nodeId
    Battle_BossBattleFail,

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

        // ========== 新增必需事件 ==========
    /// <summary> 楼层完成事件 参数：楼层索引 </summary>
    Tower_LayerComplete,
    /// <summary> 启程按钮状态变更事件 参数：是否可点击 </summary>
    UI_DepartBtnStateChanged,
    UI_BookSelectConfirm,

}
