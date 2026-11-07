using UnityEngine;

#pragma warning disable CS0649 // levelConfig 由 Unity 序列化 / Inspector 赋值

/// <summary>
/// 战斗场景装配器（Phase 0 骨架，不实现玩法）。
/// 完整装配顺序见 Architecture §11；玩法系统（WordPoolManager/WallManager/WaveController/
/// QuizStateMachine/RewardCalculator/BattleFlowController）于 Phase 1+ 接入。
/// 真源：Architecture §6.3 / §11 / §17。
/// </summary>
public class BattleBootstrap : MonoBehaviour
{
    [SerializeField] LevelConfig levelConfig;

    private void Awake()
    {
        // Phase 0：仅装配占位，不实例化任何玩法组件。
        Debug.Log($"[BattleBootstrap] ready, levelConfig={(levelConfig != null ? levelConfig.name : "<none>")}");
    }
}
