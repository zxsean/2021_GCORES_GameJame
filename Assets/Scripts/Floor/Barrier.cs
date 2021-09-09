using UnityEngine;

/// <summary>
///     静止的障碍物
///     Entity不能通过，需要绕开
/// </summary>
public class Barrier : Grid, IFloor
{
    public Barrier(GameObject asset) : base(asset)
    {
        Type = ((FloorData) RawData).type;
    }

    public FloorType Type { get; }

    public override bool InRange(Bounds bounds)
    {
        var selfBounds = Renderer.bounds;
        var extends = selfBounds.extents;
        extends.x -= 0.3f;
        extends.y -= 0.3f;
        selfBounds.extents = extends;
        return selfBounds.Intersects(bounds);
    }
}