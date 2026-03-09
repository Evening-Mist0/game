using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class LevelMgr : BaseMonoMgr<LevelMgr>
{
    public LevelStateMachine machine = null;

    private void Awake()
    {
        machine = GetComponent<LevelStateMachine>();
        if (machine == null)
            machine = this.gameObject.AddComponent<LevelStateMachine>();
    }
}
