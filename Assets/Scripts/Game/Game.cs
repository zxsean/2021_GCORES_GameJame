using UnityEngine;

public static class Game
{
    public static Transform Root { get; private set; }
    
    private static GameObject EndView { get; set; }
    
    public static void Init()
    {
        Root = GameObject.Find("Game").transform;
        EndView = GameObject.Find("Canvas").transform.Find("EndView").gameObject;
    }

    public static void Start()
    {
        Root.gameObject.SetActive(true);
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
    }

    public static void End()
    {
        Root.gameObject.SetActive(false);
        EndView.SetActive(true);
    }

    public static void Destroy()
    {
        
    }
}