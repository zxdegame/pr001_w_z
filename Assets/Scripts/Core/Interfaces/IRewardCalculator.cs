/// <summary>
/// 结算奖励计算器契约。
/// 真源：Architecture §4.6。
/// 注：§4.6 原写参数类型 `BattleResult`，但 §3/§17 v1.11 已定 `BattleState` 为唯一战斗状态真源
///     （Victory/Defeat/Quit 为其子集）；经主程拍板复用 BattleState，Architecture §4.6 errata（v1.16）已记录。
/// </summary>
public interface IRewardCalculator
{
    /// <summary>按本局战果计算奖励结果。</summary>
    RewardResult Calculate(
        int normalKills,
        int eliteKills,
        int bossKills,
        BattleState result,
        int correctAnswers,
        int initialWordPoolCount,
        float durabilityPercent);
}
