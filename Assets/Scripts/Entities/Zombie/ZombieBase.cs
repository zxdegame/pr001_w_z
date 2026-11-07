using System;
using UnityEngine;

#pragma warning disable CS0067 // Phase 0 骨架：事件仅声明契约，玩法逻辑于 Phase 1+ 触发

/// <summary>
/// 所有小怪兽的抽象基类（实现 IZombie）。Phase 0 提供数据结构与契约骨架，玩法方法保留 NotImplementedException。
/// 真源：Architecture §4.1（GDD D8/D15/D16/D17/D18/D20/D21）。
/// </summary>
public abstract class ZombieBase : MonoBehaviour, IZombie
{
    /// <summary>唯一实例 ID（运行时生成）。</summary>
    public int Id { get; private set; }

    /// <summary>原型类型（子类给出）。</summary>
    public abstract ZombieType Type { get; }

    /// <summary>机制变体（D18）。</summary>
    public EnemyModifier Modifier { get; private set; }

    /// <summary>是否有 1 层护盾（D18）。</summary>
    public bool HasActiveShield { get; private set; }

    /// <summary>活力值上限（D8：200/600/2000）。</summary>
    public int MaxVitality { get; private set; }

    /// <summary>当前活力值。</summary>
    public int CurrentVitality { get; private set; }

    /// <summary>是否已到达能量墙。</summary>
    public bool IsReachedWall { get; private set; }

    /// <summary>是否存活。</summary>
    public bool IsAlive => CurrentVitality > 0;

    /// <summary>当前位置。</summary>
    public Vector3 Position => transform.position;

    /// <summary>到能量墙的距离。</summary>
    public float DistanceToWall => throw new NotImplementedException();

    /// <summary>受击事件。</summary>
    public event Action<IZombie, int> OnDamaged;

    /// <summary>到达能量墙事件。</summary>
    public event Action<IZombie> OnReachedWall;

    /// <summary>被击败事件。</summary>
    public event Action<IZombie> OnDied;

    /// <summary>扣减活力值（仅"答对"结算调用，Phase 1+ 实现）。</summary>
    public void TakeDamage(int amount) => throw new NotImplementedException();

    /// <summary>停顿（Phase 1+ 实现）。</summary>
    public void Pause(float duration) => throw new NotImplementedException();

    /// <summary>恢复推进（Phase 1+ 实现）。</summary>
    public void Resume() => throw new NotImplementedException();

    /// <summary>被击败流程（Phase 1+ 实现）。</summary>
    public void Die() => throw new NotImplementedException();
}
