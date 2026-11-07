using UnityEngine;

/// <summary>
/// 答题参数 SO。仅字段、无逻辑。
/// 真源：TechStack §SO 表 / QuizConfig（GDD D17）。数值由 Editor 资产填充。
/// </summary>
[CreateAssetMenu(menuName = "Refuge/Quiz Config", fileName = "QuizConfig")]
public class QuizConfig : ScriptableObject
{
    /// <summary>默认答题时限（秒，设置可调 10-20）。</summary>
    public float defaultTimeLimitSeconds;

    /// <summary>连击窗口（秒，GDD D17=4）。</summary>
    public float comboWindowSeconds;

    /// <summary>受击硬直（秒，仅表现层，GDD D17=0.3）。</summary>
    public float hitStunSeconds;

    /// <summary>Swift 阈值（秒，单题用时≤此值且答对触发，GDD D17=3.0）。</summary>
    public float swiftThresholdSeconds;

    /// <summary>Swift 伤害倍率（GDD D17=1.15，仍是"答对"暴击的乘数）。</summary>
    public float swiftDamageMultiplier;
}
