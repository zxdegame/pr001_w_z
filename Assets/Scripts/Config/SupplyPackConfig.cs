using System;
using UnityEngine;

/// <summary>
/// 补给包配置 SO（7 种 + 分类 + 掉落保底 + 囤积上限）。仅字段、无逻辑。
/// 真源：TechStack §SO 表 / DevTasks D16（控制类 Ice/Slow/Confuse=0 伤害，转化类 Lightning/Spread/Fire/Pierce 扩散答对暴击）。
/// </summary>
[CreateAssetMenu(menuName = "Refuge/Supply Pack Config", fileName = "SupplyPackConfig")]
public class SupplyPackConfig : ScriptableObject
{
    /// <summary>7 种补给包条目。</summary>
    public SupplyPackEntry[] packs;

    /// <summary>每关掉落保底：控制类最少（GDD D16≥1）。</summary>
    public int minControlPerLevel;

    /// <summary>每关掉落保底：转化类最少（GDD D16≥1）。</summary>
    public int minConversionPerLevel;

    /// <summary>同时生效上限（GDD D16：≤2 = 1 控制 + 1 转化）。</summary>
    public int maxActivePacks;
}

/// <summary>单个补给包配置。真源：GDD D16/D17。</summary>
[Serializable]
public class SupplyPackEntry
{
    /// <summary>补给包类型。</summary>
    public SupplyPackType type;

    /// <summary>true=控制类（Ice/Slow/Confuse，0 伤害）；false=转化类（扩散答对暴击）。</summary>
    public bool isControlType;

    /// <summary>最大等级（通常 3）。</summary>
    public int maxLevel;
}
