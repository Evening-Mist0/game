using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inkstone : I_Treasure
{
    //奖励总次数
    public int rewardCount = 3;
    //奖励当前次数
    public int currentRewardCount;

    public int weight = 3;


    public void OnCreateDefTower(BasePlaceCard card)
    {

    }



    public void OnDrawCard(BaseCard card)
    {

    }

    public void OnPlay(BaseCard card)
    {

    }

    public void OnSynthesis(BaseCard card)
    {
        if(card.isRareCard)
        {
            Debug.Log($"[墨砚]检测到{card.cardID}为稀有牌");

            CardPlayingPanel panel = UIMgr.Instance.GetPanel<CardPlayingPanel>();

            if (panel != null)
            {
                if(currentRewardCount < rewardCount)
                {
                    panel.DropRandomRadicalCard(card.transform.position);
                    currentRewardCount++;
                    Debug.Log("[墨砚]检测到卡牌为稀有牌，随机奖励部首牌一张,剩余奖励次数" + (rewardCount - currentRewardCount));

                }
            }
            else
            {
                Debug.LogError("[墨砚]此时无法获取到面板，无法生成基础牌");
            }
        } 
    }

    public void ResetOnClickOverTurn()
    {

    }

    public void ResetOnLevelOver()
    {
        currentRewardCount = 0;
    }
}
