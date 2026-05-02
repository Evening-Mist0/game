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

        //GrowthMgr.Instance.AddRelic("ClosedBook");
        //GrowthMgr.Instance.AddRelic("Inkstone");
        GrowthMgr.Instance.AddRelic("Paperweight");
        //GrowthMgr.Instance.AddRelic("PenEdge");
        //GrowthMgr.Instance.AddRelic("MagicBrush");
        //GrowthMgr.Instance.AddRelic("HuoRong");
        //GrowthMgr.Instance.AddRelic("DropWater");
        //GrowthMgr.Instance.AddRelic("Stone");
        //GrowthMgr.Instance.AddRelic("WoodLeaf");
        //GrowthMgr.Instance.AddRelic("PenEdge");
        //GrowthMgr.Instance.AddRelic("GuiyuanCompass");
        //GrowthMgr.Instance.AddRelic("EchoConch");




        //GrowthMgr.Instance.AddBook(E_BookType.Fire_Fen);
        //GrowthMgr.Instance.AddBook(E_BookType.Fire_Xie);
        //GrowthMgr.Instance.AddBook(E_BookType.Fire_Yi);
        //GrowthMgr.Instance.AddBook(E_BookType.Earth_Yao);
        //GrowthMgr.Instance.AddBook(E_BookType.Earth_Zhuo);
        //GrowthMgr.Instance.AddBook(E_BookType.Water_Lin);
        //GrowthMgr.Instance.AddBook(E_BookType.Water_Miao);
        //GrowthMgr.Instance.AddBook(E_BookType.Wood_Bi);
        //GrowthMgr.Instance.AddBook(E_BookType.Wood_Yi );
        //BaseBook book = GamePlayer.Instance.playerBag.GetBook(E_BookType.Wood_Yi );
        //book.LevelUp(3);

        //BaseBook book = GamePlayer.Instance.playerBag.GetBook(E_BookType.Fire_Xie);
        //book.LevelUp(3);
        //GamePlayer.Instance.playerBag.books[E_BookType.Water_Chi].currentLevel = 2;


        //GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.HpMaxAdd);
        //GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.DrawCardSpeedUp);
        //GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.HandCardMaxAdd);
        //GamePlayer.Instance.playerBag.AddSkill(E_LevelUpOptionType.InitArmor);


        AudioMgr.Instance.PlaySFX("选牌音效");

        UIMgr.Instance.HidePanel<VictoryPanel>();


        EventCenter.Instance.EventTrigger(E_EventType.UI_LevelOver);

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

    public override void ShowMe()
    {
        base.ShowMe();
        //播放胜利音效
        //AudioMgr.Instance.PlaySFX("局内游戏胜利");
    }
}
