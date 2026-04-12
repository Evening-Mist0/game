using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string itemTitle;
    [SerializeField] private string itemDesc;

    public void Init(string title, string description)
    {
        itemTitle = title;
        itemDesc = description;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (DescriptionPanel.Instance != null)
        {
            // 使用 eventData.position 更准确
            DescriptionPanel.Instance.Show(itemTitle, itemDesc, eventData.position);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (DescriptionPanel.Instance != null)
        {
            DescriptionPanel.Instance.Hide();
        }
    }
}