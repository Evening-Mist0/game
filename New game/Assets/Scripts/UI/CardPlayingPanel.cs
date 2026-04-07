using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 打牌面板
/// </summary>
public class CardPlayingPanel : BasePanel
{
    //实例化主卡牌槽的原始位置（基础牌+组合牌）
    public RectTransform originMainPos;

    //实例化副卡牌槽的原始位置（部首)
    public RectTransform radicalPosXi;
    public RectTransform radicalPosYe;
    public RectTransform radicalPosKe;
    public RectTransform radicalPosPi;

    //卡牌槽的对象引用
    public Radical_Xi slotXi;
    public Radical_Ye slotYe;
    public Radical_Ke slotKe;
    public Radical_Pi slotPi;


    //控制面板置灰的组件
    [HideInInspector]
    public CanvasGroup canvasGroup;
    //格子布局更新事件回调
    [HideInInspector]
    public GridLayoutCallback mainCallBack;
    [HideInInspector]
    public GridLayoutCallback minorCallBack;

    //部首牌显示数量控件
    public TMP_Text textCountXi;
    public TMP_Text textCountYe;
    public TMP_Text textCountKe;
    public TMP_Text textCountPi;

    public RectTransform tempRadicalStartPos;
    


    protected override void Awake()
    {
        mainCallBack = this.GetComponentInChildren<GridLayoutCallback>();
        if (mainCallBack == null)

            canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
       
        base.Awake();



    }
    protected override void ButtonClick(string name)
    {
        base.ButtonClick(name);
        switch (name)
        {
            case "btnOverMyTurn":
                HandleOverMyTurn();
                break;
            case "btnAddXi":
                HandleAddXi();
                break;
            case "btnAddYe":
                HandleAddYe();
                break;
            case "btnAddKe":
                HandleAddKe();
                break;
            case "btnAddPi":
                HandleAddPi();
                break;
        }
    }

    private void HandleOverMyTurn()
    {
        Debug.Log("按钮点击结束回合");
        LevelStepMgr.Instance.machine.ChangeState(E_LevelState.MonsterTurn_EnterSettle);
    }

