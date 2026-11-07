using System.Collections.Generic;
using NUnit.Framework;

/// <summary>
/// EventBus 契约测试（真源 Architecture §5.1 / §17）。覆盖：发布-订阅投递、句柄 Dispose 退订、owner 销毁无泄漏。
/// EventBus 为静态总线，用每例独立 owner + TearDown UnsubscribeAll 保证用例隔离。
/// </summary>
public class EventBusTests
{
    private struct TestEvent : IEvent
    {
        public int Value;
    }

    private object owner;
    private List<int> received;

    [SetUp]
    public void Setup()
    {
        owner = new object();
        received = new List<int>();
    }

    [TearDown]
    public void TearDown()
    {
        EventBus.UnsubscribeAll(owner);
    }

    [Test]
    public void Subscribe_And_Publish_Delivers_To_Handler()
    {
        EventBus.Subscribe<TestEvent>(e => received.Add(e.Value), owner);

        EventBus.Publish(new TestEvent { Value = 5 });

        Assert.AreEqual(1, received.Count);
        Assert.AreEqual(5, received[0]);
    }

    [Test]
    public void Subscription_Dispose_Stops_Delivery()
    {
        var sub = EventBus.Subscribe<TestEvent>(e => received.Add(e.Value), owner);
        sub.Dispose();

        EventBus.Publish(new TestEvent { Value = 5 });

        Assert.IsEmpty(received);
    }

    [Test]
    public void EventBus_WhenOwnerDestroyed_NoMemoryLeak()
    {
        EventBus.Subscribe<TestEvent>(e => received.Add(e.Value), owner);

        // 模拟 MonoBehaviour.OnDestroy 中的批量清理
        EventBus.UnsubscribeAll(owner);

        EventBus.Publish(new TestEvent { Value = 5 });

        Assert.IsEmpty(received);
    }
}
