using UnityEngine;

/// <summary>小怪兽生成。发布者：WaveController；订阅者：Quiz。真源：Architecture §5。</summary>
public struct ZombieSpawnedEvent : IEvent
{
    /// <summary>实例 ID。</summary>
    public int zombieId;

    /// <summary>原型类型。</summary>
    public ZombieType type;

    /// <summary>生成位置。</summary>
    public Vector3 position;
}

/// <summary>小怪兽进入答题触发圈。发布者：TriggerZone；订阅者：Quiz。真源：Architecture §5。</summary>
public struct ZombieEnteredTriggerEvent : IEvent
{
    /// <summary>实例 ID。</summary>
    public int zombieId;
}

/// <summary>小怪兽被锁定为答题目标。发布者：Quiz；订阅者：UI。真源：Architecture §5。</summary>
public struct ZombieTargetLockedEvent : IEvent
{
    /// <summary>实例 ID。</summary>
    public int zombieId;
}

/// <summary>小怪兽受击（活力值变化，D8）。发布者：Zombie；订阅者：Reward, UI。真源：Architecture §5。</summary>
public struct ZombieDamagedEvent : IEvent
{
    /// <summary>实例 ID。</summary>
    public int zombieId;

    /// <summary>本次活力值变化量。</summary>
    public int deltaVitality;

    /// <summary>剩余活力值。</summary>
    public int remainingVitality;
}

/// <summary>小怪兽被击败。发布者：Zombie；订阅者：Wave, Reward。真源：Architecture §5。</summary>
public struct ZombieKilledEvent : IEvent
{
    /// <summary>实例 ID。</summary>
    public int zombieId;

    /// <summary>原型类型。</summary>
    public ZombieType type;

    /// <summary>击败位置。</summary>
    public Vector3 position;
}

/// <summary>小怪兽到达能量墙。发布者：Zombie；订阅者：Wall。真源：Architecture §5。</summary>
public struct ZombieReachedWallEvent : IEvent
{
    /// <summary>实例 ID。</summary>
    public int zombieId;

    /// <summary>原型类型。</summary>
    public ZombieType type;
}
