using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FireMask : BaseTreasure
{
    //火系卡牌计数
    private int currentFireCardCount;
    //可以抽到多少张火基础牌
    private int fireCardCount = 2;

    public int weight = 6;

 

    public override void OnDrawCard(BaseCard card)
    {
        if (card.elementType != E_Element.Fire)
        {
            if (currentFireCardCount < fireCardCount)
            {
                Debug.Log($"[火焰面具]将{card.cardID}替换为基础火牌");
                Dealer.Instance.RemoveCard(card);
                BaseCard newCard = Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.base_fire_huo, 0);

                // 修复：正确查找背包中的 MagicBrush
                var magicBrush = GamePlayer.Instance.playerBag.treasures
                    .OfType<MagicBrush>()
                    .FirstOrDefault();

                magicBrush?.OnDrawCard(newCard);

                currentFireCardCount++;
            }
        }
    }



    

    public override void ResetOnLevelOver()
    {
        Debug.Log($"[火焰面具]清空火卡牌计数");
        currentFireCardCount = 0;
    }
}

