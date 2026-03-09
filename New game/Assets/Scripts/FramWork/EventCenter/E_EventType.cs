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
   MonsterHurt
}
