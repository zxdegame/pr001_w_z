using System;

/// <summary>
/// 单词定义（class，全局唯一引用）。
/// 仅通过 WordDatabase SO 加载，不在运行时 new。
/// 真源：Architecture §4.3。[Serializable] 为 SO 序列化所需（不改变字段契约）。
/// </summary>
[Serializable]
public class Word
{
    /// <summary>单词唯一 ID。</summary>
    public int id;

    /// <summary>英文拼写。</summary>
    public string english;

    /// <summary>中文释义。</summary>
    public string chinese;

    /// <summary>音标。</summary>
    public string phonetic;

    /// <summary>难度，取值 1-5。</summary>
    public int difficulty;

    /// <summary>掌握度四态。</summary>
    public WordMasteryLevel mastery;

    /// <summary>
    /// 记忆强度连续量 ∈[0,1]（GDD D21 M1，仅表现：头顶"上膛"进度）。
    /// Phase 1 无连续量则由 mastery 四态兜底；不参与伤害/答题结算。
    /// </summary>
    public float memoryStrength;

    /// <summary>教材键，如 "RenJiao_G3_U5"。</summary>
    public string textbookKey;

    /// <summary>年级。</summary>
    public int grade;

    /// <summary>单元。</summary>
    public int unit;
}
