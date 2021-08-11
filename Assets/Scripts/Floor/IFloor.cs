/// <summary>
/// Floor类型
/// </summary>
public enum FloorType
{
    // 静止障碍
    Barrier,
    // 移动障碍
    MovedBarrier,
    // 尖刺
    Spike,
    // 移动尖刺
    MovedSpike,
    // 飞行物
    Flyer,
    // 时停果实
    TimeStopFruit,
    // 触发器
    Trigger,
    // 幻象生成点
    IllusionGen,
    // 地刺陷阱
    SpikeTrap,
    // 反转陷阱
    RevertTrap
}

public interface ITriggerFloor : IFloor
{
    int TriggerId { get; }

    bool Trigger(ITriggerGrid trigger);
}

public interface IFloor
{
    FloorType Type { get; }
}