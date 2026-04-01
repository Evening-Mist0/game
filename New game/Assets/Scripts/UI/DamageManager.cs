using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 掉血管理脚本 - 检测输入，模拟我方/敌方掉血
/// </summary>
public class DamageManager : MonoBehaviour
{
    [Header("配置项")]
    public GameObject damageTextPrefab; // 掉血文本预制体
    public Vector2 playerPos = new Vector2(100, 100); // 我方掉血显示位置（屏幕左下）
    public Vector2 enemyPos = new Vector2(1800, 800); // 敌方掉血显示位置（屏幕右上）
    public Color playerColor = Color.red; // 我方掉血颜色（红）
    public Color enemyColor = Color.yellow; // 敌方掉血颜色（黄）

    private void Update()
    {
        // 检测输入：按A键 → 我方随机掉血（1-10点）
        if (Input.GetKeyDown(KeyCode.A))
        {
            int damage = Random.Range(1, 11);
            ShowDamage(damage, true);
            Debug.Log($"我方掉血：{damage}点");
        }

        // 检测输入：按D键 → 敌方随机掉血（1-10点）
        if (Input.GetKeyDown(KeyCode.D))
        {
            int damage = Random.Range(1, 11);
            ShowDamage(damage, false);
            Debug.Log($"敌方掉血：{damage}点");
        }
    }

    /// <summary>
    /// 显示掉血数值
    /// </summary>
    /// <param name="damage">掉血数值</param>
    /// <param name="isPlayer">是否是我方掉血</param>
    private void ShowDamage(int damage, bool isPlayer)
    {
        // 1. 动态创建掉血文本
        GameObject textObj = Instantiate(damageTextPrefab, transform);
        textObj.transform.SetParent(GameObject.Find("Canvas").transform, false); // 挂到Canvas下

        // 2. 初始化文本
        DamageTextImage damageText = textObj.AddComponent<DamageTextImage>(); // 挂载核心脚本
        Vector2 showPos = isPlayer ? playerPos : enemyPos; // 选择显示位置
        damageText.Init(damage, isPlayer ? playerColor : enemyColor, showPos);
    }
}