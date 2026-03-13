using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Cell))]
public class CellEventTrigger : MonoBehaviour
{
    Cell myCell;
    private void Awake()
    {
        myCell = GetComponent<Cell>();
    }

    public void TriggerCellUpdateAllowedHighLight(bool isAllowed)
    {
        TypeSafeEventCenter.Instance.Trigger<CellUpdateAllowedHighLightEvent>(new CellUpdateAllowedHighLightEvent(myCell, isAllowed));
    }
}
