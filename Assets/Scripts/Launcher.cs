using System;
using UnityEngine;

public class Launcher : MonoBehaviour
{
    #region Unity Events

    public GameObject[] levelData;

    public AudioClip footstepSound;
    public AudioClip dieSound;
    public AudioClip bulletShootSound;
    public AudioClip bulletHitSound;

    private void Awake()
    {
        Game.Init();
        Game.FootStepSound = footstepSound;
        Game.DieSound = dieSound;
        Game.BulletShootSound = bulletShootSound;
        Game.BulletHitSound = bulletHitSound;
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