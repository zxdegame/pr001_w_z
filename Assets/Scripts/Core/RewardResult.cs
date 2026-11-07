using System;

/// <summary>
/// 一局结算奖励结果（IRewardCalculator 输出）。
/// 真源：Architecture §4.6。
/// </summary>
[Serializable]
public struct RewardResult
{
    /// <summary>发放的能量晶（本作唯一货币）。</summary>
    public int energyCrystal;

    /// <summary>发放的金币。</summary>
    public int gold;

    /// <summary>发放的武器碎片。</summary>
    public int weaponShard;

    /// <summary>本局星级评定。</summary>
    public StarRating starRating;

    /// <summary>奖励倍率（加成/难度折算系数）。</summary>
    public float rewardMultiplier;

    /// <summary>本次写入错题本的错词数量。</summary>
    public int wrongWordsAdded;
}
