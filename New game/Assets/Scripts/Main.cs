using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
   
    }

    private void Awake()
    {
       List<CardSynthesisFormulaData> list = DataCenter.Instance.synthesisFormulaList;
    }

    // Update is called once per frame
    void Update()
    {
       
        #region ¹Ø¿¨×´Ì¬»ú²âÊÔ
        //if(Input.GetKeyDown(KeyCode.Q))
        //{
        //    LevelMgr.Instance.machine.ChangeState(E_LevelState.PlayerTurn_DrawCard);
        //}

        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    LevelMgr.Instance.machine.ChangeState(E_LevelState.Init);
        //}
        #endregion

        //if(Input.GetKeyDown(KeyCode.Q))
        //{

        //}

        UIMgr.Instance.ShowPanel<CardPlayingPanel>();
    }
}
