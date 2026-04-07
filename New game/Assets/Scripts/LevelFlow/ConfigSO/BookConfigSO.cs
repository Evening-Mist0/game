using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BookConfig", menuName = "”Œœ∑≈‰÷√/≥…≥§/µ‰ºÆ≈‰÷√")]
public class BookConfigSO : ScriptableObject
{
    public List<BookConfig> bookConfigs = new List<BookConfig>();
}

[System.Serializable]
public class BookConfig
{
    [Header("µ‰ºÆID")]
    public E_BookType bookId;
    [Header("µ‰ºÆ√˚≥∆")]
    public string bookName;
    [Header("µ‰ºÆ√Ë ˆ")]
    public string bookDesc;
    [Header("µ‰ºÆÕº±Í")]
    public Sprite bookIcon;
}
