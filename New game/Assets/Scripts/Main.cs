using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        #region 鼠标点击逻辑网格测试
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hitInfo = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

            if (hitInfo)
            {
                if (hitInfo.collider.gameObject.CompareTag("LogicalGrid"))
                {
                    Plot plot = hitInfo.collider.gameObject.GetComponent<Plot>();
                    if (plot != null)
                    {
                        Debug.Log($"点击位置：{plot.logicalPos.x} {plot.logicalPos.y}");
                    }
                    else
                    {
                        Debug.LogWarning("该格子没有Plot组件");
                    }
                }
            }
        }
        #endregion

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

        if(Input.GetKeyDown(KeyCode.Q))
        {
            
        }


    }
}
