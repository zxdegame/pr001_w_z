/// <summary>某词首次达到 Mastered（D21 M2 首克爆字+额外晶）。发布者：Learning；订阅者：Reward, UI。真源：Architecture §5。</summary>
public struct FirstWordMasteredEvent : IEvent
{
    /// <summary>单词 ID。</summary>
    public int wordId;
}

/// <summary>某错题被攻克（D21 M2 攻克错题爆字+额外晶）。发布者：Learning；订阅者：Reward, UI。真源：Architecture §5。</summary>
public struct WrongWordFixedEvent : IEvent
{
    /// <summary>单词 ID。</summary>
    public int wordId;
}
