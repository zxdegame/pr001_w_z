using Cysharp.Threading.Tasks;

/// <summary>
/// IAgeVerifyService 的 Phase 0 Mock 骨架（弹窗 + 二次确认于 UI/合规接入时替换）。
/// 默认按保守口径返回"未成年 · 未同意"，真实号码认证 SDK 于 Phase 2-3 接入。
/// 真源：Architecture §4.12（GDD D22 / Compliance C1）。
/// </summary>
public class MockAgeVerifyService : IAgeVerifyService
{
    /// <inheritdoc />
    public bool IsVerified => false;

    /// <inheritdoc />
    public UniTask<AgeVerifyResult> VerifyAsync() => UniTask.FromResult(new AgeVerifyResult
    {
        success = false,
        isMinor = true,
        parentalConsent = false,
        method = AgeVerifyMethod.Mock
    });

    /// <inheritdoc />
    public bool IsMinor => true;

    /// <inheritdoc />
    public bool HasParentalConsent => false;
}
