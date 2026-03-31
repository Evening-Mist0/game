using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerTest : MonoBehaviour
{
    private void Start()
    {
        // 1. 验证单例是否存在
        if (TowerNodeMgr.Instance == null)
        {
            Debug.LogError("TowerNodeMgr单例初始化失败！");
            return;
        }

        // 3. 显示爬塔面板（面板会监听初始化完成事件生成节点）
        UIMgr.Instance.ShowPanel<TowerPanel>(E_UILayerType.middle);

        // 2. 初始化爬塔数据（核心，必须先调用）
        TowerNodeMgr.Instance.InitNewTowerData();



        Debug.Log("爬塔初始化流程执行完成"); // 新增日志
    }


}
