/// <summary>
/// 本版本地轻量成就分类（GDD §12 D19）。纯单机、读本地统计一次性发奖；区别于 Phase 4+ 三会/联网/每日成就（不违反 D4）。
/// </summary>
public enum LocalAchievementCategory
{
    /// <summary>击败类。</summary>
    Defeat,

    /// <summary>连击类。</summary>
    Combo,

    /// <summary>掌握类。</summary>
    Mastery,

    /// <summary>通关类。</summary>
    Clear,

    /// <summary>收集类。</summary>
    Collection
}
