/// <summary>题目展示。发布者：Quiz；订阅者：UI。真源：Architecture §5。</summary>
public struct QuestionShownEvent : IEvent
{
    /// <summary>目标词。</summary>
    public Word word;

    /// <summary>4 个选项词。</summary>
    public Word[] options;

    /// <summary>正确选项索引。</summary>
    public int correctIndex;
}

/// <summary>答案提交。发布者：Quiz；订阅者：Learning。真源：Architecture §5。</summary>
public struct AnswerSubmittedEvent : IEvent
{
    /// <summary>玩家选择的索引。</summary>
    public int selectedIndex;

    /// <summary>是否答对。</summary>
    public bool isCorrect;

    /// <summary>本题考查词。</summary>
    public Word word;
}

/// <summary>答题超时。发布者：Quiz；订阅者：Learning。真源：Architecture §5。</summary>
public struct AnswerTimeoutEvent : IEvent
{
    /// <summary>本题考查词。</summary>
    public Word word;
}

/// <summary>连击变化。发布者：Quiz；订阅者：UI, Effects。真源：Architecture §5。</summary>
public struct ComboChangedEvent : IEvent
{
    /// <summary>新的连击数。</summary>
    public int newComboCount;

    /// <summary>是否触发连击 buff。</summary>
    public bool buffTriggered;
}
