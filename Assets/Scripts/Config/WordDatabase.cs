using UnityEngine;

/// <summary>
/// 词库 ScriptableObject（按教材+年级分资产）。
/// TODO(RFC)：Phase 0 仅提供编译所需最小字段；真实结构（分组/单元索引/元数据引用）随主策划词库出表于 Phase 1+ 补全。
/// 真源：Architecture §4.3 使用点 / §15.2 单词数据加载方案。
/// </summary>
[CreateAssetMenu(menuName = "Refuge/Word Database", fileName = "WordDatabase")]
public class WordDatabase : ScriptableObject
{
    /// <summary>教材键，如 "RenJiao_G3_U5"。</summary>
    public string textbookKey;

    /// <summary>本词库全部单词（运行时只读，不 new）。</summary>
    public Word[] words;
}
