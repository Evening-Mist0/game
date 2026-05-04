using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireMask : BaseTreasure
{
    //火系卡牌计数
    //[Obsolete]
    //private int currentFireCardCount;
    //可以抽到多少张火基础牌
    //[Obsolete]
    //private int fireCardCount = 1;

    //打出的火元素卡牌计数
    private int playFireCardCount = 0;

    //奖励阈值
    private int rewardDotCount = 3;
    

    public int weight = 6;

    public override E_TreasureType type => E_TreasureType.FireMask;

    public override int round => -1;


    //[Obsolete]
    //public override void OnDrawCard(BaseCard card)
    //{
    //    if (card.elementType != E_Element.Fire)
    //    {
    //        if (currentFireCardCount < fireCardCount)
    //        {
    //            Debug.Log($"[火焰面具]将{card.cardID}替换为基础火牌");
    //            Dealer.Instance.RemoveCard(card);
    //            BaseCard newCard = Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_fire_huo, 0);

    //            // 修复：正确查找背包中的 MagicBrush
    //            var magicBrush = GamePlayer.Instance.playerBag.treasures
    //                .OfType<MagicBrush>()
    //                .FirstOrDefault();

    //            magicBrush?.OnDrawCard(newCard);

    //            currentFireCardCount++;
    //        }
    //    }
    //}

    public override void OnPlay(BaseCard card)
    {
        base.OnPlay(card);
        if (card.elementType == E_Element.Fire)
        {
            //增加计数
            Debug.Log($"[火焰面具]检测到火元素牌{card.cardID}计数加一，目前计数为{playFireCardCount}");
            playFireCardCount++;          
            //达到阈值给予卡牌奖励
            if (playFireCardCount == rewardDotCount)
            {
                BaseCard newCard = Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_fire_huo, card.transform.GetSiblingIndex());
                
                playFireCardCount = 0;
            }
            //UI更新
            CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();
            if (panel != null)
                panel.treasuresViewControl.UpdateIconCount(type, playFireCardCount);
        }
    }


    public override void ResetOnLevelOver()
    {
        base.ResetOnLevelOver();
        playFireCardCount = 0;
    }
    

    //public override void ResetOnClickOverTurn()
    //{
    //    Debug.Log($"[火焰面具]清空火卡牌计数");
    //    currentFireCardCount = 0;
    //}
}

