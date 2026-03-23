using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 记录所有的数据读取
/// </summary>
public class DataCenter : BaseMgr<DataCenter>
{
    public List<CardSynthesisFormulaData> synthesisFormulaList = new List<CardSynthesisFormulaData>();

    public CardResNameData cardResNameData;
    public MonsterResNameData monsterResNameData;
    public DefTowerResNameData defTowerResNameData;

    private DataCenter() 
    {
        synthesisFormulaList = JsonMgr.Instance.LoadData<List<CardSynthesisFormulaData>>("CardISynthesisFormulaInfo");

        cardResNameData = JsonMgr.Instance.LoadData<CardResNameData>("CardResName");

        monsterResNameData = JsonMgr.Instance.LoadData<MonsterResNameData>("MonsterResName");
        monsterResNameData.Initialize();

        defTowerResNameData = JsonMgr.Instance.LoadData<DefTowerResNameData>("DefTowerResName");
    }
}
