using System;
using UnityEngine;

/// <summary>
/// 所有小怪兽的公共契约。答题状态机通过此接口与小怪兽交互。
/// 真源：Architecture §4.1（GDD D8/D15/D16/D17/D18/D20/D21）。
/// </summary>
public interface IZombie
{
    // ---- 数据 ----
    /// <summary>唯一实例 ID（运行时生成）。</summary>
    int Id { get; }

    /// <summary>原型类型（Normal/Elite/Boss，D3 三类不变）。</summary>
    ZombieType Type { get; }

    /// <summary>D18：叠加机制变体（不改变 Type）；Shielded 时 HasActiveShield 为 true。</summary>
    EnemyModifier Modifier { get; }

    /// <summary>D18：有 1 层护盾时，首次 TakeDamage 只破盾、不扣 CurrentVitality。</summary>
    bool HasActiveShield { get; }

    /// <summary>活力值上限：小怪=200，大怪=600，BOSS=2000（GDD D8）。</summary>
    int MaxVitality { get; }

    /// <summary>当前活力值（不叫 HP/血量；玩家可见文案为"活力值"）。</summary>
    int CurrentVitality { get; }

    /// <summary>是否已到达能量墙。</summary>
    bool IsReachedWall { get; }

    /// <summary>是否存活。</summary>
    bool IsAlive { get; }

    /// <summary>当前位置。</summary>
    Vector3 Position { get; }

    /// <summary>到能量墙的距离。</summary>
    float DistanceToWall { get; }

    // ---- 行为 ----
    /// <summary>扣减活力值（amount=武器攻击力×200%，仅"答对"结算调用，GDD D8）；归零触发被击败。</summary>
    void TakeDamage(int amount);

    /// <summary>停顿一段时间（不影响其他小怪兽）。</summary>
    void Pause(float duration);

    /// <summary>恢复推进。</summary>
    void Resume();

    /// <summary>播放被净化动画 → 触发掉落 → 销毁。</summary>
    void Die();

    // ---- 事件（内部）----
    /// <summary>受击事件（zombie, deltaVitality）。</summary>
    event Action<IZombie, int> OnDamaged;

    /// <summary>到达能量墙事件。</summary>
    event Action<IZombie> OnReachedWall;

    /// <summary>被击败事件。</summary>
    event Action<IZombie> OnDied;
}
