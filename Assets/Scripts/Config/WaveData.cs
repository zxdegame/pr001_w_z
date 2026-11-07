using System;
using System.Collections.Generic;

/// <summary>
/// 单波配置（LevelConfig.waves[] 元素，[Serializable] 非独立 SO）。
/// 真源：DevTasks Config 层 + Levels.md 字段表（数值口径以 GDD §6.1 / 附录 D 为准）。
/// </summary>
[Serializable]
public class WaveData
{
    /// <summary>波次编号（1 起）。</summary>
    public int waveNumber;

    /// <summary>本波各类原型数量（Normal/Elite/Boss → 数量）。</summary>
    public Dictionary<ZombieType, int> zombieTypes;

    /// <summary>逐只刷怪条目（机制变体/species 走此层，D18/D20/D23）。</summary>
    public ZombieSpawnEntry[] spawns;

    /// <summary>事件波类型（D18：None/Rush/BossPressure/Ambush）。</summary>
    public WaveEventType eventType;

    /// <summary>刷怪间隔（秒）；Rush 波 ×0.6，Logistics Doctrine −1s。</summary>
    public float spawnInterval;
}

/// <summary>
/// 波内单只刷怪条目（LevelConfig.waves[].spawns[] 元素）。
/// 真源：Levels.md 字段表 / GDD D18/D20/D23。
/// </summary>
[Serializable]
public class ZombieSpawnEntry
{
    /// <summary>该只的原型类型（Normal/Elite/Boss，不新增枚举值，守 D3）。</summary>
    public ZombieType type;

    /// <summary>在本波序列中的索引位（用于精确定位叠加 modifier 的那只）。</summary>
    public int index;

    /// <summary>机制变体（D18/D20：None/Shielded/Sprinter/Mender/TwinWord/Wanderer）。</summary>
    public EnemyModifier[] modifiers;

    /// <summary>敌人种 ID（D23 可选，指 ZombieSpeciesConfig；缺省=该原型默认种）。</summary>
    public string speciesId;
}
