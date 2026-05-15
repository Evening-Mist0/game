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
    [Header("ID")]
    public E_BookType bookId;
    [Header("????")]
    public string bookName;
    [Header("????")]
    public string baseDesc;
    [Header("2???")]
    public string level1Desc;
    [Header("3???")]
    public string level2Desc;
    [Header("??")]
    public Sprite bookIcon;


    /// <summary>
    /// ????????????
    /// </summary>
    public string GetDescription(int level)
    {
        switch (level)
        {
            case 2: return string.IsNullOrEmpty(baseDesc) ? baseDesc : level1Desc;
            case 3: return string.IsNullOrEmpty(level2Desc) ? baseDesc : level2Desc;
            default: return baseDesc;
        }
    }
}


public class BookDisplayData
{
    public E_BookType bookType;
    public string bookName;
    public string bookDesc;      // ?????????
    public Sprite bookIcon;
    public int upgradeLevel;
}