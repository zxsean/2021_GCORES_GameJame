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
}

public interface IFloor
{
    FloorType Type { get; }
}