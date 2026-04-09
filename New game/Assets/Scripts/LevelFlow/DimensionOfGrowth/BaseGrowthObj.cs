using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_GrowthObjType
{
    /// <summary>
    /// Ö´ÕÕ¼¼ÄÜ
    /// </summary>
    License,
    /// <summary>
    /// ÆæÎï
    /// </summary>
    Treasure,
    /// <summary>
    /// µä¼®
    /// </summary>
    Book,
}
public class BaseGrowthObj : MonoBehaviour
{
    public E_GrowthObjType growthType;
    public string ID;

}
