/// <summary>能量墙受击。发布者：Wall；订阅者：UI。真源：Architecture §5。</summary>
public struct WallDamagedEvent : IEvent
{
    /// <summary>本次耐久变化量。</summary>
    public int deltaDurability;

    /// <summary>当前耐久。</summary>
    public int currentDurability;
}
