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

    /// <summary>
    /// 根据关卡数据创建关卡
    /// </summary>
    /// <param name="levels"></param>
    public static void CreateLevels(LevelData[] levels)
    {
        for (var i = 0; i < levels.Length; ++i)
        {
            CreateLevel(levels[i]);
        }
    }
    
    /// <summary>
    /// 根据关卡数据创建关卡
    /// </summary>
    /// <param name="data"></param>
    public static void CreateLevel(LevelData data)
    {
        var level = new Level(data);
        levels.Add(level);
        CurLevel = level;
        
        // init grid
        GridMgr.CreateGrids(data.gridData);

        // init player
        EntityMgr.CreatePlayer(data.playerData);

        // init monster
        EntityMgr.CreateMonsters(data.monsterData);
    }

    public static Level GetLevel(int levelId)
    {
        return levels[levelId];
    }

    /// <summary>
    /// 进入指定关卡
    /// </summary>
    /// <param name="levelId"></param>
    public static void EnterLevel(int levelId)
    {
        CurLevelId = levelId;
        CurLevel = levels[CurLevelId];
        CurLevel.Enter();
    }

    /// <summary>
    /// 进入下一关
    /// </summary>
    public static void EnterNextLevel()
    {
        CurLevel?.Exit();
        EnterLevel(CurLevelId + 1);
    }

    public static void GetPosByRowAndCol(int levelId, int row, int col, out float x, out float y)
    {
        var level = GetLevel(levelId);
        x = -level.Rows / 2.0f + col + 0.5f;
        y = -level.Cols / 2.0f + row + 0.5f;
    }
}