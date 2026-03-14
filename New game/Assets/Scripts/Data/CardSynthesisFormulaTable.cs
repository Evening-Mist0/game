//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//public class CardSynthesisFormulaTable : BaseMgr<CardSynthesisFormulaTable>
//{
//    private CardSynthesisFormulaTable()
//    {
//        List<CardSynthesisFormulaData> list = DataCenter.Instance.synthesisFormulaList;
//        if (list == null)
//        {
//            Debug.LogError("合成表获取失败");
//            return;
//        }

//        for(int i = 0;i < list.Count;i++)
//        {
//            string id1 = list[i].cardId;
//            string id2 = list[i].synthesize_to_ids;
//            Tuple<string,string> tuple = GetSortedCardIdTuple(id1, id2);
//            SynthesisDic.Add(tuple, list[i]);
//        }
//    }


//    // 改用Tuple作为键
//    public Dictionary<Tuple<string, string>, CardSynthesisFormulaData> SynthesisDic = new Dictionary<Tuple<string, string>, CardSynthesisFormulaData>();

//    // 生成Tuple键
//    public Tuple<string, string> GetSortedCardIdTuple(string id1, string id2)
//    {
//        var sortedIds = new[] { id1, id2 }.OrderBy(s => s).ToArray();
//        return Tuple.Create(sortedIds[0], sortedIds[1]);
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSynthesisFormulaTable : BaseMgr<CardSynthesisFormulaTable>
{
    private CardSynthesisFormulaTable()
    {
        SynthesisDic = new Dictionary<Tuple<string, string>, CardSynthesisFormulaData>();

        // 加载配表数据（你的核心逻辑保留，仅补充注释+校验）
        List<CardSynthesisFormulaData> list = DataCenter.Instance.synthesisFormulaList;
        if (list == null)
        {
            Debug.LogError("合成表获取失败");
            return;
        }

        for (int i = 0; i < list.Count; i++)
        {
            CardSynthesisFormulaData formula = list[i];
            if (formula == null || !formula.isActive) continue; // 补充未激活过滤

            string materialId1 = formula.cardId;          // 第一个合成材料ID
            string materialId2 = formula.synthesizeCard; // 第二个合成材料ID（注：字段名语义为“合成结果ID”，实际用途为第二个材料ID）
            Tuple<string, string> tuple = GetSortedCardIdTuple(materialId1, materialId2);

            // 补充重复键校验（避免Add抛异常）
            if (SynthesisDic.ContainsKey(tuple))
            {
                Debug.LogWarning($"合成公式重复：{materialId1}+{materialId2}，覆盖旧数据");
                SynthesisDic[tuple] = formula;
            }
            else
            {
                SynthesisDic.Add(tuple, formula);
            }
        }
    }

    // 字典：键=排序后的两个材料ID，值=合成公式数据（无需暴露resultResName）
    public Dictionary<Tuple<string, string>, CardSynthesisFormulaData> SynthesisDic =
        new Dictionary<Tuple<string, string>, CardSynthesisFormulaData>();

    // 你的核心方法（逻辑正确，仅补充空值防御）
    public Tuple<string, string> GetSortedCardIdTuple(string id1, string id2)
    {
        // 补充空值防御：避免空字符串排序异常
        id1 = string.IsNullOrEmpty(id1) ? "" : id1;
        id2 = string.IsNullOrEmpty(id2) ? "" : id2;

        var sortedIds = new[] { id1, id2 }.OrderBy(s => s).ToArray();
        return Tuple.Create(sortedIds[0], sortedIds[1]);
    }

    // 对外提供的查询方法（封装字典读取，无需暴露内部字段）
    public bool TryGetFormula(string matId1, string matId2, out CardSynthesisFormulaData formula)
    {
        var tuple = GetSortedCardIdTuple(matId1, matId2);
        return SynthesisDic.TryGetValue(tuple, out formula);
    }
}
