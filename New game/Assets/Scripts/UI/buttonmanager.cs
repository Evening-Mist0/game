using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class buttonmanager : MonoBehaviour
{
    public Button item;
    public bool isselected;
    public Image highlight;
    public void OnButttonClick()
    {
        isselected = !isselected;
        item.Select();
        highlight.enabled = isselected;
    }

  
}
