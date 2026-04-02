using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCardDrop : MonoBehaviour
{
    // 关联你的卡牌脚本
    private CardPlayingPanel _cardPlayingPanel;
    private BaseMonsterCore _monsterCore;

    public void Init(BaseMonsterCore core, MonsterEffectControl effectControl)
    {
        _monsterCore = core;
        // 自动查找场景中的卡牌面板
        _cardPlayingPanel = FindObjectOfType<CardPlayingPanel>();
    }

    // 怪物死亡调用
    public void TryDropCard()
    {
        // 空值判断
        if (_cardPlayingPanel == null || _monsterCore == null)
        {
            Debug.LogError("卡牌面板/怪物核心为空，无法掉落");
            return;
        }

        // 调用卡牌脚本的掉落方法，传入【怪物自身Transform】作为生成位置
        _cardPlayingPanel.DropRandomRadicalCard(_monsterCore.transform.position);
        Debug.Log($"✅ {_monsterCore.monsterName} 死亡，尝试掉落卡牌");
    }
}