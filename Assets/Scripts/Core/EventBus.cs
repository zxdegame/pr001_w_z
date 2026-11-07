using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 全局事件总线（自研，真源 Architecture §5.1 主程确认方案）。
/// 结构 Type → 订阅者列表；Publish 倒序遍历以支持回调内取消订阅；owner 销毁须调 UnsubscribeAll 防内存泄漏。
/// 约定：每个 MonoBehaviour 在 OnDestroy 中调用 <see cref="UnsubscribeAll"/>。
/// </summary>
public static class EventBus
{
    static readonly Dictionary<Type, List<HandlerEntry>> handlers = new Dictionary<Type, List<HandlerEntry>>();
    static readonly object lockObj = new object();

    /// <summary>订阅事件。owner 为空时以 handler.Target 作为归属，用于销毁时批量清理。</summary>
    /// <typeparam name="T">事件类型（须实现 <see cref="IEvent"/>）。</typeparam>
    /// <param name="handler">处理回调。</param>
    /// <param name="owner">归属对象（一般传 this）。</param>
    /// <returns>可 Dispose 的订阅句柄。</returns>
    public static Subscription<T> Subscribe<T>(Action<T> handler, object owner = null) where T : IEvent
    {
        if (handler == null) throw new ArgumentNullException(nameof(handler));
        var type = typeof(T);
        lock (lockObj)
        {
            if (!handlers.TryGetValue(type, out var list))
            {
                list = new List<HandlerEntry>();
                handlers[type] = list;
            }
            list.Add(new HandlerEntry(handler, owner ?? handler.Target));
        }
        return new Subscription<T>(handler, type);
    }

    /// <summary>发布事件；倒序遍历，单个订阅者异常经 Debug.LogException 记录且不影响其余订阅者。</summary>
    /// <typeparam name="T">事件类型。</typeparam>
    /// <param name="evt">事件数据。</param>
    public static void Publish<T>(T evt) where T : IEvent
    {
        if (!handlers.TryGetValue(typeof(T), out var list) || list.Count == 0) return;
        for (int i = list.Count - 1; i >= 0; i--)
        {
            try
            {
                ((Action<T>)list[i].Handler)?.Invoke(evt);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    /// <summary>清理某 owner 的全部订阅；MonoBehaviour 须在 OnDestroy 调用（内存安全约定）。</summary>
    /// <param name="owner">订阅时登记的归属对象。</param>
    public static void UnsubscribeAll(object owner)
    {
        if (owner == null) return;
        lock (lockObj)
        {
            foreach (var list in handlers.Values)
            {
                list.RemoveAll(h => ReferenceEquals(h.Owner, owner));
            }
        }
    }

    internal static void Remove(Type type, Delegate handler)
    {
        lock (lockObj)
        {
            if (handlers.TryGetValue(type, out var list))
            {
                list.RemoveAll(h => h.Handler == handler);
            }
        }
    }

    struct HandlerEntry
    {
        public readonly Delegate Handler;
        public readonly object Owner;
        public HandlerEntry(Delegate handler, object owner) { Handler = handler; Owner = owner; }
    }
}

/// <summary>订阅句柄；Dispose 时从总线移除对应处理器（幂等）。</summary>
/// <typeparam name="T">事件类型。</typeparam>
public sealed class Subscription<T> : IDisposable where T : IEvent
{
    readonly Action<T> _handler;
    readonly Type _type;
    bool _disposed;

    internal Subscription(Action<T> handler, Type type)
    {
        _handler = handler;
        _type = type;
    }

    /// <summary>取消本订阅。</summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        EventBus.Remove(_type, _handler);
    }
}
