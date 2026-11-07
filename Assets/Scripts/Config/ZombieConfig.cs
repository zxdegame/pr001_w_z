using System;
using UnityEngine;

/// <summary>
/// 小怪兽原型配置 SO（活力值/速度/掉落）。仅字段、无逻辑。
/// 真源：TechStack §SO 表（GDD D8/D17/D18/D20/D23）。数值由 Editor 资产按 GDD 附录 D 填充。
/// </summary>
[CreateAssetMenu(menuName = "Refuge/Zombie Config", fileName = "ZombieConfig")]
public class ZombieConfig : ScriptableObject
{
    /// <summary>三类原型配置（Normal/Elite/Boss）。</summary>
    public ZombieProfile[] profiles;

    /// <summary>Mender 回复节拍（秒，GDD D18=5）。</summary>
    public float menderTickSeconds;

    /// <summary>游走怪随机移速（m/s，GDD D20=0.6）。</summary>
    public float wanderSpeed;

    /// <summary>D20 游走奖励怪掉落覆盖（金库怪能量晶 20-40 / 铁匠怪碎片 15-30）。</summary>
    public ZombieDrop dropOverride;
}

/// <summary>单个原型配置。真源：GDD 附录 D。</summary>
[Serializable]
public class ZombieProfile
{
    /// <summary>原型类型。</summary>
    public ZombieType type;

    /// <summary>活力值上限（小怪200/大怪600/BOSS2000，GDD D8）。</summary>
    public int maxVitality;

    /// <summary>移动速度（m/s，普1.0/大1.2/BOSS0.7）。</summary>
    public float moveSpeed;

    /// <summary>掉落。</summary>
    public ZombieDrop drop;
}

/// <summary>掉落区间。真源：GDD 附录 D / D20。</summary>
[Serializable]
public class ZombieDrop
{
    /// <summary>能量晶掉落下限。</summary>
    public int energyCrystalMin;

    /// <summary>能量晶掉落上限。</summary>
    public int energyCrystalMax;

    /// <summary>武器碎片掉落下限。</summary>
    public int weaponShardMin;

    /// <summary>武器碎片掉落上限。</summary>
    public int weaponShardMax;
}
