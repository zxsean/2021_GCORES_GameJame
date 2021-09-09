using UnityEngine;

/// <summary>
///     Grid基类
/// </summary>
public class Grid : IGrid
{
    public Grid(GameObject asset)
    {
        RawData = asset.GetComponent<GridData>();
        Row = RawData.row;
        Col = RawData.col;
        //LevelMgr.GetPosByRowAndCol(Row, Col, out var curPox);

        gameObject = asset;
        transform = asset.transform;
        Renderer = asset.GetComponent<Renderer>();

        var curPox = transform.localPosition;
        CurPosX = curPox.x;
        CurPosY = curPox.y;
    }

    protected GridData RawData { get; set; }

    /// <summary>
    ///     中心所在行
    /// </summary>
    public int Row { get; }

    /// <summary>
    ///     中心所在列
    /// </summary>
    public int Col { get; }

    /// <summary>
    ///     关联的游戏物体
    /// </summary>
    public GameObject gameObject { get; }

    /// <summary>
    ///     关联的游戏Transform
    /// </summary>
    public Transform transform { get; }

    /// <summary>
    ///     关联的渲染器
    /// </summary>
    public Renderer Renderer { get; protected set; }

    /// <summary>
    ///     中心所在x坐标
    /// </summary>
    public float CurPosX { get; set; }

    /// <summary>
    ///     中心所在y坐标
    /// </summary>
    public float CurPosY { get; set; }

    /// <summary>
    ///     指定Bounds是否在范围内
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public virtual bool InRange(Bounds bounds)
    {
        var selfBounds = Renderer.bounds;
        var extends = selfBounds.extents;
        extends.x -= 0.2f;
        extends.y -= 0.2f;
        selfBounds.extents = extends;
        return selfBounds.Intersects(bounds);
    }
}