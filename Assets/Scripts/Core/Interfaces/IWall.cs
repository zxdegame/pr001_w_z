using System;

/// <summary>
/// 防线数值唯一主体。避难所无血量、不可被攻击（GDD D2）；全场唯一失败数值就是能量墙耐久。
/// 因此不存在第二个 IShelter/IBase 血量接口。
/// 真源：Architecture §4.5。
/// </summary>
public interface IWall
{
    /// <summary>当前耐久。</summary>
    int CurrentDurability { get; }

    /// <summary>上限 = 能量墙阶位（1 阶 100 → 5 阶 500）。</summary>
    int MaxDurability { get; }

    /// <summary>0-1，星级判定用。</summary>
    float DurabilityPercent { get; }

    /// <summary>CurrentDurability &lt;= 0，触发即失败。</summary>
    bool IsBroken { get; }

    /// <summary>受到伤害。</summary>
    void TakeDamage(int amount);

    /// <summary>修复（仅急救补给站触发，战斗中不得消耗能量晶修复，GDD D2/Q8）；不溢出 MaxDurability。</summary>
    void Repair(int amount);

    /// <summary>耐久变化（current, max）。</summary>
    event Action<int, int> OnWallChanged;

    /// <summary>墙体破碎事件。</summary>
    event Action OnWallBroken;
}
