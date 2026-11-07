/// <summary>
/// 购买货币（GDD §12 D1 / Compliance §3.3）。**全局唯一值 EnergyCrystal**：0 内购、能量晶不可真钱兑换。
/// EditMode 测试反射断言 Enum.GetNames(typeof(PurchaseCurrency)).Length == 1，禁止默默新增第二货币。
/// </summary>
public enum PurchaseCurrency
{
    /// <summary>能量晶——游戏内唯一可用购买货币。</summary>
    EnergyCrystal
}
