/// <summary>战斗开始。发布者：BattleFlow；订阅者：所有系统。真源：Architecture §5。</summary>
public struct BattleStartEvent : IEvent
{
    /// <summary>关卡 ID（如 "L101"）。</summary>
    public string levelId;

    /// <summary>本局初始词池规模。</summary>
    public int initialWordCount;
}

/// <summary>战斗结束。发布者：BattleFlow；订阅者：UI, Save。真源：Architecture §5。</summary>
public struct BattleEndEvent : IEvent
{
    /// <summary>战斗结果（Victory/Defeat/Quit）。</summary>
    public BattleState result;

    /// <summary>星级评定。</summary>
    public StarRating starRating;

    /// <summary>结算奖励。</summary>
    public RewardResult rewards;
}
