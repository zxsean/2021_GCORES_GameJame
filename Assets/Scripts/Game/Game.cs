using UnityEngine;

public static class Game
{
    public static Transform Root { get; private set; }
    private static GameObject EndView { get; set; }
    private static GameObject StartView { get; set; }
    
    public static AudioClip FootStepSound { get; set; }
    public static AudioClip DieSound { get; set; }
    public static AudioClip BulletShootSound { get; set; }
    public static AudioClip BulletHitSound { get; set; }
    
    public static AudioClip NormalMusic { get; set; }
    public static AudioClip BossMusic { get; set; }
    public static AudioClip EndMusic { get; set; }

    public static void Init()
    {
        Root = GameObject.Find("Game").transform;
        StartView = GameObject.Find("Canvas").transform.Find("StartView").gameObject;
        EndView = GameObject.Find("Canvas").transform.Find("EndView").gameObject;
        AudioMgr.Root = GameObject.Find("Audio");
        
        StartView.SetActive(true);
    }

    public static void Start()
    {
        Root.gameObject.SetActive(true);
        StartView.SetActive(false);
        EndView.SetActive(false);
        LevelMgr.CreateAndEnterLevel(0);
    }

    public static void ReStart()
    {
        Root.gameObject.SetActive(true);
        EndView.SetActive(false);
        LevelMgr.ReStart();
    }

    public static void Update()
    {
        LevelMgr.Update();
        AnimationMgr.Update();
    }

    public static void End()
    {
        Root.gameObject.SetActive(false);
        EndView.SetActive(true);
    }

    public static void Destroy()
    {
        AnimationMgr.Clear();
    }
}