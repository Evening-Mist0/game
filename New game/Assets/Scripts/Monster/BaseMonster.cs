using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMonster : MonoBehaviour
{
    private void Awake()
    {
        EventCenter.Instance.AddEventListener<BaseMonster>(E_EventType.MonsterHurt, Hurt);
    }

    public void Hurt(BaseMonster monster)
    {

    }

    private void OnDestroy()
    {
        EventCenter.Instance.RemoveEventListener<BaseMonster>(E_EventType.MonsterHurt, Hurt);
    }
}
