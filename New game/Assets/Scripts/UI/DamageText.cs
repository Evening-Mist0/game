using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 掉血数值文本 - 负责动画和自动销毁
/// </summary>
public class DamageText : MonoBehaviour
{
    [Header("动画参数")]
    public float moveSpeed = 60f;    // 向上移动速度
    public float fadeTime = 1f;      // 渐隐时间
    public float lifeTime = 1.5f;    // 总显示时长

    private Text damageText;         // 显示数值的Text组件
    private CanvasGroup canvasGroup; // 用于渐隐

    // 初始化掉血文本
    public void Init(int damage, Color color, Vector2 startPos)
    {
        // 获取组件（自动添加缺失的CanvasGroup）
        damageText = GetComponent<Text>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();

        // 设置数值、颜色、初始位置
        damageText.text = $"-{damage}";
        damageText.color = color;
        GetComponent<RectTransform>().anchoredPosition = startPos;

        // 启动动画协程
        StartCoroutine(PlayAnimation());
    }

    // 播放移动+渐隐动画
    private IEnumerator PlayAnimation()
    {
        float elapsedTime = 0f;
        Vector2 startPos = GetComponent<RectTransform>().anchoredPosition;

        while (elapsedTime < lifeTime)
        {
            // 1. 向上移动
            Vector2 newPos = startPos + new Vector2(0, moveSpeed * elapsedTime);
            GetComponent<RectTransform>().anchoredPosition = newPos;

            // 2. 最后0.5秒开始渐隐
            if (elapsedTime > lifeTime - fadeTime)
            {
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, (elapsedTime - (lifeTime - fadeTime)) / fadeTime);
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 动画结束后销毁
        Destroy(gameObject);
    }
}