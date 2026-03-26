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
    public EffectResNameData effectResNameData;
    public BuffIconResNameData buffIconResNameData;
    public BuffDescribeData buffDescribeData;
    private DataCenter() 
    {
        synthesisFormulaList = JsonMgr.Instance.LoadData<List<CardSynthesisFormulaData>>("CardISynthesisFormulaInfo");

        cardResNameData = JsonMgr.Instance.LoadData<CardResNameData>("CardResName");

        monsterResNameData = JsonMgr.Instance.LoadData<MonsterResNameData>("MonsterResName");
        monsterResNameData.Initialize();

        defTowerResNameData = JsonMgr.Instance.LoadData<DefTowerResNameData>("DefTowerResName");
        effectResNameData = JsonMgr.Instance.LoadData<EffectResNameData>("EffectResName");
        buffIconResNameData = JsonMgr.Instance.LoadData<BuffIconResNameData>("BuffIconResName");
        buffDescribeData = JsonMgr.Instance.LoadData<BuffDescribeData>("BuffDescribeInfo");

    }
}
