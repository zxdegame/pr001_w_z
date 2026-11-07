using System;

/// <summary>
/// 教材元数据（词库列表仅返回此轻量信息，不含单词正文）。
/// 真源：Architecture §4.3。
/// </summary>
[Serializable]
public class TextbookMetadata
{
    /// <summary>教材键，如 "RenJiao_G3_U5"。</summary>
    public string textbookKey;

    /// <summary>展示名，如 "人教版三年级"。</summary>
    public string displayName;

    /// <summary>年级。</summary>
    public int grade;

    /// <summary>教材版本，如 "人教版/外研社/..."。</summary>
    public string textbookVersion;

    /// <summary>该词库总词数。</summary>
    public int totalWordCount;
}
