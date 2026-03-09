using UnityEngine;
using System;
using System.Collections.Generic;

#region 基础枚举
// 卡牌类型枚举（基础/合成/部首）
public enum CardType
{
    Base,       // 基础牌（水火土木）
    Combine,    // 合成牌
    Radical     // 部首牌（回合结束销毁，可合成）
}

// 元素属性枚举（单元素/双元素，兼容沐<木+水>这类双属性）
[Flags] // 支持多元素拼接，如CardElement.Wood | CardElement.Water
public enum CardElement
{
    None = 0,   // 无属性
    Fire = 1,   // 火
    Water = 2,  // 水
    Earth = 4,  // 土
    Wood = 8    // 木
}

// 生效范围类型（适配1×1/2×4/十字/全屏等）
public enum EffectRangeType
{
    SingleGrid, // 1×1单格
    Rect,       // 矩形范围（如2×4、3×1）
    Cross,      // 十字范围
    FullScreen, // 全屏
    Self        // 自身（如地牌加护甲）
}

// 附加效果枚举（支持多效果叠加，如灼烧+真伤）
[Flags]
public enum CardExtraEffect
{
    None = 0,
    Burn,       // 灼烧
    Repel,      // 击退
    Imprison,   // 禁锢
    Reflect,    // 反伤
    Heal,       // 恢复
    TrueDamage  // 真伤
}

// 卡牌强化维度（适配执照/奇物的强化方向)
public enum CardStrengthenType
{
    Damage,     // 伤害强化
    RepelStep,  // 击退步数强化
    ImprisonRound, // 禁锢回合强化
    PlaceHp,    // 放置物血量强化
    Range,      // 生效范围强化
    Consume,    // 消耗规则强化（如不消耗卡牌）
    HealValue   // 恢复数值强化
}
#endregion

#region 卡牌合成配方类（新增，适配典籍解锁配方）
/// <summary>
/// 卡牌合成配方实体，存储合成所需素材、是否被典籍解锁
/// </summary>
[Serializable]
public class CardCombineFormula
{
    public List<string> needCardIds; // 合成所需卡牌ID列表（如火+火=炎）
    public bool isUnlocked = false;  // 是否被典籍解锁（默认关闭，典籍激活后解锁）
    public bool isRareFormula = false; // 是否为稀有合成配方（如焚、燚，适配一气呵成执照）
}
#endregion

/// <summary>
/// 卡牌基类，所有卡牌子类继承此抽象类
/// 完善后适配肉鸽爬塔：执照升级、典籍解锁、奇物强化三大成长维度
/// </summary>
public abstract class BaseCard : MonoBehaviour
{
    #region 1. 基础标识属性
    [Header("卡牌基础配置")]
    [Tooltip("卡牌唯一ID")]
    public string cardId;
    [Tooltip("卡牌名称")]
    public string cardName;
    [Tooltip("卡牌类型")]
    public CardType cardType;
    [Tooltip("元素属性")]
    public CardElement cardElement;
    [Tooltip("卡牌稀有度")]
    public bool isRareCard = false;
    [Tooltip("是否为放置类卡牌")]
    public bool isPlaceCard = false;
    #endregion

    #region 2. 生效范围属性
    [Header("生效范围配置")]
    [Tooltip("生效范围类型：单格/矩形/十字/全屏/自身")]
    public EffectRangeType rangeType;

    [Tooltip("矩形范围宽-基础值")]
    public int baseRangeWidth = 1;
    [Tooltip("矩形范围高-基础值")]
    public int baseRangeHeight = 1;

    [Tooltip("当前实际生效宽度（由基础值+强化值计算，运行时赋值")]
    [HideInInspector] public int currentRangeWidth;
    [Tooltip("当前实际生效高度（由基础值+强化值计算，运行时赋值）")]
    [HideInInspector] public int currentRangeHeight;

