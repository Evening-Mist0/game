using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CallExit : MonoBehaviour
{
    [HideInInspector]
    public bool isOpen;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if (!isOpen)
                UIMgr.Instance.ShowPanel<SettingPanel>();
        }
    }
}
