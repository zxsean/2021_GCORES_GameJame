using UnityEngine;

public static class Game
{
    public static Transform Root { get; private set; }
    private static GameObject EndView { get; set; }
    private static GameObject StartView { get; set; }
    public static TransitionView TransitionView { get; private set; }
    public static BossView BossView { get; set; }
    
    public static AudioClip FootStepSound { get; set; }
    public static AudioClip DieSound { get; set; }
    public static AudioClip BulletShootSound { get; set; }
    public static AudioClip BulletHitSound { get; set; }
    
    public static AudioClip NormalMusic { get; set; }
    public static AudioClip BossMusic { get; set; }
    public static AudioClip EndMusic { get; set; }

    public static void Init()
    {
        CameraMgr.Init();
        Root = GameObject.Find("Game").transform;
        
        var startView = Resources.Load<GameObject>("Prefabs/UI/StartView");
        var endView = Resources.Load<GameObject>("Prefabs/UI/EndView");
        var transitionView = Resources.Load<GameObject>("Prefabs/UI/TransitionView");
        var bossView = Resources.Load<GameObject>("Prefabs/UI/BossView");
        var canvas = GameObject.Find("Canvas").transform;
        
        bossView = Object.Instantiate(bossView, Vector3.zero, Quaternion.identity);
        bossView.transform.SetParent(canvas, false);
        BossView = bossView.GetComponent<BossView>();
        transitionView = Object.Instantiate(transitionView, Vector3.zero, Quaternion.identity);
        transitionView.transform.SetParent(canvas, false);
        TransitionView = transitionView.GetComponent<TransitionView>();
        StartView = Object.Instantiate(startView, Vector3.zero, Quaternion.identity);
        StartView.transform.SetParent(canvas, false);
        EndView = Object.Instantiate(endView, Vector3.zero, Quaternion.identity);
        EndView.transform.SetParent(canvas, false);


        AudioMgr.Root = GameObject.Find("Audio");

        StartView.SetActive(true);
    }

    public static void Start()
    {
        Root.gameObject.SetActive(true);
        StartView.SetActive(false);
        EndView.SetActive(false);
        BossView.gameObject.SetActive(false);
        TransitionView.PlayHide();
        LevelMgr.CreateAndEnterLevel(0, false);
    }

    public static void ReStart()
    {
        Root.gameObject.SetActive(true);
        EndView.SetActive(false);
        BossView.gameObject.SetActive(false);
        LevelMgr.ReStart();
    }

    public static void Update()
    {
        LevelMgr.Update();
        AnimationMgr.Update();
        CameraMgr.Update();
        AudioMgr.Update();
    }

    public static void End()
    {
        TransitionView.PlayShow();
        AudioMgr.StopAllSound();
        //Root.gameObject.SetActive(false);
        EndView.SetActive(true);
        BossView.gameObject.SetActive(false);
    }

    public static void Destroy()
    {
        AnimationMgr.Clear();
        CameraMgr.Clear();
    }
}