    [Tooltip("十字范围半径-基础值）")]
    public int baseCrossRadius = 1;
    [Tooltip("当前十字范围半径（运行时赋值）")]
    [HideInInspector] public int currentCrossRadius;
    #endregion

    #region 3. 核心效果数值
    [Header("核心效果数值-基础值（强化后从这里读取原始值）")]
    [Tooltip("基础伤害/护甲/恢复量/反伤值等核心数值-基础值")]
    public int baseCoreValue;
    [Tooltip("效果持续回合数-基础值（如灼烧2回合、禁锢1回合）")]
    public int baseEffectLastRound = 0;
    [Tooltip("附加效果配套数值-基础值（如击退2格、灼烧每回合3伤）")]
    public int baseExtraEffectValue;
    [Tooltip("放置类卡牌的实体血量-基础值（如木障1点、坷牌土块6点，适配执照/奇物强化）")]
    public int basePlaceCardHp = 0;

    [Header("核心效果数值-当前值（运行时由基础值+强化值计算）")]
    [HideInInspector] public int currentCoreValue;
    [HideInInspector] public int currentEffectLastRound;
    [HideInInspector] public int currentExtraEffectValue;
    [HideInInspector] public int currentPlaceCardHp;
    #endregion

    #region 4. 附加效果属性）
    [Header("附加效果配置")]
    [Tooltip("卡牌附加效果：灼烧/击退/禁锢等，支持多效果叠加")]
    public CardExtraEffect extraEffects;
    #endregion

    #region 5. 留存/消耗规则
    [Header("留存/消耗规则配置")]
    [Tooltip("是否跨回合保留（部首牌强制false，基础/合成牌默认true）")]
    public bool isCrossRoundKeep = true;
    [Tooltip("是否为使用后销毁（所有卡牌使用/合成后均销毁，默认true）")]
    public bool isUseDestroy = true;
    [Tooltip("当前是否触发「不消耗」效果（适配燧石/贝壳/元素充盈执照，运行时赋值）")]
    [HideInInspector] public bool isNoConsume = false;
    [Tooltip("「不消耗」效果的触发概率（0-1，如50%则为0.5）")]
    [HideInInspector] public float noConsumeProb = 0f;
    [Tooltip("稀有合成牌是否保留部首牌（适配一气呵成执照，运行时赋值）")]
    [HideInInspector] public bool isKeepRadicalWhenCombine = false;
    #endregion

    #region 6. 合成相关属性
    [Header("合成配方配置（仅合成牌配置）")]
    [Tooltip("当前合成牌的配方信息（适配典籍解锁，如燚牌需火经·燎原解锁）")]
    public CardCombineFormula combineFormula;
    [Tooltip("是否为典籍专属合成牌（如燚、淼、垚，默认false）")]
    public bool isClassicBookCard = false;
    #endregion

    #region 7. 美术效果配置（原有）
    [Header("美术效果配置")]
    [Tooltip("卡牌使用/合成时的特效预制体")]
    public GameObject effectPrefab;
    [Tooltip("特效层级（如火焰最上层、水流在怪物下层）")]
    public int effectSortingOrder = 0;
    [Tooltip("卡牌图标（供手牌/图鉴/典籍/奇物面板显示）")]
    public Sprite cardIcon;
    #endregion

    #region 8. 图鉴解锁配置（完善，适配卡牌/典籍/奇物统一图鉴）
    [Header("图鉴解锁配置（预留）")]
    [Tooltip("是否为卡牌图鉴（默认true，典籍/奇物为false）")]
    public bool isCardAlbum = true;
    [Tooltip("是否已解锁图鉴（玩家首次获得时设为true）")]
    public bool isUnlockAlbum = false;
    [Tooltip("图鉴分类ID（卡牌/典籍/奇物分别对应不同ID，方便面板分类）")]
    public string albumCateId = "card";
    #endregion

