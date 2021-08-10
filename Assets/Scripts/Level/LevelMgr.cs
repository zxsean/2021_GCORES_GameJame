using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 关卡管理器
/// </summary>
public static class LevelMgr
{
    public static Level CurLevel { get; private set; }

    private static int CurLevelId { get; set; }
    
    private static List<Level> levels = new List<Level>();

    private static List<GameObject> levelAssets = new List<GameObject>();

    public static void LoadLevels(GameObject[] assets)
    {
        for (var i = 0; i < assets.Length; ++i)
        {
            var ins = Object.Instantiate(assets[i], Vector3.zero, Quaternion.identity, Game.Root);
            levelAssets.Add(ins);
        }
    }

    /// <summary>
    /// 根据关卡数据创建关卡
    /// </summary>
    /// <param name="data"></param>
    public static void CreateAndEnterLevel(int levelId)
    {
        CurLevel?.Exit();

        var asset = levelAssets[levelId];
        var level = new Level(asset);
        levels.Add(level);
        CurLevel = level;
        CurLevel.Enter();
    }

    public static Level GetLevel(int levelId)
    {
        return levels[levelId];
    }

    public static void GetPosByRowAndCol(int row, int col, out Vector2 pos)
    {
        var level = CurLevel;
        pos.x = -level.Cols / 2.0f + col + 0.5f;
        pos.y = level.Rows / 2.0f - row - 0.5f;
    }
}