    #region 测试使用
    private void HandleAddXi()
    {
        Debug.Log($"[按钮添加部首牌]{DataCenter.Instance.cardResNameData.radical_xi}");
        BaseCard card = Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.radical_xi, 0, tempRadicalStartPos.transform);
        card.cardEffectControl.enabled = false;
        if (card is BaseRadicalCard cardXi)
        {
            cardXi.MoveAt(radicalPosXi);
        }
        else
        {
            Debug.LogError("身为部首牌但是无法里氏替换");
        }
    }

   


    private void HandleAddYe()
    {
        Debug.Log($"[按钮添加部首牌]{DataCenter.Instance.cardResNameData.radical_ye}");

        BaseCard card = Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.radical_ye, 0, tempRadicalStartPos.transform);
        card.cardEffectControl.enabled = false;

        if (card is BaseRadicalCard cardYe)
        {
            cardYe.MoveAt(radicalPosYe);
        }
        else
        {
            Debug.LogError("身为部首牌但是无法里氏替换");
        }
    }
    private void HandleAddKe()
    {
        Debug.Log($"[按钮添加部首牌]{DataCenter.Instance.cardResNameData.radical_ke}");

        BaseCard card = Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.radical_ke, 0, tempRadicalStartPos.transform);
        card.cardEffectControl.enabled = false;

        if (card is BaseRadicalCard cardKe)
        {
            cardKe.MoveAt(radicalPosKe);
        }
        else
        {
            Debug.LogError("身为部首牌但是无法里氏替换");
        }
    }
    private void HandleAddPi()
    {
        Debug.Log($"[按钮添加部首牌]{DataCenter.Instance.cardResNameData.radical_pi}");
        BaseCard card = Dealer.Instance.CreateAndAddCard(DataCenter.Instance.cardResNameData.radical_pi, 0, tempRadicalStartPos.transform);
        card.cardEffectControl.enabled = false;

        if (card is BaseRadicalCard cardPi)
        {
            cardPi.MoveAt(radicalPosPi);
        }
        else
        {
            Debug.LogError("身为部首牌但是无法里氏替换");
        }
    }
    #endregion
    // 外部调用（怪物死亡时调用），参数：怪物的Transform（生成卡牌的起始位置）
    private void SpawnDropCard(string cardResName, RectTransform targetPos, Vector3 monsterWorldPos)
    {
        
        BaseCard card = Dealer.Instance.CreateAndAddCard(cardResName, 0, tempRadicalStartPos);
        card.cardEffectControl.enabled = false;

        
        if (card.TryGetComponent<RectTransform>(out var cardRect))
        {
            Canvas canvas = tempRadicalStartPos.GetComponentInParent<Canvas>();
            Camera mainCamera = Camera.main;
            Debug.Log($"【怪物世界坐标】: {monsterWorldPos}");
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, monsterWorldPos);
            // 改用Canvas根节点做局部坐标转换
            Debug.Log($"【怪物对应屏幕坐标】: {screenPos}");
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform,
                screenPos,
                canvas.worldCamera,
                out Vector2 localPos
            );
           
            Debug.Log($"【卡牌要设置的UI坐标】: {localPos}");
            cardRect.anchoredPosition = localPos;
            
            Debug.Log($"【卡牌实际生成坐标】: {cardRect.anchoredPosition}");
            LayoutRebuilder.ForceRebuildLayoutImmediate(cardRect);
            Debug.Log($"屏幕分辨率：{Screen.width} x {Screen.height}");
            Debug.Log($"怪物屏幕坐标是否在屏幕内：{screenPos.x < Screen.width && screenPos.y < Screen.height}");
            cardRect.SetParent(canvas.transform, false);
        }

        
        if (card is BaseRadicalCard radicalCard)
        {
            radicalCard.MoveAt(targetPos);
        }
        else
        {
            Debug.LogError("身为部首牌但是无法里氏替换");
        }
    }


    public void DropRandomRadicalCard(Vector3 monsterDeathPos)
    {
        //if (Random.value <= 0.5f)
        if (true)//测试用
        {
            int random = Random.Range(0, 4);
            switch (random)
            {
                case 0:
                    SpawnDropCard(DataCenter.Instance.cardResNameData.radical_xi, radicalPosXi as RectTransform, monsterDeathPos);
                    break;
                case 1:
                    SpawnDropCard(DataCenter.Instance.cardResNameData.radical_ye, radicalPosYe as RectTransform, monsterDeathPos);
                    break;
                case 2:
                    SpawnDropCard(DataCenter.Instance.cardResNameData.radical_ke, radicalPosKe as RectTransform, monsterDeathPos);
                    break;
                case 3:
                    SpawnDropCard(DataCenter.Instance.cardResNameData.radical_pi, radicalPosPi as RectTransform, monsterDeathPos);
                    break;
            }
        }
    }



    /// <summary>
    /// 面板进入置灰状态，只能看见不能被点击
    /// </summary>
    public void EnterAsh()
    {

        Debug.Log("�����Ʋ�����塿�����û�״̬");
        if(canvasGroup != null)

     
        

        canvasGroup.blocksRaycasts = false;           
    }

    /// <summary>
    /// 退出置灰状态
    /// </summary>
    public void ExitAsh()
    {

        Debug.Log("�����Ʋ�����塿�˳��û�״̬");
        if (canvasGroup != null)
            canvasGroup.blocksRaycasts = true;

    }

    /// <summary>
    /// 更新部首牌的UI显示数量（在CardOperateState有拾取部首牌数量记录）
    /// </summary>
    /// <param name="type">哪张部首牌</param>
    /// <param name="count">目前的数量为多少</param>
    public void UpdateRadicalCount(E_RadicalCardType type,int count)
    {
        string result = count.ToString();

        switch (type)
        {
            case E_RadicalCardType.Xi:
                textCountXi.text = result;
                break;
            case E_RadicalCardType.Ye:
                textCountYe.text = result;
                break;
            case E_RadicalCardType.Ke:
                textCountKe.text = result;
                break;
            case E_RadicalCardType.Pi:
                textCountPi.text = result;
                break;
        }
    }

    /// <summary>
    /// 将所有部首牌的UI显示数量归零
    /// </summary>
    public void RemoveAllRadicalCard()
    {
        textCountXi.text = "0";
        textCountYe.text = "0";
        textCountKe.text = "0";
        textCountPi.text = "0";
    }


}
