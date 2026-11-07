/// <summary>词池偏低（触发补充）。发布者：WordPool；订阅者：WordReplenisher。真源：Architecture §5。</summary>
public struct WordPoolLowEvent : IEvent
{
    /// <summary>当前剩余数量。</summary>
    public int remainingCount;
}

/// <summary>词池已补充。发布者：WordReplenisher；订阅者：Quiz。真源：Architecture §5。</summary>
public struct WordReplenishedEvent : IEvent
{
    /// <summary>本次新增数量。</summary>
    public int addedCount;

    /// <summary>补充后总数。</summary>
    public int newTotal;
}
