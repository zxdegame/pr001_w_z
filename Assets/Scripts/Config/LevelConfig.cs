using UnityEngine;

/// <summary>
/// 关卡配置 SO（5 波 + 词库 + 目标）。仅字段、无逻辑。
/// 真源：DevTasks Config 层 / Levels.md §1.1 / TechStack §SO 表（GDD D18/D19）。
/// TODO(RFC)：starThresholds / firstClearRewardExtra 结构型字段随星级·经济出表补全（当前走全局统一值）。
/// 数值一律留空，由 Editor 资产按 Levels.md / GDD 附录 D 填充（不写死）。
/// </summary>
[CreateAssetMenu(menuName = "Refuge/Level Config", fileName = "LevelConfig")]
public class LevelConfig : ScriptableObject
{
    /// <summary>关卡 ID，命名 L{章}{序}，如 "L101"。</summary>
    public string levelId;

    /// <summary>章节号 1/2/3。</summary>
    public int chapterId;

    /// <summary>总波数（标准关=5，教学关可少）。</summary>
    public int totalWaves;

    /// <summary>逐波配置。</summary>
    public WaveData[] waves;

    /// <summary>本关词库来源。</summary>
    public WordDatabase wordDatabase;

    /// <summary>初始词池规模（星级答对率分母基准之一）。</summary>
    public int initialWordPoolCount;

    /// <summary>词池来源比例：待复习（§4.1 默认 0.4）。</summary>
    public float reviewRatio;

    /// <summary>词池来源比例：错题本（§4.1 默认 0.3）。</summary>
    public float wrongBookRatio;

    /// <summary>词池来源比例：新学（§4.1 默认 0.2）。</summary>
    public float newLearnedRatio;

    /// <summary>词池来源比例：词池新词（§4.1 默认 0.1）。</summary>
    public float poolNewRatio;

    /// <summary>关卡开局战术三选一开关（教学/复习/错题关=false，GDD D18）。</summary>
    public bool doctrineEnabled;

    /// <summary>首通产出加成（第 1-3 章=1.3，replay 不叠加，GDD D19）。</summary>
    public float earlyChapterBonus;

    /// <summary>关卡目标文本（详情页展示）。</summary>
    public string[] objectives;
}
