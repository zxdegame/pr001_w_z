/// <summary>
/// 局内战术构筑（GDD §12 D18）。关卡开始前 3 选 1，**仅本局生效、不写回永久存档**；与星级/经济解耦；doctrineEnabled=false 时不弹。
/// 效果写入 BattleModifiers（运行时加成容器）。
/// </summary>
public enum BattleDoctrine
{
    /// <summary>无。</summary>
    None = 0,

    /// <summary>火力：attackMultiplier=1.2。</summary>
    Fire,

    /// <summary>壁垒：wallDurabilityMultiplier=1.3（守 D9，仅本局）。</summary>
    Bulwark,

    /// <summary>后勤：掉落保底 +1 且 waveIntervalSeconds −1。</summary>
    Logistics
}
