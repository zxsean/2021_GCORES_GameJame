using UnityEngine;

public interface ITriggerGrid : IGrid
{
}

public interface IGrid
{
    /// <summary>
    ///     中心所在行
    /// </summary>
    int Row { get; }

    /// <summary>
    ///     中心所在列
    /// </summary>
    int Col { get; }

    /// <summary>
    ///     关联的游戏物体
    /// </summary>
    GameObject gameObject { get; }

    /// <summary>
    ///     关联的游戏Transform
    /// </summary>
    Transform transform { get; }

    /// <summary>
    ///     关联的渲染器
    /// </summary>
    Renderer Renderer { get; }

    /// <summary>
    ///     中心所在x坐标
    /// </summary>
    float CurPosX { get; set; }

    /// <summary>
    ///     中心所在y坐标
    /// </summary>
    float CurPosY { get; set; }

    bool InRange(Bounds bounds);
}