    #region 9. 全局常量（原有，贴合卡牌通用规则）
    /// <summary>
    /// 全局手牌上限-基础值（适配手牌扩容执照，改为可动态修改）
    /// </summary>
    public static int BaseMaxHandCardCount = 15;
    /// <summary>
    /// 全局手牌上限-当前值（运行时由基础值+扩容值计算）
    /// </summary>
    public static int CurrentMaxHandCardCount = 15;
    #endregion

    #region 10. 成长相关事件（核心新增，解耦执照/奇物/典籍对卡牌的修改）
    /// <summary>
    /// 卡牌数值强化事件（如执照升级的元素充盈、战法经·破妄）
    /// 入参：强化维度、强化值（如Damage+1、ImprisonRound+1）
    /// </summary>
    public event Action<CardStrengthenType, int> OnCardStrengthen;
    /// <summary>
    /// 卡牌范围强化事件（如笔锋奇物的范围+1）
    /// 入参：强化值（如+1）
    /// </summary>
    public event Action<int> OnCardRangeStrengthen;
    /// <summary>
    /// 卡牌消耗规则修改事件（如元素充盈、贝壳奇物的不消耗概率）
    /// 入参：不消耗概率（0-1）
    /// </summary>
    public event Action<float> OnCardNoConsumeSet;
    /// <summary>
    /// 合成配方解锁事件（如典籍激活解锁燚/淼配方）
    /// 入参：是否解锁
    /// </summary>
    public event Action<bool> OnFormulaUnlocked;
    #endregion

  
    private void Awake()
    {
        // 初始化当前数值=基础数值，避免强化前数值为空
        InitCardValue();
        // 注册成长事件
        RegisterGrowEvent();
        // 子类初始化（留给子类重写）
        OnCardAwake();
    }

    /// <summary>
    /// 初始化卡牌当前数值为基础值
    /// </summary>
    private void InitCardValue()
    {
        currentCoreValue = baseCoreValue;
        currentEffectLastRound = baseEffectLastRound;
        currentExtraEffectValue = baseExtraEffectValue;
        currentPlaceCardHp = basePlaceCardHp;
        currentRangeWidth = baseRangeWidth;
        currentRangeHeight = baseRangeHeight;
        currentCrossRadius = baseCrossRadius;
    }

    /// <summary>
    /// 注册成长相关事件（核心逻辑，绑定事件处理方法）
    /// </summary>
    private void RegisterGrowEvent()
    {
        OnCardStrengthen += HandleCardStrengthen;
        OnCardRangeStrengthen += HandleCardRangeStrengthen;
        OnCardNoConsumeSet += HandleCardNoConsumeSet;
        OnFormulaUnlocked += HandleFormulaUnlocked;
    }

    /// <summary>
    /// 子类Awake（留给子类重写，避免覆盖基类Awake）
    /// </summary>
    protected virtual void OnCardAwake() { }


