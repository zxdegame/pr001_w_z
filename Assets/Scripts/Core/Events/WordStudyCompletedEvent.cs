/// <summary>
/// 三会全部通过事件（Phase 4+ 才发布）。
/// TODO(RFC)：Phase 0 仅编译占位（IWordStudyService 事件参数所需）；字段随三会系统正式出表补全，不在 §5 事件清单内。
/// </summary>
public struct WordStudyCompletedEvent : IEvent
{
    /// <summary>完成的单词 ID。</summary>
    public int wordId;
}
