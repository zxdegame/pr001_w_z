/// <summary>
/// 敌人机制变体（GDD §12 D18/D20）。逐只在 LevelConfig.waves[].spawns[].modifiers[] 配置。
/// **不新增 ZombieType**（三类原型不变；D23 扩种走 ZombieSpeciesConfig 数据层）。
/// </summary>
public enum EnemyModifier
{
    /// <summary>无变体。</summary>
    None = 0,

    /// <summary>护盾怪（Phase3）：叠加护盾 ring。</summary>
    Shielded,

    /// <summary>急速怪（Phase3）：速度线残影。</summary>
    Sprinter,

    /// <summary>治疗怪（Phase3）：MenderTickSeconds=5 周期治疗。</summary>
    Mender,

    /// <summary>双词怪（Phase4）：头顶双色词卡。</summary>
    TwinWord,

    /// <summary>稀有游走奖励怪（GDD D20，Phase3）：进答题圈停止推进、随机游走、永不攻墙、不计通关。</summary>
    Wanderer
}
