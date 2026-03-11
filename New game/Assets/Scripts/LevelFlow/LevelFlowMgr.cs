using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelFlowMgr : BaseMgr<LevelFlowMgr>
{
    private LevelFlowMgr()
    {
        EventCenter.Instance.AddEventListener<int>(E_EventType.OnLayerChanged, OnLayerChanged);
    }

    // 处理Model的层数变更事件（通知View更新）
    private void OnLayerChanged(int newLayer)
    {

    }

   
}
