using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_BuffIconType
{
    Burn,
    ImmunityBurn,
    Imprison,
    ImmunityImprison,
    SpeedUp,
    Reflect,
    /// <summary>
    /// 任意伤害减伤
    /// </summary>
    ArbitraryDamegeRedution,
    /// <summary>
    /// 火伤害减伤
    /// </summary>
    FireDamegeRedution,
    /// <summary>
    /// 元素湮灭
    /// </summary>
    AnnihilationOfElements,
    /// <summary>
    /// 摧毁前方阻挡物
    /// </summary>
    DestroyBuildings,
    /// <summary>
    /// 每回合会获得护盾
    /// </summary>
    GetDef,
    /// <summary>
    /// 放置木系卡牌
    /// </summary>
    Wood,


}
public class BuffIconControl : MonoBehaviour
{
    //描述气泡预设体
    GameObject obj;
    //buff持续的回合数
    private int count;
    //描述内容，根据Init方法自动获取
    private string description;
    [Header("基础配置")]
    [Tooltip("该图标需要描述的内容枚举")]
    public E_BuffIconType myType;
    [Tooltip("回合数更新图片控件")]
    public SpriteRenderer srCount;
    [Tooltip("提示气泡的偏移量坐标")]
    private Vector3 tipOffsetPos = new Vector3(0.3f,0.34f,0);


    private void Awake()
    {
        Init(myType);
    }

    private void OnMouseEnter()
    {
        Debug.Log("鼠标进入Icon");
        obj = PoolMgr.Instance.GetObj("UI/DescriptionBubble");
        if (obj != null)
        {
            DescriptionBubble bubble = obj.GetComponent<DescriptionBubble>();
            Vector3 newPos = this.transform.position;
            newPos.z = 0;
            newPos += tipOffsetPos;
            bubble.transform.position = newPos;
            bubble.UpdateDescibe(description);
        }
    }

    private void OnMouseExit()
    {
        PoolMgr.Instance.PushObj(obj);
    }

    private void Init(E_BuffIconType type)
    {
        switch (type)
        {
            case E_BuffIconType.Burn:
                description = DataCenter.Instance.buffDescribeData.burn;
                break;
            case E_BuffIconType.ImmunityBurn:
                description = DataCenter.Instance.buffDescribeData.immunityBurn;
                break;
            case E_BuffIconType.Imprison:
                description = DataCenter.Instance.buffDescribeData.imprison;

                break;
            case E_BuffIconType.ImmunityImprison:
                description = DataCenter.Instance.buffDescribeData.immunityImprison;

                break;
            case E_BuffIconType.SpeedUp:
                description = DataCenter.Instance.buffDescribeData.speedUp;

                break;
            case E_BuffIconType.Reflect:
                description = DataCenter.Instance.buffDescribeData.reflect;

                break;
            case E_BuffIconType.ArbitraryDamegeRedution:
                description = DataCenter.Instance.buffDescribeData.arbitraryDamegeRedution;

                break;
            case E_BuffIconType.FireDamegeRedution:
                description = DataCenter.Instance.buffDescribeData.firedDamegeRedution;

                break;
            case E_BuffIconType.AnnihilationOfElements:
                description = DataCenter.Instance.buffDescribeData.annihilationOfElements;

                break;
            case E_BuffIconType.DestroyBuildings:
                description = DataCenter.Instance.buffDescribeData.destroyBuildings;
                break;
        }
    }

    /// <summary>
    /// 更新图标的回合数
    /// </summary>
    public void UpdateMyIconCount(int count)
    {
        if(count <= 0)
            return;
        string path = "Number/"+count.ToString();
        srCount.sprite = Resources.Load<Sprite>(path);
    }
}