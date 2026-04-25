using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComboViewControl : MonoBehaviour
{
    public TMP_Text textCombo;
    public TMP_Text textStageMultiple;

    public Animator animator;
    public Animator inkBKAnimator;


    //连击奖励提示位置
    public RectTransform rewardTip;


    private void OnEnable()
    {
        UpdateComboView(0,null);
        if(animator == null)
            Debug.LogError("没有检测到 Animator 组件");    
    }
    

    public void UpdateComboView(int comboCount,ComboData data)
    {
 
        if (comboCount > 0)
        {
            PlayComboAnim();
            textCombo.text = comboCount.ToString() + " 连!";
            textCombo.gameObject.SetActive(true);

            switch (data.cardElement)
            {
                case E_Element.Wood:   // 木 → 墨青/苍色（国风水墨木色）
                    textCombo.color = new Color(0.22f, 0.42f, 0.33f);
                    break;

                case E_Element.Fire:  // 火 → 朱砂红/赭红（国风不刺眼红）
                    textCombo.color = new Color(0.65f, 0.16f, 0.12f);
                    break;

                case E_Element.Earth: // 土 → 赭石/土黄（宣纸棕黄）
                    textCombo.color = new Color(0.55f, 0.42f, 0.27f);
                    break;

                case E_Element.Water: // 水 → 花青/墨蓝（水墨蓝）
                    textCombo.color = new Color(0.18f, 0.28f, 0.45f);
                    break;
            }
        }
        else
        {
            textCombo.gameObject.SetActive(false);
        }
    }

    public void UpdateStateMultipleView(int comboCount)
    {
        Debug.Log("[阶段倍数]更新打牌面板的阶段倍数，当前连击数为" + comboCount);
        if (comboCount > 0)
        {
            Debug.Log("当前连击数" + comboCount);



            if (comboCount <= 1)
                return;

            if(comboCount == 2)
            {
                PlayInkComboAnim();
                Debug.Log("[阶段倍数]播放动画");

            }
            else if((comboCount - 1) % 3 == 0)
            {
                PlayInkComboAnim();
                Debug.Log("[阶段倍数]播放动画");
            }


            int multiple = (comboCount-1) / 3 + 1;
   
            if (multiple > 3)
                multiple = 3;
            textStageMultiple.text = "×"+multiple.ToString() + "笔墨";
            textStageMultiple.gameObject.SetActive(true);

        }
        else
        {
            textStageMultiple.gameObject.SetActive(false);
        }

    }


    private void PlayComboAnim()
    {
        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("CombView_CombText"))
            animator.Play("CombView_CombText", 0, 0f);
        else
            animator.SetTrigger("Play");
    }

    private void PlayInkComboAnim()
    {
        // 获取当前动画状态信息
        AnimatorStateInfo stateInfo = inkBKAnimator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("CombView_InkMultiple"))
            inkBKAnimator.Play("CombView_InkMultiple", 0, 0f);
        else
            inkBKAnimator.SetTrigger("Play");
    }



    public void PlayReWardAnim(int value)
    {

        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/DamageTextImage"), rewardTip);
        obj.name = "rewardTip";
        if (obj == null)
        {
            Debug.LogError("无法加载 DamageTextImage 预制体");
            return;
        }
        obj.transform.localPosition = Vector3.zero; // 确保实例化的对象在父对象的中心位置
        obj.transform.localScale = Vector3.one * 10; // 确保实例化的对象缩放正常
        DamageTextImage text = obj.GetComponent<DamageTextImage>();
        if (text == null)
            {
            Debug.LogError("DamageTextImage 组件未找到");
            return;
        }
        Debug.Log("播放奖励动画");
        text.Init(value, Color.black, Vector2.zero, "笔墨+");
    }
}
