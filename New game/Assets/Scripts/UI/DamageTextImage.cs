using UnityEngine;
using TMPro; // 必须引用TMP命名空间
using System.Collections;

/// <summary>
/// 掉血数值文本 - 负责动画和自动销毁（适配 TextMeshPro）
/// </summary>
public class DamageTextImage : MonoBehaviour
{
    [Header("动画参数")]
    public float moveSpeed = 60f;    // 向上移动速度
    public float fadeTime = 1f;      // 渐隐时间
    public float lifeTime = 1.5f;    // 总显示时长

    // 缓存引用：改为 TMP_Text
    private TMP_Text damageText;
    private CanvasGroup canvasGroup;
    private RectTransform rect;

    void Awake()
    {
        TryGetComponent<TMP_Text>(out damageText);
        TryGetComponent<CanvasGroup>(out canvasGroup);
        TryGetComponent<RectTransform>(out rect);

        // 自动添加缺失的 TMP_Text，而不是旧版 Text
        if (damageText == null) damageText = gameObject.AddComponent<TMP_Text>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        if (rect == null) rect = gameObject.AddComponent<RectTransform>();
    }

    /// <summary>
    /// 外部调用初始化
    /// </summary>
    public void Init(int damage, Color color, Vector2 startPos, string ExtraContent = "-")
    {
        Debug.Log("数字初始化");
        // 重置状态
        canvasGroup.alpha = 1f;

        // 设置数值和颜色
        damageText.text = $"{ExtraContent}{damage}";
        damageText.color = color;

        // 设置位置
        rect.anchoredPosition = startPos;

        // 启动动画
        StopAllCoroutines();
        StartCoroutine(PlayAnimation());
    }

    /// <summary>
    /// 播放动画
    /// </summary>
    private IEnumerator PlayAnimation()
    {
        float elapsedTime = 0f;
        Vector2 startPos = rect.anchoredPosition;

        while (elapsedTime < lifeTime)
        {
            // 这里的 moveSpeed 单位是像素/秒，乘以 deltaTime 更准确
            Vector2 newPos = startPos + new Vector2(0, moveSpeed * elapsedTime);
            rect.anchoredPosition = newPos;

            //// 渐隐效果
            //if (elapsedTime > lifeTime - fadeTime)
            //{
            //    canvasGroup.alpha = Mathf.Lerp(1f, 0f, (elapsedTime - (lifeTime - fadeTime)) / fadeTime);
            //}

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Destroy(gameObject);
    }
}