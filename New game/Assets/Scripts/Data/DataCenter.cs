using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 记录所有的数据读取
/// </summary>
public class DataCenter : BaseMgr<DataCenter>
{
    public List<CardSynthesisFormulaData> synthesisFormulaList = new List<CardSynthesisFormulaData>();
    private DataCenter() 
    {
        synthesisFormulaList = JsonMgr.Instance.LoadData<List<CardSynthesisFormulaData>>("CardISynthesisFormulaInfo");
    }
   
}
