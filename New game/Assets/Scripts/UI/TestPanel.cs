using System.Collections.Generic;
using UnityEngine;

// 测试脚本：按快捷键打开选择面板，加载测试物品
public class TestPanel : MonoBehaviour
{
    [Header("测试配置（拖拽赋值）")]
    public SelectionPanelManager panelManager; // 选择面板管理器
    public List<ItemData> testItemDatas;       // 测试物品数据（在Inspector添加）

    // 每帧检测输入
    private void Update()
    {
        // 按K键打开面板（可自行修改快捷键）
        if (Input.GetKeyDown(KeyCode.K))
        {
            // 校验：面板管理器和测试数据不为空
            if (panelManager == null)
            {
                Debug.LogError("测试脚本未绑定面板管理器！");
                return;
            }
            if (testItemDatas == null || testItemDatas.Count == 0)
            {
                Debug.LogWarning("测试物品数据为空！请在Inspector添加物品");
                return;
            }

            // 打开面板并加载测试物品
            panelManager.ShowPanel(testItemDatas);
        }
    }
}
