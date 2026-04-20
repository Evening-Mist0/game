using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasuresViewControl : MonoBehaviour
{
    List<TreasureIconControl> treasures = new List<TreasureIconControl>();

    public RectTransform father;

    /// <summary>
    /// 每次开始游戏时调用，遍历玩家背包中的奇物，实例化在面板上
    /// </summary>
    public void Refresh()
    {
        RemoveTreasureIcons();

        List<BaseTreasure> tempList = GamePlayer.Instance.playerBag.treasures;
        for(int i = 0; i < tempList.Count; i++)
        {
            CreateTreasureIcon(tempList[i]);
        }
    }

    private void CreateTreasureIcon(BaseTreasure treasure)
    {
        string name = typeof(BaseTreasure).Name;
        GameObject obj = Instantiate(Resources.Load<GameObject>("TreasureIcon/"+name));
        TreasureIconControl icon = obj.GetComponent<TreasureIconControl>();
        if(obj == null || treasure == null)
        {
            Debug.LogError("获取的对象为空或路径传入错误");
            return;
        }
        AddTreasureIcon(icon);
        icon.gameObject.transform.SetParent(father,false);
    }

    private void AddTreasureIcon(TreasureIconControl icon)
    {
        if(!treasures.Contains(icon))
            treasures.Add(icon);
    }

    private void RemoveTreasureIcon(TreasureIconControl icon)
    {
        if (treasures.Contains(icon))
            treasures.Remove(icon);
    }

    private void RemoveTreasureIcons()
    {
        List <TreasureIconControl> tempList = treasures;
        for (int i = 0; i < tempList.Count;i++)
        {
            if (tempList[i] != null && tempList[i].gameObject != null)
                Destroy(tempList[i].gameObject);

        }
        treasures.Clear();
    }
}
