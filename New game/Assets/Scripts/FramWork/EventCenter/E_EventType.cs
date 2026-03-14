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

}
