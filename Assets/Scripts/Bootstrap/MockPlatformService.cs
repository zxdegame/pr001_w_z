using Cysharp.Threading.Tasks;

/// <summary>
/// IPlatformService 的 Phase 0 Mock。防沉迷后置版号阶段，IsAntiAddictionEnabled 恒 false。
/// 真源：Architecture §4.13。
/// </summary>
public class MockPlatformService : IPlatformService
{
    /// <inheritdoc />
    public UniTask<bool> LoginAsync() => UniTask.FromResult(true);

    /// <inheritdoc />
    public bool IsAntiAddictionEnabled => false;   // 无版号阶段恒 false

    /// <inheritdoc />
    public UniTask ReportToAntiAddictionAsync(string playerId, int onlineMinutes) => UniTask.CompletedTask;

    /// <inheritdoc />
    public bool IsWithinAllowedPlayWindow() => true;   // 防沉迷后置，默认放行
}
