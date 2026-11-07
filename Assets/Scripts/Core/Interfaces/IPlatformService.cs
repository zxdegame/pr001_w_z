using Cysharp.Threading.Tasks;

/// <summary>
/// 平台能力桥接：登录 / 防沉迷时长时段 / 合规开关。
/// 本版为骨架 + Mock；防沉迷真实接入后置至版号后。
/// 真源：Architecture §4.13。
/// </summary>
public interface IPlatformService
{
    /// <summary>国内渠道登录（版号后接入具体 SDK）。</summary>
    UniTask<bool> LoginAsync();

    /// <summary>无版号阶段恒 false（防沉迷后置）。</summary>
    bool IsAntiAddictionEnabled { get; }

    /// <summary>上报防沉迷（版号后接入国家实名系统）。</summary>
    UniTask ReportToAntiAddictionAsync(string playerId, int onlineMinutes);

    /// <summary>是否在允许游玩时段内（未成年仅周五六日 20-21 点，版号后生效）。</summary>
    bool IsWithinAllowedPlayWindow();
}
