/// <summary>
/// 全局代码级常量（真源 Architecture §2 / §17）。
/// 编码规范：可配数值一律走 ScriptableObject（见 Config 层），此处仅放非配置的换算/结构常量，杜绝代码内 magic number。
/// </summary>
public static class Constants
{
    /// <summary>1 米对应的像素数（PPU=100，与附录 D / GDD D15 一致）。用于把 SO 中 distanceInMeters 转换为像素坐标。</summary>
    public const int PIXELS_PER_METER = 100;
}
