using System;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    #region Unity Events

    public GameObject[] levelData;

    private void Awake()
    {
        Game.Init();
    }

    private void Start()
    {
        LevelMgr.LoadLevels(levelData);
        Game.Start();
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