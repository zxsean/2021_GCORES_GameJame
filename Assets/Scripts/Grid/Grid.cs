
using UnityEngine;

/// <summary>
/// Grid类型
/// </summary>
public enum GridType
{
    // 空地
    Empty,
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
}

/// <summary>
/// Grid基类
/// </summary>
public class Grid : IUpdatable
{
    /// <summary>
    /// 中心所在行
    /// </summary>
    public int Row { get; private set; }
    /// <summary>
    /// 中心所在列
    /// </summary>
    public int Col { get; private set; }
    /// <summary>
    /// 是否可同行
    /// </summary>
    public bool CanPass { get; set; }
    /// <summary>
    /// 中心所在x坐标
    /// </summary>
    public float curPosX;
    /// <summary>
    /// 中心所在y坐标
    /// </summary>
    public float curPosY;
    /// <summary>
    /// 关联的游戏物体
    /// </summary>
    public GameObject gameObject { get; private set; }
    /// <summary>
    /// 关联的游戏Transform
    /// </summary>
    public Transform transform { get; private set; }

    public Grid(GridData data)
    {
        Row = data.row;
        Col = data.col;
        LevelMgr.GetPosByRowAndCol(data.levelId, Row, Col, out curPosX, out curPosY);

        var prefab = Resources.Load(data.prefab);
        gameObject = Object.Instantiate(
            prefab, 
            new Vector3(curPosX, curPosY),
            Quaternion.identity, 
            LevelMgr.CurLevel.GridRoot) as GameObject;
        transform = gameObject.transform;
    }

    /// <summary>
    /// 指定Bounds是否在范围内
    /// </summary>
    /// <param name="bounds"></param>
    /// <returns></returns>
    public virtual bool InRange(Bounds bounds)
    {
        return false;
    }

    public virtual void Update()
    {
        
    }
}