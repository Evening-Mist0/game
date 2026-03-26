using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DescriptionBubble : MonoBehaviour
{
    public TMP_Text textDescribe;
   
    public void UpdateDescibe(string content)
    {
        textDescribe.text = content;
    }
}
