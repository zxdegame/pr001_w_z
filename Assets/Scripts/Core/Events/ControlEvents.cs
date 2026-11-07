/// <summary>暂停请求。发布者：Input；订阅者：Time。真源：Architecture §5。</summary>
public struct PauseRequestedEvent : IEvent
{
    /// <summary>目标暂停状态。</summary>
    public bool isPaused;
}

/// <summary>退出请求（无字段）。发布者：Input；订阅者：BattleFlow。真源：Architecture §5。</summary>
public struct QuitRequestedEvent : IEvent
{
}
