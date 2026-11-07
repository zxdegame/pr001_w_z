using System;

/// <summary>
/// 三会自主学习服务抽象。Phase 0-3 为 NoOp 实现；Phase 4+ 填充真实逻辑。
/// 不接入战斗流程，不影响 QuizStateMachine（GDD D4）。
/// 真源：Architecture §4.10。
/// </summary>
public interface IWordStudyService
{
    /// <summary>NoOp 实现恒返 false。</summary>
    bool IsEnabled { get; }

    /// <summary>三模式各自星级/进度，无数据时返回全零。</summary>
    StudyProgress GetProgress(int wordId);

    /// <summary>提交一次学习结果。</summary>
    bool SubmitResult(int wordId, StudyMode mode, bool correct);

    /// <summary>三会均通过（Phase 4+ 才会发布）。</summary>
    event Action<WordStudyCompletedEvent> OnWordFullyStudied;
}

/// <summary>三会模式：会读 / 会认 / 会写。真源：Architecture §4.10。</summary>
public enum StudyMode
{
    /// <summary>会读。</summary>
    Read = 0,

    /// <summary>会认。</summary>
    Recognize = 1,

    /// <summary>会写。</summary>
    Write = 2
}

/// <summary>三会进度（各模式星级 0-5）。真源：Architecture §4.10。</summary>
[Serializable]
public struct StudyProgress
{
    /// <summary>会读星级 0-5。</summary>
    public int readStars;

    /// <summary>会认星级 0-5。</summary>
    public int recognizeStars;

    /// <summary>会写星级 0-5。</summary>
    public int writeStars;
}
