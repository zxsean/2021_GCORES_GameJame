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
        levelAssets.AddRange(assets);
    }

    /// <summary>
    /// 根据关卡数据创建关卡
    /// </summary>
    /// <param name="data"></param>
    public static void CreateAndEnterLevel(int levelId)
    {
        CurLevel?.Exit();

        if (levelId <= levelAssets.Count - 2)
        {
            AudioMgr.PlayContinueMusic(Game.NormalMusic);
        }
        else
        {
            AudioMgr.PlayContinueMusic(Game.BossMusic);
        }

        if (levelId >= levelAssets.Count)
        {
            // 已通过最终关，播放游戏Ending
            Game.End();
            return;
        }
        
        var asset = levelAssets[levelId];
        var ins = Object.Instantiate(asset, Vector3.zero, Quaternion.identity, Game.Root);
        var level = new Level(ins);
        levels.Add(level);
        CurLevel = level;
        CurLevelId = levelId;
        CurLevel.Enter();
    }

    public static void ReEnter()
    {
        CurLevel.Clear();
        levels.Remove(CurLevel);
        Object.Destroy(CurLevel.gameObject);
        CurLevel = null;
        CreateAndEnterLevel(CurLevelId);
    }

    /// <summary>
    /// 下一关
    /// </summary>
    public static void NextLevel()
    {
        CreateAndEnterLevel(CurLevelId + 1);
    }

    public static void ReStart()
    {
        Clear();
        CreateAndEnterLevel(0);
    }

    public static void Update()
    {
        CurLevel?.Update();
    }

    public static void Clear()
    {
        CurLevel.Exit();
        for (var i = 0; i < levels.Count; ++i)
        {
            var level = levels[i];
            Object.Destroy(level.gameObject);
        }
        levels.Clear();
        CurLevel = null;
        CurLevelId = 0;
    }

    public static void GetPosByRowAndCol(int row, int col, out Vector2 pos)
    {
        var level = CurLevel;
        pos.x = -level.Cols / 2.0f + col + 0.5f;
        pos.y = level.Rows / 2.0f - row - 0.5f;
    }
}