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


        for(int i = 0; i < GamePlayer.Instance.playerBag.treasures.Count; i++)
        {
            CreateTreasureIcon(GamePlayer.Instance.playerBag.treasures[i]);
        }
    }

    private void CreateTreasureIcon(BaseTreasure treasure)
    {
        string name = treasure.GetType().Name;
        Debug.Log("获取的名字" + name);
        GameObject obj = Instantiate(Resources.Load<GameObject>("TreasureIcon/" + name));
        if (obj == null)
        {
            Debug.LogError("加载 prefab 失败：" + name);
            return;
        }
        TreasureIconControl icon = obj.GetComponent<TreasureIconControl>();
        if (icon == null)
        {
            Debug.LogError("prefab 缺少 TreasureIconControl 组件");
            Destroy(obj);
            return;
        }
        if (treasure == null) return;

        AddTreasureIcon(icon);
        if (icon.isNumberImgVisible)
            icon.UpdateMyIconCount(treasure.round);
        
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

    public void UpdateIconCount(E_TreasureType type,int count)
    {
        for(int i = 0; i < treasures.Count;i++)
        {
            if(type == treasures[i].myType)
            {
                Debug.Log("找到背包中拥有奇物" + type + "更新数字下标");
                 treasures[i].UpdateMyIconCount(count);
            }
        }
    }
}
