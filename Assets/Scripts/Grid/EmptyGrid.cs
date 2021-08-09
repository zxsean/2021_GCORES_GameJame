using UnityEngine;

/// <summary>
/// 空地格子
/// </summary>
public class EmptyGrid : Grid
{
    private Bounds Bounds;
    
    public EmptyGrid(GridData data) : base(data)
    {
        Bounds = new Bounds(new Vector3(curPosX, curPosY), Vector3.one);
    }

    public override bool InRange(Bounds bounds)
    {
        return Bounds.Intersects(bounds);
    }
}