    #region 成长事件处理方法（核心新增，处理执照/奇物/典籍的强化逻辑）
    /// <summary>
    /// 处理卡牌数值强化（如伤害+1、禁锢回合+1）
    /// </summary>
    private void HandleCardStrengthen(CardStrengthenType type, int value)
    {
        switch (type)
        {
            case CardStrengthenType.Damage:
                currentCoreValue += value;
                break;
            case CardStrengthenType.ImprisonRound:
                currentEffectLastRound += value;
                break;
            case CardStrengthenType.RepelStep:
                currentExtraEffectValue += value;
                break;
            case CardStrengthenType.PlaceHp:
                currentPlaceCardHp += value;
                break;
            case CardStrengthenType.HealValue:
                currentCoreValue += value;
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 处理卡牌范围强化（如笔锋奇物的范围+1）
    /// </summary>
    private void HandleCardRangeStrengthen(int value)
    {
        currentRangeWidth += value;
        currentRangeHeight += value;
        currentCrossRadius += value;
        // 范围最小为1，避免强化后为0
        currentRangeWidth = Mathf.Max(1, currentRangeWidth);
        currentRangeHeight = Mathf.Max(1, currentRangeHeight);
        currentCrossRadius = Mathf.Max(1, currentCrossRadius);
    }

    /// <summary>
    /// 处理卡牌不消耗规则设置（如贝壳奇物50%不消耗水牌）
    /// </summary>
    private void HandleCardNoConsumeSet(float prob)
    {
        noConsumeProb = prob;
        // 概率范围限制0-1
        noConsumeProb = Mathf.Clamp01(noConsumeProb);
    }

    /// <summary>
    /// 处理合成配方解锁（如典籍激活解锁燚牌配方）
    /// </summary>
    private void HandleFormulaUnlocked(bool isUnlocked)
    {
        if (combineFormula != null)
        {
            combineFormula.isUnlocked = isUnlocked;
        }
    }
    #endregion

    #region 通用强化方法
    /// <summary>
    /// 调用卡牌数值强化（外部管理器调用，如战法经·破妄伤害+1）
    /// </summary>
    /// <param name="type">强化维度</param>
    /// <param name="value">强化值</param>
    public void TriggerStrengthen(CardStrengthenType type, int value)
    {
        OnCardStrengthen?.Invoke(type, value);
    }

    /// <summary>
    /// 调用卡牌范围强化（外部管理器调用，如笔锋奇物范围+1）
    /// </summary>
    /// <param name="value">强化值</param>
    public void TriggerRangeStrengthen(int value)
    {
        OnCardRangeStrengthen?.Invoke(value);
    }

    /// <summary>
    /// 调用卡牌不消耗规则设置（外部管理器调用，如元素充盈火牌50%不消耗）
    /// </summary>
    /// <param name="prob">不消耗概率（0-1）</param>
    public void TriggerNoConsumeSet(float prob)
    {
        OnCardNoConsumeSet?.Invoke(prob);
    }

    /// <summary>
    /// 调用合成配方解锁（外部典籍管理器调用，如火经·燎原解锁燚牌配方）
    /// </summary>
    /// <param name="isUnlocked">是否解锁</param>
    public void TriggerFormulaUnlocked(bool isUnlocked)
    {
        OnFormulaUnlocked?.Invoke(isUnlocked);
    }

    /// <summary>
    /// 校验是否触发不消耗效果（如50%概率不消耗，运行时调用）
    /// </summary>
    /// <returns>是否不消耗</returns>
    public bool CheckNoConsume()
    {
        if (noConsumeProb <= 0) return false;
        return UnityEngine.Random.value <= noConsumeProb;
    }

    /// <summary>
    /// 重置卡牌所有数值为基础值（如爬塔失败/通关后重置，供外部调用）
    /// </summary>
    public void ResetCardValue()
    {
        InitCardValue();
        noConsumeProb = 0f;
        isNoConsume = false;
        isKeepRadicalWhenCombine = false;
        if (combineFormula != null && isClassicBookCard)
        {
            combineFormula.isUnlocked = false;
        }
    }
    #endregion

    #region 原有抽象方法（子类必须实现，个性化核心逻辑）
    /// <summary>
    /// 卡牌使用逻辑（核心方法，触发伤害/效果/放置实体等）
    /// </summary>
    /// <param name="targetPos">目标位置（格子坐标）</param>
    public abstract void OnUseCard(Vector2Int targetPos);

    /// <summary>
    /// 卡牌合成成功后的逻辑（如生成合成牌、播放合成动画）
    /// </summary>
    /// <param name="partnerCard">合成的另一张卡牌</param>
    /// <returns>合成后的新卡牌实例</returns>
    public abstract BaseCard OnCombineCard(BaseCard partnerCard);

    /// <summary>
    /// 卡牌销毁逻辑（使用/合成/回合结束时触发，如从手牌移除、清理特效）
    /// </summary>
    public abstract void OnDestroyCard();

    /// <summary>
    /// 触发附加效果（如灼烧每回合结算、禁锢生效）
    /// </summary>
    /// <param name="target">目标对象（怪物/玩家/格子）</param>
    public abstract void TriggerExtraEffect(GameObject target);
    #endregion

    #region 原有虚方法（通用逻辑，子类可重写）
    /// <summary>
    /// 卡牌高亮逻辑（合成配对时触发，默认实现基础高亮，子类可重写）
    /// </summary>
    /// <param name="isHighlight">是否高亮</param>
    public virtual void SetCardHighlight(bool isHighlight)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = isHighlight ? Color.yellow : Color.white;
        }
    }

