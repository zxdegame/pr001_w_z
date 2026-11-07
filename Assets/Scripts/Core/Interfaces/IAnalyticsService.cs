using System;
using System.Collections.Generic;

/// <summary>
/// 埋点服务抽象：自研实现，未来可升级 ThinkingData/Firebase。
/// 真源：Architecture §4.9（三层 sink：LocalFile/StructuredEvent/RemoteUpload）。
/// </summary>
public interface IAnalyticsService
{
    /// <summary>记录一个事件。</summary>
    void LogEvent(string eventName, Dictionary<string, object> properties = null);

    /// <summary>记录一个错误。</summary>
    void LogError(string tag, string message, Exception ex = null);

    /// <summary>强制写入本地日志。</summary>
    void Flush();
}
