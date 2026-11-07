/// <summary>
/// 单词掌握度四态。真源 Architecture §3 / GDD 学习系统。
/// D21 M1：Phase 1 若 memoryStrength 无连续量，则由本四态兜底映射 0 / 0.5 / 0.8 / 1.0（仅表现、不参与伤害/结算）。
/// </summary>
public enum WordMasteryLevel
{
    /// <summary>未学习。</summary>
    Unlearned,

    /// <summary>学习中。</summary>
    Learning,

    /// <summary>已掌握（本局达成）。</summary>
    Mastered,

    /// <summary>长期掌握（跨局保持）。</summary>
    LongTermMastered
}
