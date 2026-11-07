using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 埋点管理单例：对外暴露 IAnalyticsService 语义。三层 sink（LocalFile/StructuredEvent/RemoteUpload）实现于 Phase 1+ 接入。
/// 真源：Architecture §4.9 / §6。Phase 0 骨架，方法为空实现（不阻塞主循环）。
/// </summary>
public class AnalyticsManager : MonoBehaviour
{
    /// <summary>全局唯一实例。</summary>
    public static AnalyticsManager Instance { get; private set; }

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

    /// <summary>记录一个事件（Phase 1+ 落三层 sink）。</summary>
    public void LogEvent(string eventName, Dictionary<string, object> properties = null)
    {
        // TODO(Phase 1+)：写入 LocalFile/StructuredEvent sink。
    }

    /// <summary>记录一个错误（Phase 1+ 落三层 sink）。</summary>
    public void LogError(string tag, string message, Exception ex = null)
    {
        // TODO(Phase 1+)：错误上报。
    }

    /// <summary>强制写入本地日志（Phase 1+ 实现）。</summary>
    public void Flush()
    {
        // TODO(Phase 1+)：刷盘。
    }
}
