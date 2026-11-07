using UnityEngine;

/// <summary>
/// 暂停管理单例：统一切换 Time.timeScale 并维护暂停态。
/// 真源：Architecture §6（PauseRequestedEvent 发布者 Input、订阅者 Time）。Phase 0 骨架。
/// </summary>
public class PauseManager : MonoBehaviour
{
    /// <summary>全局唯一实例。</summary>
    public static PauseManager Instance { get; private set; }

    /// <summary>当前是否暂停。</summary>
    public bool IsPaused { get; private set; }

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

    /// <summary>切换暂停（冻结/恢复 Time.timeScale）。</summary>
    public void SetPaused(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;
    }
}
