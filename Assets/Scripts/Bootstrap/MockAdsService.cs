using Cysharp.Threading.Tasks;

/// <summary>
/// IAdsService 的默认无网/编辑器/测试实现（不展示任何广告）。
/// 真源：Architecture §4.8 / §17。
/// </summary>
public class MockAdsService : IAdsService
{
    /// <inheritdoc />
    public bool IsAvailable => false;

    /// <inheritdoc />
    public UniTask<bool> ShowRewardedAsync(string placementId) => UniTask.FromResult(false);

    /// <inheritdoc />
    public UniTask ShowInterstitialAsync(string placementId) => UniTask.CompletedTask;

    /// <inheritdoc />
    public bool CanShowInThisSession => false;
}
