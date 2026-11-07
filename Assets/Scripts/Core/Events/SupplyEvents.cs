/// <summary>补给包掉落（弹出三选一）。发布者：Supply；订阅者：UI。真源：Architecture §5。</summary>
public struct SupplyPackDroppedEvent : IEvent
{
    /// <summary>掉落主类型。</summary>
    public SupplyPackType packType;

    /// <summary>本次可选的补给包列表（UI.md「3 卡横排」）。</summary>
    public SupplyPackType[] options;
}

/// <summary>补给包被选择。发布者：UI；订阅者：Supply。真源：Architecture §5。</summary>
public struct SupplyPackSelectedEvent : IEvent
{
    /// <summary>被选中的补给包类型。</summary>
    public SupplyPackType packType;

    /// <summary>
    /// 转化类首射目标僵尸实例 ID（从该目标扩散"答对"暴击）。
    /// TODO(RFC)：真源 §5 仅给字段名，类型按 zombieId(int) 推定，待 Supply 系统出表确认。
    /// </summary>
    public int targetZombieFirstShot;
}

/// <summary>补给包效果生效。发布者：Supply；订阅者：Effects。真源：Architecture §5。</summary>
public struct SupplyPackEffectAppliedEvent : IEvent
{
    /// <summary>补给包类型。</summary>
    public SupplyPackType packType;

    /// <summary>生效等级。</summary>
    public int level;
}
