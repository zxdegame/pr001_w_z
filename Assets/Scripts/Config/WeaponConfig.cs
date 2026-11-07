using System;
using UnityEngine;

/// <summary>
/// 武器配置 SO（碎片/等级/效果）。仅字段、无逻辑。
/// 真源：TechStack §SO 表 / GDD §2.2.5 武器表（Lv.1-7，附录 D 碎片总量 790）。数值由 Editor 资产填充。
/// </summary>
[CreateAssetMenu(menuName = "Refuge/Weapon Config", fileName = "WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    /// <summary>逐级配置（Lv.1-7）。</summary>
    public WeaponLevelEntry[] levels;
}

/// <summary>武器单级配置。真源：GDD §2.2.5 / 附录 D。</summary>
[Serializable]
public class WeaponLevelEntry
{
    /// <summary>等级 1-7。</summary>
    public int level;

    /// <summary>攻击力（基础武器=100，每发暴击=攻×200%，GDD D8）。</summary>
    public int attack;

    /// <summary>升级到该级所需武器碎片。</summary>
    public int fragmentCost;
}