    /// <summary>
    /// 播放卡牌美术特效（通用逻辑，子类可重写）
    /// </summary>
    /// <param name="spawnPos">特效生成位置</param>
    public virtual void PlayEffect(Vector2 spawnPos)
    {
        if (effectPrefab == null) return;
        GameObject effect = Instantiate(effectPrefab, spawnPos, Quaternion.identity);
        SpriteRenderer effectSr = effect.GetComponent<SpriteRenderer>();
        if (effectSr != null)
        {
            effectSr.sortingOrder = effectSortingOrder;
        }
        Destroy(effect, 2f);
    }

    /// <summary>
    /// 图鉴解锁逻辑（玩家首次获得时调用，兼容卡牌/典籍/奇物）
    /// </summary>
    public virtual void UnlockAlbum()
    {
        if (!isUnlockAlbum)
        {
            isUnlockAlbum = true;
            Debug.Log($"【{albumCateId}】{cardName} 已解锁图鉴！");
            // 对接图鉴面板事件，如：AlbumManager.Instance.OnAlbumUnlocked(this);
        }
    }
    #endregion

    #region 原有通用工具方法（微调，适配动态手牌上限）
    /// <summary>
    /// 检查手牌是否达到当前上限（适配手牌扩容执照，供抽卡/合成奖励调用）
    /// </summary>
    /// <param name="handCardCount">当前手牌数量</param>
    /// <returns>是否可添加新卡牌</returns>
    public bool CheckHandCardLimit(int handCardCount)
    {
        return handCardCount < CurrentMaxHandCardCount;
    }

    /// <summary>
    /// 检查是否为合法合成配对（基础+基础/基础+部首，供合成时校验）
    /// </summary>
    /// <param name="otherCard">另一张合成卡牌</param>
    /// <returns>是否合法</returns>
    public bool IsLegalCombine(BaseCard otherCard)
    {
        if ((this.cardType == CardType.Base && otherCard.cardType == CardType.Base) ||
            (this.cardType == CardType.Base && otherCard.cardType == CardType.Radical) ||
            (this.cardType == CardType.Radical && otherCard.cardType == CardType.Base))
        {
            // 额外校验：合成配方是否解锁（如燚牌需典籍解锁后才能合成）
            if (this.isClassicBookCard && !this.combineFormula.isUnlocked)
            {
                Debug.LogWarning($"卡牌{this.cardName}的合成配方未解锁，需激活对应典籍！");
                return false;
            }
            if (otherCard.isClassicBookCard && !otherCard.combineFormula.isUnlocked)
            {
                Debug.LogWarning($"卡牌{otherCard.cardName}的合成配方未解锁，需激活对应典籍！");
                return false;
            }
            return true;
        }
        return false;
    }
    #endregion

    #region 手动解除事件（防止内存泄漏）
    private void OnDestroy()
    {
        OnCardStrengthen -= HandleCardStrengthen;
        OnCardRangeStrengthen -= HandleCardRangeStrengthen;
        OnCardNoConsumeSet -= HandleCardNoConsumeSet;
        OnFormulaUnlocked -= HandleFormulaUnlocked;
    }
    #endregion
}