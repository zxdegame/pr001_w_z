/// <summary>
/// 事件波类型（GDD §12 D18）。挂在 WaveConfig.eventType。
/// </summary>
public enum WaveEventType
{
    /// <summary>普通波。</summary>
    None = 0,

    /// <summary> Rush 突袭波（Phase3）。</summary>
    Rush,

    /// <summary>BOSS 压力波（Phase3）。</summary>
    BossPressure,

    /// <summary>伏击波（Phase4，需 ZombieController 支持反向推进）。</summary>
    Ambush
}
