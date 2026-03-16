using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // 核心：让框架加载并显示你的开始面板
        UIMgr.Instance.ShowPanel<StartPanel>(E_UILayerType.middle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
