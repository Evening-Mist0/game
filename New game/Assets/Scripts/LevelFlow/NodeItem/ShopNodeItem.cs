using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopNodeItem : BaseNodeItem
{
    protected override void OnNodeClick()
    {
        base.OnNodeClick();
        // 打开商店面板
        UIMgr.Instance.ShowPanel<ShopPanel>(E_UILayerType.top, (panel) =>
        {
             panel.Init(() =>
             {
                 // 关闭商店面板后完成节点
                 LevelFlowMgr.Instance.CompleteNode(nodeId);
                 UIMgr.Instance.GetPanel<TowerPanel>()?.ShowMe();
             });
        });
    }
}
