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

    public AudioClip normalMusic;
    public AudioClip bossMusic;
    public AudioClip endMusic;

    private void Awake()
    {
        Game.Init();
        Game.FootStepSound = footstepSound;
        Game.DieSound = dieSound;
        Game.BulletShootSound = bulletShootSound;
        Game.BulletHitSound = bulletHitSound;
        Game.NormalMusic = normalMusic;
        Game.BossMusic = bossMusic;
        Game.EndMusic = endMusic;
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