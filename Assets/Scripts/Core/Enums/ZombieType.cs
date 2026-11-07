/// <summary>
/// 敌人原型枚举。真源 GDD §12 D3 / D23 修订：只有小怪兽 / 大怪兽 / BOSS 怪三类原型。
/// 不存在快速小怪兽、坦克小怪兽、最终 BOSS；D23 的 30 种敌人属原型下 species，走 ZombieSpeciesConfig 数据层，**不得新增本枚举值**。
/// </summary>
public enum ZombieType
{
    /// <summary>小怪兽（基础原型，活力 200）。</summary>
    Normal = 0,

    /// <summary>大怪兽（中型原型，活力 600）。</summary>
    Elite = 1,

    /// <summary>BOSS 怪（每局仅 1 只，活力 2000）。</summary>
    Boss = 2
}
