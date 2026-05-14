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
    /// 虚弱
    /// </summary>
    Weakness,
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
    MonsterDescription_Monster_Fire01_FlameSprite,
    MonsterDescription_Monster_Fire02_CombustionWorm,
    MonsterDescription_Monster_Fire03_MoltenGuard,
    MonsterDescription_Monster_None01_GodofAllElementalArts_FireForm,
    MonsterDescription_Monster_None01_GodofAllElementalArts_WaterForm,
    MonsterDescription_Monster_None01_GodofAllElementalArts_EarthForm,
    MonsterDescription_Monster_Water01_WaterWisp,
    MonsterDescription_Monster_Water02_TideSoldier,
    MonsterDescription_Monster_Water03_AbyssEel,
    MonsterDescription_Monster_Wood01_WoodSpirit,

    /// <summary>
    /// 阻挡物特性描述
    /// </summary>
    TowerDescripTion_Refelct,
    TowerDescripTion_LieGu,
    TowerDescripTion_Chain,
    TowerDescripTion_WaterRegion,
    TowerDescripTion_ImprisonRegion,


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
    private Vector3 tipOffsetPos = new Vector3(2.3f,0,0);

    //是否格式化过，如果格式化过就不要在执行描述更新
    public bool isFormatted = false;


    private void Start()
    {
        UpdateIconDescription(myType);
    }

    private void OnMouseDown()
    {
        Debug.Log("鼠标点击Icon");
        Camera.main.depth = 1;
        obj = PoolMgr.Instance.GetObj("UI/DescriptionBubble");
        if (obj != null)
        {
            DescriptionBubble bubble = obj.GetComponent<DescriptionBubble>();
            Vector3 newPos = this.transform.position;
            float offsetY = bubble.GetTopToCenterYOffset();
            tipOffsetPos.y = -offsetY;
            Debug.Log("tipOffsetPos:"+tipOffsetPos);

            newPos += tipOffsetPos;
            bubble.transform.position = newPos;
            bubble.transform.localScale = Vector3.one;
            Debug.Log("更新的内容为" + description);
            bubble.UpdateDescibe(description);
            tipOffsetPos.y = 0;
        }
    }

    private void OnMouseUp()
    {
        Debug.Log("技能介绍面板：鼠标抬起");
        PoolMgr.Instance.PushObj(obj);
        Camera.main.depth = -1;
    }

 
    public void UpdateIconDescription(E_BuffIconType type)
    {
        if (isFormatted) return;
        Debug.Log($"无参重载被调用，实例={GetHashCode()}，type={type}\n{StackTraceUtility.ExtractStackTrace()}");

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
                Debug.Log("description替换为" + description);
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
            case E_BuffIconType.MonsterDescription_Monster_Wood01_WoodSpirit:
                description = DataCenter.Instance.buffDescribeData.desMonster_Wood01_WoodSpirit;
                break;
            case E_BuffIconType.TowerDescripTion_Refelct:
                description = DataCenter.Instance.buffDescribeData.TowerDescripTion_Refelct;
                break;
            case E_BuffIconType.TowerDescripTion_LieGu:
                description = DataCenter.Instance.buffDescribeData.TowerDescripTion_LieGu;
                break;
            case E_BuffIconType.TowerDescripTion_ImprisonRegion:
                description = DataCenter.Instance.buffDescribeData.TowerDescripTion_ImprisonRegion;
                break;
            case E_BuffIconType.TowerDescripTion_WaterRegion:
                description = DataCenter.Instance.buffDescribeData.TowerDescripTion_WaterRegion;
                break;
            case E_BuffIconType.TowerDescripTion_Chain:
                description = DataCenter.Instance.buffDescribeData.TowerDescripTion_Chain;
                break;
        }
        isFormatted = true;
    }

    /// <summary>
    /// 支持两个替换符的替换，主要用于建筑物的升级描述
    /// </summary>
    /// <param name="type"></param>
    /// <param name="effectValue">技能的效果值</param>
    /// <param name="roundValue">技能带来的回合值</param>
    public void UpdateIconDescription(E_BuffIconType type,int effectValue,int roundValue)
    {
        string newStr;
        string strEffectValue = effectValue > 0 ? effectValue.ToString() : "";
        string strRoundValue = roundValue > 0 ? roundValue.ToString() : "";


        switch (type)
        {
       
            case E_BuffIconType.TowerDescripTion_Refelct:
                newStr = DataCenter.Instance.buffDescribeData.reflect;
                break;
            case E_BuffIconType.TowerDescripTion_LieGu:
                newStr = DataCenter.Instance.buffDescribeData.TowerDescripTion_LieGu;

                break;
            case E_BuffIconType.TowerDescripTion_Chain:
                newStr = DataCenter.Instance.buffDescribeData.TowerDescripTion_Chain;
                break;
            case E_BuffIconType.TowerDescripTion_WaterRegion:
                newStr = DataCenter.Instance.buffDescribeData.TowerDescripTion_WaterRegion;
                break;
            case E_BuffIconType.TowerDescripTion_ImprisonRegion:
                newStr = DataCenter.Instance.buffDescribeData.TowerDescripTion_ImprisonRegion;
                break;

            default:
                Debug.Log(type + "描述还没有设置替换符号，请用另外一个重载");
                newStr = string.Empty;
                break;
        }
        newStr = string.Format(newStr, strEffectValue, strRoundValue);
        if (newStr != string.Empty)
            description = newStr;

        Debug.Log("[特征描述控件]更新" + type + "的描述为" + description);
        isFormatted = true;
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