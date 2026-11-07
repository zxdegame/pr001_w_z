/// <summary>
/// 答题状态机通过此接口向战斗系统索取"下一个待答目标的小怪兽"。
/// 真源：Architecture §4.2。
/// </summary>
public interface IQuizTargetProvider
{
    /// <summary>返回进入答题圈的下一只小怪兽。</summary>
    IZombie AcquireNextTarget();

    /// <summary>当前是否有待答目标。</summary>
    bool HasPendingTarget { get; }
}
