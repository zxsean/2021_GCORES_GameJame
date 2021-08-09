using UnityEngine;

/// <summary>
/// Entity接口
/// </summary>

public interface IEntity : IUpdatable
{
    bool IsDestroy { get; }
    float CurPosX { get; set; }
    float CurPosY { get; set; }
    Bounds Bounds { get; }
}