/// <summary>
/// 本地商店：仅消耗游戏内货币，无任何真实货币路径（GDD D1，0 内购）。
/// 真源：Architecture §4.11。
/// </summary>
public interface IStoreService
{
    /// <summary>在售列表（词库/补给包/记忆卡）。</summary>
    StoreCatalog GetCatalog();

    /// <summary>购买，只接受 EnergyCrystal。</summary>
    StoreResult Purchase(string itemId, PurchaseCurrency currency);

    /// <summary>掌握度门槛检查。</summary>
    bool CanUnlock(string wordBankId, out UnlockBlockReason reason);
}

/// <summary>购买结果。真源：Architecture §4.11。</summary>
public struct StoreResult
{
    /// <summary>是否成功。</summary>
    public bool success;

    /// <summary>消耗能量晶数量。</summary>
    public int costCrystal;

    /// <summary>失败原因：INSUFFICIENT_CRYSTAL / LOCKED_BY_MASTERY / ALREADY_OWNED。</summary>
    public string failReason;
}

/// <summary>
/// 在售商品目录。
/// TODO(RFC)：Phase 0 仅编译占位；真实商品结构（词库/补给包/记忆卡条目）随主策划商店出表补全。
/// </summary>
public class StoreCatalog
{
}

/// <summary>
/// 解锁受阻原因。
/// TODO(RFC)：Phase 0 仅编译占位；具体门槛枚举随商店/词库解锁规则出表补全。
/// </summary>
public enum UnlockBlockReason
{
    /// <summary>无阻碍（可解锁）。</summary>
    None = 0
}
