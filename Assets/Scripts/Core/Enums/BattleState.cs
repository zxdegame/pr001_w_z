/// <summary>
/// 战斗场景顶层状态机（6 态）。真源 Architecture §3 / BattleState。
/// 失败优先级高于胜利（BattleFlowController 判定）。
/// </summary>
public enum BattleState
{
    /// <summary>加载中。</summary>
    Loading,

    /// <summary>运行中。</summary>
    Running,

    /// <summary>暂停。</summary>
    Paused,

    /// <summary>胜利。</summary>
    Victory,

    /// <summary>失败（能量墙耐久归零，唯一失败条件，GDD D2）。</summary>
    Defeat,

    /// <summary>退出。</summary>
    Quit
}
