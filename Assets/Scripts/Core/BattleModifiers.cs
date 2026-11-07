/// <summary>
/// 局内运行时加成容器（GDD D21：仅本局生效、不写存档，与 BattleDoctrine 同结构）。
/// 真源：Architecture §3。
/// </summary>
public class BattleModifiers
{
    /// <summary>攻击力倍率（Doctrine Fire → 1.2）。</summary>
    public float attackMultiplier = 1f;

    /// <summary>能量墙耐久倍率（Doctrine Bulwark → 1.3，守 D9，仅本局）。</summary>
    public float wallDurabilityMultiplier = 1f;

    /// <summary>
    /// Swift 阈值加成（D21 M3：Mastery Meter ≥60% → +0.5s，SwiftThresholdSeconds 3.0→3.5）。
    /// 仅动触发节奏，不动伤害。
    /// </summary>
    public float swiftThresholdBonus = 0f;

    /// <summary>
    /// 连击窗口加成（D21 M3：Mastery Meter ≥80% → +0.5s，ComboWindowSeconds 4.0→4.5）。
    /// 仅动触发节奏，不动伤害。
    /// </summary>
    public float comboWindowBonus = 0f;
}
