/// <summary>
/// 答题状态机状态。真源 Architecture §3 / GDD §12 D8 答题流程；UI 表现层只订阅、不自造状态机。
/// 8 态：IDLE → QUESTION_LOADING → WAITING_ANSWER →（CORRECT/WRONG/TIMEOUT)_RESOLVE →（ZOMBIE_KILLED）→ NEXT_QUESTION。
/// </summary>
public enum QuizState
{
    /// <summary>空闲：无待答目标。</summary>
    IDLE,

    /// <summary>题目加载中。</summary>
    QUESTION_LOADING,

    /// <summary>等待作答（时限走 QuizConfig）。</summary>
    WAITING_ANSWER,

    /// <summary>答对结算（当帧暴击，伤害=武器攻击力×200%）。</summary>
    CORRECT_RESOLVE,

    /// <summary>答错结算（错题入本、连击清零、目标嘲讽气泡）。</summary>
    WRONG_RESOLVE,

    /// <summary>超时结算（视同答错，不造成伤害）。</summary>
    TIMEOUT_RESOLVE,

    /// <summary>目标被击败。</summary>
    ZOMBIE_KILLED,

    /// <summary>串接下一题。</summary>
    NEXT_QUESTION
}
