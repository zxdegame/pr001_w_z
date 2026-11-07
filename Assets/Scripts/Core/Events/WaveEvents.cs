/// <summary>波次开始（瞭望台预警）。发布者：WaveController；订阅者：UI。真源：Architecture §5。</summary>
public struct WaveStartedEvent : IEvent
{
    /// <summary>波次编号。</summary>
    public int waveNumber;

    /// <summary>本波小怪兽总数。</summary>
    public int totalZombies;
}

/// <summary>波次完成。发布者：WaveController；订阅者：BattleFlow。真源：Architecture §5。</summary>
public struct WaveCompletedEvent : IEvent
{
    /// <summary>波次编号。</summary>
    public int waveNumber;

    /// <summary>结算时剩余耐久。</summary>
    public int remainingDurability;
}
