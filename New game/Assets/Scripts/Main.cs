
using System.Collections.Generic;
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
       
        #region 关卡状态机测试
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


       
            //// 鼠标左键按下时进行射线检测
            //if (Input.GetMouseButtonDown(0))
            //{
            //    // 将鼠标屏幕坐标转换为世界坐标
            //    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //    mouseWorldPos.z = 0; // 2D场景中Z轴设为0

            //    // 从鼠标位置发射射线（2D）
            //    RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            //    if (hit.collider != null)
            //    {
            //        Debug.Log($"命中物体: {hit.collider.name}, 位置: {hit.point}");
            //    }
            //    else
            //    {
            //        Debug.Log("没有命中任何物体");
            //    }
            //}

            //// 可选：实时显示鼠标位置的射线信息
            //if (Input.GetMouseButton(1)) // 右键持续显示
            //{
            //    Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //    mouseWorldPos.z = 0;
            //    RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            //    if (hit.collider != null)
            //    {
            //        Debug.Log($"实时命中: {hit.collider.name}");
            //    }
            //}
        

    }
}
