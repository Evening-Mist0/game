using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 石块（奇物）
/// </summary>
public class Stone : BaseTreasure
{
    //土基础牌附加的伤害值
    private int extraDamage = 1;
    public override int round => -1;


    public override E_TreasureType type => E_TreasureType.Stone;


    public override void OnCreatNewCard(BaseCard card)
    {
        base.OnCreatNewCard(card);
        if (card.elementType == E_Element.Earth)
        {
            card.desViewAtk = card.currentAtk + extraDamage;
            Debug.Log("[石块]更新{card.cardID}伤害为：" + card.desViewAtk);
            MonoMgr.Instance.StartCoroutine(UpdateDesAtk(card, card.desViewAtk));
        }
    }

    private IEnumerator UpdateDesAtk(BaseCard card, int atk)
    {
        yield return null;
        card.cardEffectControl.UpdateDesEffection(atk);
    }

    public override void OnPlay(BaseCard card)
    {
        if (card.elementType == E_Element.Earth)
        {
            Debug.Log("[石块]基础伤害增加：" + extraDamage);
            card.currentAtk += extraDamage;
        }
    }
    public override void OnPrevSlected(BaseCardScriptableData data)
    {
        base.OnPrevSlected(data);
        if (data.elementType == E_Element.Earth)
        {
            data.baseAtk += extraDamage;
        }
    }

}

