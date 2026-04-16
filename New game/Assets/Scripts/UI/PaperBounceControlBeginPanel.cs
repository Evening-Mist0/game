using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;



public class PaperBounceControlBeginPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform rect;
    private Vector2 originalPos;
    private Vector3 originalScale;
    private Coroutine animCoroutine;

    // 手动定义弹动曲线
    private AnimationCurve bounceCurve = new AnimationCurve(
        new Keyframe(0, 0, 0, 5),    // 0秒：初始状态，切线陡
        new Keyframe(0.6f, 1, 0, -3) // 0.6秒：目标状态，切线向下（回弹）
    );

    // 新增可在Inspector中修改的变量
    [Header("弹开设置")]
    [Tooltip("弹开的水平偏移量")]
    public float bounceXOffset = 50f;
    [Tooltip("弹开的垂直偏移量")]
    public float bounceYOffset = 30f;
    [Tooltip("弹开的缩放比例增量")]
    public float bounceScaleIncrement = 0.1f;


    void Awake()
    {
        rect = GetComponent<RectTransform>();
        originalPos = rect.anchoredPosition;
        originalScale = rect.localScale;
    }


    private void OnEnable()
    {
        originalPos = rect.anchoredPosition;
        originalScale = rect.localScale;
    }

    // 鼠标进入按钮时触发
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine); // 停止已有动画
        animCoroutine = StartCoroutine(PlayBounceAndFloat());
    }

    // 鼠标离开按钮时触发
    public void OnPointerExit(PointerEventData eventData)
    {
        if (animCoroutine != null)
            StopCoroutine(animCoroutine);
        // 恢复初始状态
        rect.anchoredPosition = originalPos;
        rect.localScale = originalScale;
    }

    IEnumerator PlayBounceAndFloat()
    {
        // 弹开动画
        float time = 0;
        while (time < 0.6f)
        {
            float t = bounceCurve.Evaluate(time / 0.6f);
            rect.anchoredPosition = originalPos + new Vector2(bounceXOffset * t, bounceYOffset * t);
            rect.localScale = originalScale * (1 + bounceScaleIncrement * t);
            time += Time.deltaTime;
            yield return null;
        }
    }
}
