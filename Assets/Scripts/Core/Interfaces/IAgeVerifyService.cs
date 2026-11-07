using Cysharp.Threading.Tasks;

/// <summary>
/// 年龄识别与家长同意验证抽象。Phase 0 提供 Mock 骨架（弹窗 + 二次确认），
/// Phase 2-3 接入运营商号码认证 SDK（客户端直连、不自建后端）。
/// 真源：Architecture §4.12（GDD D22 / Compliance C1）。
/// </summary>
public interface IAgeVerifyService
{
    /// <summary>是否已完成年龄/家长验证。</summary>
    bool IsVerified { get; }

    /// <summary>一键登录/短信验证码主流程。</summary>
    UniTask<AgeVerifyResult> VerifyAsync();

    /// <summary>判定是否未成年（驱动广告降级 C5）。</summary>
    bool IsMinor { get; }

    /// <summary>未成年是否取得家长同意（成年恒 true）。</summary>
    bool HasParentalConsent { get; }
}

/// <summary>年龄验证结果。真源：Architecture §4.12。</summary>
public struct AgeVerifyResult
{
    /// <summary>验证是否成功。</summary>
    public bool success;

    /// <summary>是否未成年。</summary>
    public bool isMinor;

    /// <summary>是否已取得家长同意。</summary>
    public bool parentalConsent;

    /// <summary>验证方式。</summary>
    public AgeVerifyMethod method;
}

/// <summary>年龄验证方式。真源：Architecture §4.12。</summary>
public enum AgeVerifyMethod
{
    /// <summary>运营商一键登录。</summary>
    CarrierOneClick,

    /// <summary>短信验证码。</summary>
    SmsCode,

    /// <summary>Phase 0 Mock 骨架。</summary>
    Mock
}
