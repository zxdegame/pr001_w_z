using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 广告管理单例：转发到 IAdsService。Phase 0 默认使用 MockAdsService（无网/编辑器不展示）。
/// 真源：Architecture §4.8。真实 SDK（穿山甲/优量汇）于后续 Phase 接入。
/// </summary>
public class AdsManager : MonoBehaviour
{
    /// <summary>全局唯一实例。</summary>
    public static AdsManager Instance { get; private set; }

    /// <summary>当前广告实现（Phase 0 默认 Mock）。</summary>
    public IAdsService Service { get; private set; } = new MockAdsService();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>当前环境能否展示广告。</summary>
    public bool IsAvailable => Service.IsAvailable;

    /// <summary>展示激励视频，返回是否完成观看。</summary>
    public UniTask<bool> ShowRewardedAsync(string placementId) => Service.ShowRewardedAsync(placementId);

    /// <summary>展示插屏广告。</summary>
    public UniTask ShowInterstitialAsync(string placementId) => Service.ShowInterstitialAsync(placementId);
}
