using System;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    #region Unity Events

    private void Start()
    {
        //生成个地图10x10
        var level = new LevelData
        {
            levelId = 0,
            rows = 10,
            cols = 10,
            gridData = new GridData[10 * 10],
            playerData = new PlayerData
            {
                levelId = 0,
                row = 0,
                col = 5,
                speed = 5.0f,
                prefab = "Prefabs/Player"
            },
            prefab = "Prefabs/Level"
        };
        for (var i = 0; i < 10; ++i)
        {
            for (var j = 0; j < 10; ++j)
            {
                var grid = new GridData
                {
                    levelId = 0,
                    row = i,
                    col = j,
                    type = (i != 2 && j != 3) ? (int) GridType.Empty : (int) GridType.Barrier,
                    prefab = i != 2 && j != 3 ? "Prefabs/EmptyGrid" : "Prefabs/BarrierGrid"
                };
                level.gridData[j + 10 * i] = grid;
            }
        }
        LevelMgr.CreateLevel(level);

        Game.Init();
    }

    private void Update()
    {
        Game.Update();
    }

    private void OnDestroy()
    {
        Game.Destroy();
    }

    #endregion

}