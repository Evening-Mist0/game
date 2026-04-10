using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    public Button backpackBtn;


    private void Awake()
    {
        
    }
    void Start()
    {
        Debug.Log("[Test]执行了一次Test");

        backpackBtn.onClick.RemoveAllListeners();
        backpackBtn.onClick.AddListener(OnBackpackClick);

        //LevelFlowMgr.Instance.ClearAllData();
        //GrowthMgr.Instance.ResetGrowthData();
        //// 重新初始化爬塔面板
        //UIMgr.Instance.GetPanel<TowerPanel>()?.ClearTowerPanel();

        //UIMgr.Instance.ShowPanel<TowerPanel>(E_UILayerType.middle);
        //// 2. 初始化游戏流程
        //LevelFlowMgr.Instance.InitNewGame();

    }

    public void SimulateNormalBattleWin()
    {
        BattleMgr.Instance.SimulateBattleWin();
    }



    private void OnBackpackClick()
    {
        UIMgr.Instance.ShowPanel<BackpackPanel>(E_UILayerType.top);
    }

}
