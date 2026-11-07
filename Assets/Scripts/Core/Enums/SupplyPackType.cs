/// <summary>
/// 补给包类型。真源 Architecture §3 / GDD §12 D16。
/// 控制类=Ice/Slow/Confuse：**不造成任何伤害**，仅延长答题窗口/改移动状态；转化类=Lightning/Spread/Fire/Pierce：把一次答对的暴击扩散到多目标，伤害仍由“答对”触发。
/// 同时生效上限：1 控制 + 1 转化（PlayerData.activeSupplyPacks 长度 ≤ 2）。
/// </summary>
public enum SupplyPackType
{
    /// <summary>控制类：冰冻。</summary>
    Ice,

    /// <summary>控制类：减速。</summary>
    Slow,

    /// <summary>转化类：闪电链。</summary>
    Lightning,

    /// <summary>控制类：迷惑。</summary>
    Confuse,

    /// <summary>转化类：分裂弹头。</summary>
    Spread,

    /// <summary>转化类：同源暴击火焰。</summary>
    Fire,

    /// <summary>转化类：穿透轨。</summary>
    Pierce
}
