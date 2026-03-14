using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelFlowMgr : BaseMonoMgr<LevelFlowMgr>
{
    private void Start()
    {
        //处理Model的层数变更事件
        EventCenter.Instance.AddEventListener<int>(E_EventType.OnLayerChanged, OnLayerChanged);
    }




    // 处理Model的层数变更事件（通知View更新）
    private void OnLayerChanged(int newLayer)
    {

    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<int>(E_EventType.OnLayerChanged, OnLayerChanged);
    }
}
