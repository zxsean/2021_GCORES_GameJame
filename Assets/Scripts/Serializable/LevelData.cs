
using System;
using UnityEngine;

public class MovedBarrierGridData : GridData
{
    public Vector2[] path;
    public float speed;
}

/// <summary>
/// 怪物数据
/// </summary>
public class MonsterData
{
    public int levelId;
    public float posX;
    public float posY;
    public float speed;
    public string prefab;
}

/// <summary>
/// 关卡数据
/// </summary>
public class LevelData : MonoBehaviour
{
    public int levelId;
    public int rows;
    public int cols;
    public string prefab;
    public GridData[] gridData;
    public PlayerData playerData;
    public MonsterData[] monsterData;
}