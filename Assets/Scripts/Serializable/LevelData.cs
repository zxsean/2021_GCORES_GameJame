using UnityEngine;

/// <summary>
///     关卡数据
/// </summary>
public class LevelData : MonoBehaviour
{
    public int cols;
    public GameObject ground;

    [HideInInspector] public float height;

    public int rows;

    [HideInInspector] public float width;
}