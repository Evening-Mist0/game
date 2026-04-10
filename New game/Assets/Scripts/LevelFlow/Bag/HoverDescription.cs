using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 鼠标悬停描述组件
public class HoverDescription : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image iconImage;
    private string title;
    private string description;

    public void Init(string title, string desc)
    {
        this.title = title;
        this.description = desc;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // 显示描述面板（全局单例）
        DescriptionPanel.Instance.Show(title, description, transform.position);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DescriptionPanel.Instance.Hide();
    }
}
