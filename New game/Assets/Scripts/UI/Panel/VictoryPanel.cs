using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryPanel : BasePanel
{
    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch (name)
        {
            case "btnSure":
                HandleSure();
                break;

        }
    }

    private void HandleSure()
    {
        //GrowthMgr.Instance.AddRelic("Flint");
        //GrowthMgr.Instance.AddRelic("Cobblestone");
        //GrowthMgr.Instance.AddRelic("Shell");
        //GrowthMgr.Instance.AddRelic("ClosedBook");
        //GrowthMgr.Instance.AddRelic("Inkstone");
        //GrowthMgr.Instance.AddRelic("Paperweight");
        //GrowthMgr.Instance.AddRelic("PenEdge");
        //GrowthMgr.Instance.AddRelic("MagicBrush");

        //GrowthMgr.Instance.AddBook(E_BookType.Fire_LiaoYuan);
        //GrowthMgr.Instance.AddBook(E_BookType.Earth_HouTu);
        ////GrowthMgr.Instance.AddBook(E_BookType.Water_BaiChuan);
        ////GrowthMgr.Instance.AddBook(E_BookType.Battle_PoWang);
        ////GrowthMgr.Instance.AddBook(E_BookType.Wood_KuRong);

        //GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.HpMaxAdd);   
        //GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.DrawCardSpeedUp);   
        //GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.HandCardMaxAdd);   
        //GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.InitArmor);



        UIMgr.Instance.HidePanel<VictoryPanel>();

      

        //切换音乐
        AudioMgr.Instance.PlayBGM("爬塔面板_青阶缓行");
        //关卡回归初始
        LevelStepMgr.Instance.ResetMe();
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.Idle);

            SceneMgr.Instance.LoadSceneAsync("ClimbingTowerScene", () => {
                BattleMgr.Instance.SimulateBattleWin();
                //清理对象池
                PoolMgr.Instance.Clear();
            });
      
    }
}
