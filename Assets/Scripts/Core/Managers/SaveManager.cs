using UnityEngine;

/// <summary>
/// 全局单例：负责 PlayerData、WordMastery、WrongWords 的加载与保存。
/// 触发保存：关卡结算后、应用退出前、每 60 秒自动保存。
/// 真源：Architecture §6.2 / §9。Phase 0 骨架，序列化实现（JSON/存档路径）于 Phase 1+ 接入。
/// </summary>
public class SaveManager : MonoBehaviour
{
    /// <summary>全局唯一实例。</summary>
    public static SaveManager Instance { get; private set; }

    /// <summary>当前存档数据。</summary>
    public PlayerData Current { get; private set; }

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

    /// <summary>加载存档（Phase 1+ 实现）。</summary>
    public void Load() => throw new System.NotImplementedException();

    /// <summary>保存存档（Phase 1+ 实现）。</summary>
    public void Save() => throw new System.NotImplementedException();
}
