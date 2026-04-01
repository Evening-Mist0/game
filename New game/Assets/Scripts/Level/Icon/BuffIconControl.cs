using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_BuffIconType
{
    Heal,
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
    /// <summary>
    /// 死亡反击
    /// </summary>
    DeadReflect,
    /// <summary>
    /// 给其他怪物加血
    /// </summary>
    AddBloodToMonster,
    /// <summary>
    /// 移动
    /// </summary>
    Move,
    /// <summary>
    /// 怪物的特性描述，
    /// </summary>
    MonsterDescription_Monster_Earth01_StoneSprite,
    MonsterDescription_Monster_Earth02_ShieldGuard,
    MonsterDescription_Monster_Earth03_StoneGiant,
    MonsterDescription_Monster_Monster_Earth03_StoneGiant,
    MonsterDescription_Monster_Monster_Fire01_FlameSprite,
    MonsterDescription_Monster_Monster_Fire02_CombustionWorm,
    MonsterDescription_Monster_Monster_Fire03_MoltenGuard,
    MonsterDescription_Monster_None01_GodofAllElementalArts_FireForm,
    MonsterDescription_Monster_None01_GodofAllElementalArts_WaterForm,
    MonsterDescription_Monster_None01_GodofAllElementalArts_EarthForm,
    MonsterDescription_Monster_Water01_WaterWisp,
    MonsterDescription_Monster_Water02_TideSoldier,
    MonsterDescription_Monster_Water03_AbyssEel,
    MonsterDescription_Monster_Fire01_FlameSprite,
    MonsterDescription_Monster_Fire02_CombustionWorm,
    MonsterDescription_Monster_Fire03_MoltenGuard,

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
    private Vector3 tipOffsetPos = new Vector3(0.8f,0.34f,0);


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
            case E_BuffIconType.Heal:
                description = DataCenter.Instance.buffDescribeData.heal;
                break;
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
            case E_BuffIconType.GetDef:
                description = DataCenter.Instance.buffDescribeData.getDef;
                break;
            case E_BuffIconType.DeadReflect:
                description = DataCenter.Instance.buffDescribeData.deadReflect;
                break;
            case E_BuffIconType.AddBloodToMonster:
                description = DataCenter.Instance.buffDescribeData.AddBloodToMonster;
                break;
            case E_BuffIconType.Move:
                description = DataCenter.Instance.buffDescribeData.move;
                break;
            case E_BuffIconType.MonsterDescription_Monster_Earth01_StoneSprite:
                description = DataCenter.Instance.buffDescribeData.desMonster_Earth01_StoneSprite;
                break;
            case E_BuffIconType.MonsterDescription_Monster_Earth02_ShieldGuard:
                description = DataCenter.Instance.buffDescribeData.desMonster_Earth02_ShieldGuard;
                break;
            case E_BuffIconType.MonsterDescription_Monster_Earth03_StoneGiant:
                description = DataCenter.Instance.buffDescribeData.desMonster_Earth03_StoneGiant;
                break;
            case E_BuffIconType.MonsterDescription_Monster_Fire01_FlameSprite:
                description = DataCenter.Instance.buffDescribeData.desMonster_Fire01_FlameSprite;
                break;
            case E_BuffIconType.MonsterDescription_Monster_Fire02_CombustionWorm:
                description = DataCenter.Instance.buffDescribeData.desMonster_Fire02_CombustionWorm;
                break;
            case E_BuffIconType.MonsterDescription_Monster_Fire03_MoltenGuard:
                description = DataCenter.Instance.buffDescribeData.desMonster_Fire03_MoltenGuard;
                break;
            case E_BuffIconType.MonsterDescription_Monster_None01_GodofAllElementalArts_FireForm:
                description = DataCenter.Instance.buffDescribeData.desMonster_None01_GodofAllElementalArts_FireForm;
                break;
            case E_BuffIconType.MonsterDescription_Monster_None01_GodofAllElementalArts_WaterForm:
                description = DataCenter.Instance.buffDescribeData.desMonster_None01_GodofAllElementalArts_WaterForm;
                break;
            case E_BuffIconType.MonsterDescription_Monster_None01_GodofAllElementalArts_EarthForm:
                description = DataCenter.Instance.buffDescribeData.desMonster_None01_GodofAllElementalArts_EarthForm;
                break;
            case E_BuffIconType.MonsterDescription_Monster_Water01_WaterWisp:
                description = DataCenter.Instance.buffDescribeData.desMonster_Water01_WaterWisp;
                break;
            case E_BuffIconType.MonsterDescription_Monster_Water02_TideSoldier:
                description = DataCenter.Instance.buffDescribeData.desMonster_Water02_TideSoldier;
                break;
            case E_BuffIconType.MonsterDescription_Monster_Water03_AbyssEel:
                description = DataCenter.Instance.buffDescribeData.desMonster_Water03_AbyssEel;
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