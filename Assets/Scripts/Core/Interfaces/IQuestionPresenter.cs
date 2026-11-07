using System;

/// <summary>
/// Quiz 系统对 UI 层的抽象：状态机不直接依赖 UI，通过接口调用。
/// 真源：Architecture §4.4。
/// </summary>
public interface IQuestionPresenter
{
    /// <summary>展示一道题。</summary>
    void ShowQuestion(WordQuestion question);

    /// <summary>隐藏题目面板。</summary>
    void HideQuestion();

    /// <summary>答错时高亮正确选项。</summary>
    void ShowCorrectAnswerFeedback(WordQuestion q);

    /// <summary>播放嘲讽气泡。</summary>
    void ShowTauntBubble(string text, float duration);

    /// <summary>更新计时器显示。</summary>
    void UpdateTimer(float remaining, float total);

    /// <summary>玩家选择的索引 0-3。</summary>
    event Action<int> OnAnswerSubmitted;
}
