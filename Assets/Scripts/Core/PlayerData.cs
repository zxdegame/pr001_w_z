using System;
using System.Collections.Generic;

/// <summary>
/// 玩家永久存档数据（SaveManager 序列化对象）。
/// 真源：Architecture §9。字段严格照 §9 canonical 块，后续玩法字段（activeSupplyPacks 等）待对应 Phase 走 RFC 再加。
/// </summary>
[Serializable]
public class PlayerData
{
    // —— 资源 ——
    /// <summary>能量晶（本作唯一货币，GDD D1）。</summary>
    public int energyCrystal;

    /// <summary>金币。</summary>
    public int gold;

    /// <summary>武器碎片。</summary>
    public int weaponShard;

    // —— 关卡进度 ——
    /// <summary>已解锁关卡 ID 列表。</summary>
    public List<string> unlockedLevelIds;

    /// <summary>各关卡星级（levelId → star 0-3）。</summary>
    public Dictionary<string, int> levelStarRatings;

    // —— 武器 ——
    /// <summary>武器等级 Lv.1-7（GDD §2.2.5 武器表 / 附录 D 碎片总量 790）。</summary>
    public int weaponLevel;

    /// <summary>当前累积武器碎片。</summary>
    public int currentWeaponFragment;

    // —— 防线建筑 ——
    /// <summary>箭塔等级 1-5。</summary>
    public int towerLevel;

    /// <summary>能量墙等级 1-5 → 决定本局初始耐久上限（100/200/300/400/500）。</summary>
    public int energyWallLevel;

    /// <summary>急救补给站等级 1-5。</summary>
    public int firstAidStationLevel;

    /// <summary>瞭望台等级 1-5。</summary>
    public int watchtowerLevel;

    // —— 本地最佳纪录（代替排行榜，仅本机；GDD D4）——
    /// <summary>历史最高分。</summary>
    public int bestScore;

    /// <summary>历史最高连击。</summary>
    public int bestCombo;

    /// <summary>历史最高耐久百分比。</summary>
    public int bestDurabilityPercent;

    // —— 补给包等级 ——
    /// <summary>各类补给包的等级。</summary>
    public Dictionary<SupplyPackType, int> supplyPackLevels;

    // —— 设置 ——
    /// <summary>答题时限 10-20 秒。</summary>
    public float answerTimeLimit;

    /// <summary>BGM 音量。</summary>
    public float bgmVolume;

    /// <summary>SFX 音量。</summary>
    public float sfxVolume;

    /// <summary>是否启用震动。</summary>
    public bool vibrationEnabled;

    // —— 统计 ——
    /// <summary>累计掌握单词数。</summary>
    public int totalWordsMastered;

    /// <summary>累计击败小怪兽数。</summary>
    public int totalZombiesKilled;

    /// <summary>上次游玩日期。</summary>
    public DateTime lastPlayDate;

    /// <summary>连续高准确局数（用于动态难度）。</summary>
    public int consecutiveHighAccuracyRuns;
}
