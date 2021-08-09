
using System;
using UnityEngine;

/// <summary>
/// 格子数据
/// </summary>
[Serializable]
public class GridData
{
    public int levelId;
    public int type;
    public int row;
    public int col;
    public string prefab;
    public float posX;
    public float posY;
    public bool canPass;
}

[Serializable]
public class MovedBarrierGridData : GridData
{
    public Vector2[] path;
    public float speed;
}

/// <summary>
/// 玩家数据
/// </summary>
public class PlayerData
{
    public int levelId;
    public int row;
    public int col;
    public string prefab;
    public float speed;
    public float posX;
    public float posY;
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
public class LevelData
{
    public int levelId;
    public int rows;
    public int cols;
    public string prefab;
    public GridData[] gridData;
    public PlayerData playerData;
    public MonsterData[] monsterData;
}