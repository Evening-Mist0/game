using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSynthesisFormulaTable : BaseMgr<CardSynthesisFormulaTable>
{
    private CardSynthesisFormulaTable()
    {
        List<CardSynthesisFormulaData> list = DataCenter.Instance.synthesisFormulaList;
        if (list == null)
        {
            Debug.LogError("合成表获取失败");
            return;
        }
        
        for(int i = 0;i < list.Count;i++)
        {
            string id1 = list[i].cardId;
            string id2 = list[i].synthesize_to_ids;
            Tuple<string,string> tuple = GetSortedCardIdTuple(id1, id2);
            SynthesisDic.Add(tuple, list[i]);
        }
    }


    // 改用Tuple作为键
    public Dictionary<Tuple<string, string>, CardSynthesisFormulaData> SynthesisDic = new Dictionary<Tuple<string, string>, CardSynthesisFormulaData>();

    // 生成Tuple键
    public Tuple<string, string> GetSortedCardIdTuple(string id1, string id2)
    {
        var sortedIds = new[] { id1, id2 }.OrderBy(s => s).ToArray();
        return Tuple.Create(sortedIds[0], sortedIds[1]);
    }
}
