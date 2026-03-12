using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 记录玩家持有的卡牌，用于不同阶段对卡牌的更新
/// </summary>
public class CardMgr : BaseMonoMgr<CardMgr>
{
    //当前玩家持有的卡牌
    public List<BaseCard> nowCards = new List<BaseCard>();
  
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }   
}
