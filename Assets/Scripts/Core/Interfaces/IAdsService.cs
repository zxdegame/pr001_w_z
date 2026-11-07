using Cysharp.Threading.Tasks;

/// <summary>
/// 广告抽象服务：完全离线时返回 Mock；平台自带广告时包装接口。
/// 真源：Architecture §4.8（目标平台 Android + iOS；未成年人保护广告分级 Compliance C5）。
/// </summary>
public interface IAdsService
{
    /// <summary>当前环境能否展示广告。</summary>
    bool IsAvailable { get; }

    /// <summary>展示激励视频，返回是否完成观看。</summary>
    UniTask<bool> ShowRewardedAsync(string placementId);

    /// <summary>展示插屏广告。</summary>
    UniTask ShowInterstitialAsync(string placementId);

    /// <summary>未成年人保护：单局频率上限（成年建议≤3/未成年≤1）。</summary>
    bool CanShowInThisSession { get; }
}
