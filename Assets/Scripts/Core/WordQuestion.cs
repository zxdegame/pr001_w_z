using System;

/// <summary>
/// 一道题目：目标词 + 题型 + 4 选项 + 正确索引 + 时限。
/// 真源：Architecture §4.4。
/// </summary>
[Serializable]
public class WordQuestion
{
    /// <summary>本题考查的目标词。</summary>
    public Word target;

    /// <summary>题型（英译中 / 中译英）。</summary>
    public QuestionType type;

    /// <summary>4 个选项词。</summary>
    public Word[] options;

    /// <summary>正确选项索引。</summary>
    public int correctIndex;

    /// <summary>本题时限（秒）。</summary>
    public float timeLimit;
}
