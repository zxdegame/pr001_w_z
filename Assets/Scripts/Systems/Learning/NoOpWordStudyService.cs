using System;

#pragma warning disable CS0067 // Phase 0 NoOp：事件仅契约声明，三会逻辑 Phase 4+ 才发布

/// <summary>
/// 三会服务空实现：所有方法返回默认值、不写存档、不弹 UI，保证 Phase 4+ 接入不回改调用点（GDD D4）。
/// 真源：Architecture §4.10 / §17。
/// </summary>
public class NoOpWordStudyService : IWordStudyService
{
    /// <inheritdoc />
    public bool IsEnabled => false;

    /// <inheritdoc />
    public StudyProgress GetProgress(int wordId) => default;

    /// <inheritdoc />
    public bool SubmitResult(int wordId, StudyMode mode, bool correct) => false;

    /// <inheritdoc />
    public event Action<WordStudyCompletedEvent> OnWordFullyStudied;
}
