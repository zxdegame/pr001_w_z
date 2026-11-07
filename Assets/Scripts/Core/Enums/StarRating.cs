/// <summary>
/// 关卡星级。真源 Architecture §3 / GDD §12（星级分母=本局实际首次出题数，v2.9 定案）。
/// </summary>
public enum StarRating
{
    /// <summary>失败（0 星，奖励 0.5x）。</summary>
    Fail = 0,

    /// <summary>一星：通关（1.0x）。</summary>
    OneStar = 1,

    /// <summary>二星：耐久百分比 &gt;50% 且答对率 &gt;70%（1.2x）。</summary>
    TwoStar = 2,

    /// <summary>三星：耐久百分比 &gt;80% 且答对率 &gt;90%（1.5x）。</summary>
    ThreeStar = 3
}
