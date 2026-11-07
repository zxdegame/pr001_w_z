using UnityEngine;

/// <summary>
/// 全局单例（跨场景）：负责全局状态、Time.timeScale 管理。
/// 生命周期：RuntimeInitializeOnLoadMethod 创建，DontDestroyOnLoad。
/// 真源：Architecture §6.1。Phase 0 骨架，场景切换编排于 Phase 1+ 接入。
/// </summary>
public class GameManager : MonoBehaviour
{
    /// <summary>全局唯一实例。</summary>
    public static GameManager Instance { get; private set; }

    /// <summary>全局战斗状态（真源 §3 BattleState）。</summary>
    public BattleState CurrentState { get; private set; } = BattleState.Loading;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance != null) return;
        var go = new GameObject(nameof(GameManager));
        go.AddComponent<GameManager>();
    }

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
}
