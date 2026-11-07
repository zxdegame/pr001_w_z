/// <summary>
/// 单词来源接口，QuestionLoader 从此获取单词，支持多种来源
/// （普通关卡词库 / 7 日复习 / 错题回溯 / 词池补充）。
/// 真源：Architecture §4.3。
/// </summary>
public interface IWordProvider
{
    /// <summary>按当前关卡策略返回下一个单词。</summary>
    Word GetNextWord();

    /// <summary>词池剩余数量。</summary>
    int RemainingCount { get; }

    /// <summary>初始词池规模（用于答对率分母，固定不变）。</summary>
    int InitialPoolCount { get; }

    /// <summary>是否需要补充词池。</summary>
    bool ShouldReplenish { get; }

    /// <summary>从词池移除指定单词。</summary>
    void RemoveWord(int wordId);

    /// <summary>将错词退回错题本。</summary>
    void ReturnToWrongBook(Word word);
}
