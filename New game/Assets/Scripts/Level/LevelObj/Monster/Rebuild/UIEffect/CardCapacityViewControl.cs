using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardCapacityViewControl : MonoBehaviour
{
    public TMP_Text textCapacityNum;

    public void UpdateCapacityNum(int currentCardCount,int capacity)
    {
        textCapacityNum.text = currentCardCount.ToString() + "/" + capacity.ToString();
    }
}
