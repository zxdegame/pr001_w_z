/// <summary>
/// Mastery Meter 服务：本局已出题中 Mastered/LongTermMastered 词占比 ∈[0,1]，每出题刷新（GDD D21 M3）。
/// 仅动"触发节奏"（阈值/窗口），不动伤害（守 D8/D16）。
/// 真源：Architecture §3。
/// </summary>
public interface IMasteryMeterService
{
    /// <summary>本局掌握度百分比 ∈[0,1]。</summary>
    float Percent { get; }
}
