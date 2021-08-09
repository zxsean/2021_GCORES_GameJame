using UnityEngine;

/// <summary>
/// 静止的障碍物
/// Entity不能通过，需要绕开
/// </summary>
public class BarrierGrid : Grid
{
    private Bounds Bounds;
    
    public BarrierGrid(GridData data) : base(data)
    {
        Bounds = new Bounds(new Vector3(curPosX, curPosY), Vector3.one);
    }
    
    public override bool InRange(Bounds bounds)
    {
        return Bounds.Intersects(bounds);
    }
}